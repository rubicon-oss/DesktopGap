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
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events.Arguments;
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

    private DragAndDropEvent _dragAndDropEvent;


    [SetUp]
    public void SetUp ()
    {
      _dragAndDropEvent = new DragAndDropEvent (new ResourceManager());

      var attributesDictionary = new Dictionary<string, string>
                                 {
                                     { DragAndDropEvent.DropConditionAttributeName, "1" },
                                     { DragAndDropEvent.DroppableAttributeName, "true" }
                                 };


      _droppableElementData = new HtmlElementData ("unique-id", attributesDictionary);
      _nonDroppableElementData = new HtmlElementData ("unique-id2", new Dictionary<string, string>());
    }

    [Test]
    public void RemoveDragDropSource_RemoveSource_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropEvent.AddDragDropSource (browserStub);

      Assert.That (() => _dragAndDropEvent.RemoveDragDropSource (browserStub), Throws.Nothing);
    }

    [Test]
    public void ShouldRaiseEvent_InvalidCondition_ShouldThrowArgumentException ()
    {
      Assert.That (
          () => _dragAndDropEvent.CheckRaiseCondition (new Condition (CreateCondition())),
          Throws.ArgumentException.With.Message.EqualTo ("The provided 'Criteria' object does not have the required property 'elementID'."));
    }

    [Test]
    public void DragDrop_NoFilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropEvent.AddDragDropSource (browserStub);

      var eventArgs = new ExtendedDragEventHandlerArgs (null, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _droppableElementData;

      var wasRaised = false;
      _dragAndDropEvent.DragDrop += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropEvent>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.Empty);
                                      Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.Empty);
                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragDrop_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropEvent.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);

      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _droppableElementData;

      var wasRaised = false;
      _dragAndDropEvent.DragDrop += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropEvent>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));
                                      Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.Not.Empty);

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragDrop_RequiredAttributesNotFound_ShouldNotBeCalled ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropEvent.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);

      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);
      eventArgs.Current = _nonDroppableElementData;

      var wasRaised = false;
      _dragAndDropEvent.DragDrop += (source, name, arguments) => wasRaised = true;


      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.False);
    }

    [Test]
    public void DragEnter_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropEvent.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();

      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropEvent.DragEnter += (source, name, arguments) =>
                                     {
                                       Assert.That (source, Is.InstanceOf<DragAndDropEvent>());
                                       Assert.That (arguments, Is.InstanceOf<DragEventData>());
                                       Assert.That (((DragEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                       wasRaised = true;
                                     };

      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragOver_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropEvent.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropEvent.DragOver += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropEvent>());
                                      Assert.That (arguments, Is.InstanceOf<DragEventData>());
                                      Assert.That (((DragEventData) arguments).Names, Is.EquivalentTo (_filePaths));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragOver += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }


    [Test]
    public void DragLeave_FilesAttached_ShouldSucceed ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();
      _dragAndDropEvent.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();
      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropEvent.DragLeave += (source, name, arguments) =>
                                     {
                                       Assert.That (source, Is.InstanceOf<DragAndDropEvent>());
                                       Assert.That (arguments, Is.InstanceOf<DragEventData>());

                                       wasRaised = true;
                                     };

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