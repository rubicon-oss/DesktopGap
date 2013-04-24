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
  public class HtmlDocumentHandleRegistryTest
  {
    private const string c_documentAlreadyRegisteredFormatString = "DocumentHandle '{0}' is already registered.";
    private const string c_documentNotRegisteredFormatString = "DocumentHandle '{0}' is not registered.";

    [Test]
    public void RegisterDocumentHandle_DocumentHandleAlreadyExists_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost());

      Assert.That (
          () => addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost()),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentAlreadyRegisteredFormatString, handle)));
    }

    [Test]
    public void RegisterDocumentHandle_AddMultipleDocuments_ShouldSucceed ()
    {
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      addInManager.RegisterDocumentHandle (CreateDocumentHandle(), new FakeScriptingHost());
      addInManager.RegisterDocumentHandle (CreateDocumentHandle(), new FakeScriptingHost());
      addInManager.RegisterDocumentHandle (CreateDocumentHandle(), new FakeScriptingHost());
      addInManager.RegisterDocumentHandle (CreateDocumentHandle(), new FakeScriptingHost());


      Assert.That (addInManager.EventDispatcherCount, Is.EqualTo (4));
    }

    [Test]
    public void UnregisterDocumentHandle_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      Assert.That (
          () => addInManager.UnregisterDocumentHandle (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void UnregisterDocumentHandle_RemoveDocumentHandle_ShouldSucceed ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      var count = addInManager.EventDispatcherCount;
      addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost());

      Assert.That (() => addInManager.UnregisterDocumentHandle (handle), Throws.Nothing);
      Assert.That (count, Is.EqualTo (addInManager.EventDispatcherCount));
    }

    [Test]
    public void GetEventDispatcher_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      Assert.That (
          () => addInManager.GetEventDispatcher (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void GetEventDispatcher_GetByDocumentHandle_ShouldSucceed ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost());

      Assert.That (() => addInManager.GetEventDispatcher (handle), Is.InstanceOf<FakeEventDispatcher>());
    }

    [Test]
    public void GetServiceManager_DocumentHandleDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      Assert.That (
          () => addInManager.GetServiceManager (handle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, handle)));
    }

    [Test]
    public void GetServiceManager_GetByDocumentHandle_ShouldSucceed ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost());

      Assert.That (() => addInManager.GetServiceManager (handle), Is.InstanceOf<FakeServiceManager>());
    }


    [Test]
    public void HasDocumentHandle_GetByDocumentHandle_ShouldSucceed ()
    {
      var handle = CreateDocumentHandle();
      var addInManager = new HtmlDocumentHandleRegistry (new FakeServiceManagerFactory(), new FakeEventDispatcherFactory());

      addInManager.RegisterDocumentHandle (handle, new FakeScriptingHost());

      Assert.That (() => addInManager.HasDocumentHandle (handle), Is.True);
    }

    private HtmlDocumentHandle CreateDocumentHandle ()
    {
      var guid = Guid.NewGuid();
      return new HtmlDocumentHandle (guid);
    }
  }
}