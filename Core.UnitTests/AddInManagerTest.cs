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
using DesktopGap.AddIns;
using DesktopGap.UnitTests.Fakes;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class AddInManagerTest
  {
    private const string c_documentAlreadyRegisteredFormatString = "DocumentHandle {0} is already registered.";
    private const string c_documentNotRegisteredFormatString = "DocumentHandle {0} is not registered.";

    [Test]
    public void AddEventDispatcher_DocumentHandleAlreadyExists_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      addInManager.AddEventDispatcher (handle, new FakeEventDispatcher());

      Assert.That (
          () => addInManager.AddEventDispatcher (handle, new FakeEventDispatcher()),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentAlreadyRegisteredFormatString, handle)));
    }

    [Test]
    public void RemoveEventDispatcher_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      Assert.That (
          () => addInManager.RemoveEventDispatcher (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void GetEventDispatcher_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      Assert.That (
          () => addInManager.GetEventDispatcher (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void AddServiceManager_DocumentHandleAlreadyExists_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      addInManager.AddServiceManager (handle, new FakeServiceManager());

      Assert.That (
          () => addInManager.AddServiceManager (handle, new FakeServiceManager()),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentAlreadyRegisteredFormatString, handle)));
    }

    [Test]
    public void RemoveServiceManager_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      Assert.That (
          () => addInManager.RemoveServiceManager (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void GetServiceManager_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new AddInManager();

      Assert.That (
          () => addInManager.GetServiceManager (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    private HtmlDocumentHandle CreateDocumentHandle ()
    {
      var guid = Guid.NewGuid();
      return new HtmlDocumentHandle (guid);
    }
  }
}