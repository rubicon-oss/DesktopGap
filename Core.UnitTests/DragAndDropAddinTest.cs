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
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
    private readonly string[] _filePaths = new[]
                                           {
                                               @"C:\Development\desktopgap",
                                               @"C:\Development\desktopgap\Core",
                                               @"C:\Development\desktopgap\license",
                                               @"C:\Development\desktopgap\Clients.Windows"
                                           };

    private readonly ResourceHandle[] _resourceHandles = new[]
                                                         {
                                                             new ResourceHandle (Guid.NewGuid()),
                                                             new ResourceHandle (Guid.NewGuid()),
                                                             new ResourceHandle (Guid.NewGuid()),
                                                             new ResourceHandle (Guid.NewGuid())
                                                         };

    private readonly HtmlDocumentHandle _documentHandle = new HtmlDocumentHandle (Guid.NewGuid());

    private DragAndDropAddIn _dragAndDropAddIn;


    [TestFixtureSetUp]
    public void Init ()
    {
      var resourceManager = MockRepository.GenerateStub<IResourceManager>();
      resourceManager.Stub (
          _ => _.AddResources (_filePaths.Select (p => new FileInfo (p) as FileSystemInfo).ToArray()))
          .Return (_resourceHandles);

      _dragAndDropAddIn =  new DragAndDropAddIn (resourceManager);
    }

    [Test]
    public void DragDrop ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropAddIn.AddDragDropSource (browserStub);
      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();

      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropAddIn.DragDrop += (source, name, arguments) =>
                                   {
                                     Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                     Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                     Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));
                                     Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.EquivalentTo (_resourceHandles));

                                     wasRaised = true;
                                   };

      browserStub.Raise (_ => _.DragDrop += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }


    [Test]
    public void DragEnter ()
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
                                      Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.EquivalentTo (_resourceHandles));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

    [Test]
    public void DragOver ()
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
                                      Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.EquivalentTo (_resourceHandles));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragEnter += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }


    [Test]
    public void DragLeave ()
    {
      var browserStub = MockRepository.GenerateStub<IExtendedWebBrowser>();

      _dragAndDropAddIn.AddDragDropSource (browserStub);

      var dataObjectStub = MockRepository.GenerateStub<IDataObject>();

      dataObjectStub.Stub (_ => _.GetData (DataFormats.FileDrop)).Return (_filePaths);
      var eventArgs = new ExtendedDragEventHandlerArgs (dataObjectStub, 0, 1, 1, DragDropEffects.All, DragDropEffects.None, _documentHandle);

      var wasRaised = false;
      _dragAndDropAddIn.DragLeave += (source, name, arguments) =>
                                    {
                                      Assert.That (source, Is.InstanceOf<DragAndDropAddIn>());
                                      Assert.That (arguments, Is.InstanceOf<DragDropEventData>());
                                      Assert.That (((DragDropEventData) arguments).Names, Is.EquivalentTo (_filePaths));
                                      Assert.That (((DragDropEventData) arguments).ResourceHandles, Is.EquivalentTo (_resourceHandles));

                                      wasRaised = true;
                                    };

      browserStub.Raise (_ => _.DragLeave += null, browserStub, eventArgs);
      Assert.That (wasRaised, Is.True);
    }

  }
}