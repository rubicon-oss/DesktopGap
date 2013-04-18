// This file is part of DesktopGap (desktopgap.codeplex.com)
// Copyright (c) rubicon IT GmbH, Vienna, and contributors
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Additional permissions are listed in the file DesktopGap_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Events.System;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Resources;
using DesktopGap.WebBrowser;
using NUnit.Framework;
using Rhino.Mocks;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class DragAndDropAddInTest
  {
    private const string c_fakeEventID = "GUID identifying the event in JS";

    private readonly string[] _filePaths = new[]
                                           {
                                               @"C:\Development\desktopgap",
                                               @"C:\Development\desktopgap\Core",
                                               @"C:\Development\desktopgap\license\gpl-v2.0.txt",
                                               @"C:\Development\desktopgap\Clients.Windows"
                                           };

    private readonly HtmlDocumentHandle _documentHandle = new HtmlDocumentHandle (Guid.NewGuid());
    private HtmlElementData _droppableElementData;
    private HtmlElementData _nonDroppableElementData;

    private DragAndDropAddIn _dragAndDropAddIn;


    [SetUp]
    public void SetUp ()
    {
      _dragAndDropAddIn = new DragAndDropAddIn (new ResourceManager());

      var attributesDictionary = new Dictionary<string, string>
                                 {
                                     { DragAndDropAddIn.DropConditionAttributeName, "1" },
                                     { DragAndDropAddIn.DroppableAttributeName, "true" }
                                 };


      _droppableElementData = new HtmlElementData ("unique-id", attributesDictionary);
      _nonDroppableElementData = new HtmlElementData ("unique-id2", new Dictionary<string, string>());
    }

    [Test]
    public void RemoveDragDropSource_RemoveSource_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      Assert.That (() => _dragAndDropAddIn.RemoveDragDropSource (browserStub), Throws.Nothing);
    }

    [Test]
    public void ShouldRaiseEvent_InvalidCondition_ShouldThrowArgumentException ()
    {
      Assert.That (
          () => _dragAndDropAddIn.CheckRaiseCondition (new Condition (CreateCondition())),
          Throws.ArgumentException.With.Message.EqualTo ("The provided 'Criteria' object does not have the required property 'elementID'."));
    }

    [Test]
    public void DragDrop_NoFilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var eventArgs = new ExtendedDragEventHandlerArgs (null, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _droppableElementData;

      var wasRaised = false;
      _dragAndDropAddIn.DragDrop += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragDrop_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);

      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _droppableElementData;

      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);

      var wasRaised = false;
      _dragAndDropAddIn.DragDrop += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragDrop_RequiredAttributesNotFound_ShouldNotBeCalled ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);

      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _nonDroppableElementData;

      var wasRaised = false;
      _dragAndDropAddIn.DragDrop += (source, name, arguments) => wasRaised = true;


      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.False);
    }

    [Test]
    public void DragDrop_FilesAttachedButUnregistered_ShouldIgnore ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);

      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _droppableElementData;

      var wasRaised = false;
      _dragAndDropAddIn.DragDrop += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.Empty);

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragEnter_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();

      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropAddIn.DragEnter += (source, name, arguments) =>
                                     {
                                       Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                       Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                       Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                       wasRaised = true;
                                     };

      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragOver_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      _dragAndDropAddIn.DragOver += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragOver += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }


    [Test]
    public void DragLeave_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      _dragAndDropAddIn.DragLeave += (source, name, arguments) =>
                                     {
                                       Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                       Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                       Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                       wasRaised = true;
                                     };

      browserStub.Raise (_ => _.DragLeave += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragLeave_FilesRemovedIfNotDropped_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      ScriptEvent onLeave = (source, name, arguments) =>
                            {
                              Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                              Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                              Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                              wasRaised = true;
                            };

      ScriptEvent onSubsequentLeave = (source, name, arguments) =>
                                      {
                                        Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                        Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                        Assert.That (((DragDropEventData) arguments).Names, Is.Empty);

                                        wasRaised = true;
                                      };

      _dragAndDropAddIn.DragLeave += onLeave;

      browserStub.Raise (_ => _.DragLeave += null, browserStub, eventArgs);
      _dragAndDropAddIn.DragLeave -= onLeave;
      _dragAndDropAddIn.DragLeave += onSubsequentLeave;
      browserStub.Raise (_ => _.DragLeave += null, browserStub, eventArgs);

      Assert.That (wasRaised, Is.True);
    }

    private dynamic CreateCondition ()
    {
      dynamic fakeCondition = new ExpandoObject();

      fakeCondition.EventID = c_fakeEventID;
      fakeCondition.DocumentHandle = new HtmlDocumentHandle (Guid.NewGuid()).ToString();
      fakeCondition.Criteria = new object();

      return fakeCondition;
    }
  }
}