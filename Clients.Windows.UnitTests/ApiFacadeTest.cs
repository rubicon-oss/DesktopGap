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
using NUnit.Framework;

namespace Clients.Windows.UnitTests
{
  [TestFixture]
  public class ApiFacadeTest
  {
    //private const string c_documentAlreadyRegisteredFormatString = "DocumentHandle '{0}' is already registered.";
    //private const string c_documentNotRegisteredFormatString = "DocumentHandle '{0}' is not registered.";

    //private class FakeServiceManagerFactory : IServiceManagerFactory
    //{
    //  public IServiceManager CreateServiceManager (HtmlDocumentHandle document)
    //  {
    //    var stub = MockRepository.GenerateStub<IServiceManager>();
    //    return stub;
    //  }
    //}

    //private class FakeEventDispatcerFactory : IEventDispatcherFactory
    //{
    //  public IEventDispatcher CreateEventDispatcher (HtmlDocumentHandle document)
    //  {
    //    var stub = MockRepository.GenerateStub<IEventDispatcher>();
    //    return stub;
    //  }
    //}

    //private readonly IServiceManagerFactory _serviceManagerFactory = new FakeServiceManagerFactory();
    //private readonly IEventDispatcherFactory _eventDispatcherFactory = new FakeEventDispatcerFactory();

    //private HtmlDocumentHandle _consistentDocumentHandle;

    //[SetUp]
    //public void SetUp ()
    //{
    //  _consistentDocumentHandle = new HtmlDocumentHandle (Guid.NewGuid());
    //}

    //[Test]
    //public void RemoveDocument_RemoveExistingDocumentByInstance_ShouldSucceed ()
    //{
    //  var addInManager = new AddInManager();
    //  var apiFacade = new ApiFacade (_serviceManagerFactory, _eventDispatcherFactory, addInManager);

    //  var documentStub = MockRepository.GenerateStub<HtmlDocument>();
    //  documentStub.Stub (_ => _.InvokeScript (ApiFacade.GetDocumentIdentification)).Return (_consistentDocumentHandle.ToString());

    //  var serviceManagerCount = addInManager.ServiceManagerCount;
    //  var eventDispatcherCount = addInManager.EventDispatcherCount;

    //  apiFacade.AddDocument (documentStub);

    //  Assert.That (() => apiFacade.RemoveDocument (documentStub), Throws.Nothing);

    //  Assert.That (serviceManagerCount, Is.EqualTo (addInManager.ServiceManagerCount));
    //  Assert.That (eventDispatcherCount, Is.EqualTo (addInManager.EventDispatcherCount));
    //}

    //[Test]
    //public void RemoveDocument_RemoveExistingDocumentByHandle_ShouldSucceed ()
    //{
    //  var addInManager = new AddInManager();
    //  var apiFacade = new ApiFacade (_serviceManagerFactory, _eventDispatcherFactory, addInManager);

    //  var documentStub = MockRepository.GenerateStub<HtmlDocument>();
    //  documentStub.Stub (_ => _.InvokeScript (ApiFacade.GetDocumentIdentification)).Return (_consistentDocumentHandle.ToString());


    //  var serviceManagerCount = addInManager.ServiceManagerCount;
    //  var eventDispatcherCount = addInManager.EventDispatcherCount;

    //  apiFacade.AddDocument (documentStub);

    //  Assert.That (() => apiFacade.RemoveDocument (_consistentDocumentHandle), Throws.Nothing);

    //  Assert.That (serviceManagerCount, Is.EqualTo (addInManager.ServiceManagerCount));
    //  Assert.That (eventDispatcherCount, Is.EqualTo (addInManager.EventDispatcherCount));
    //}

    //[Test]
    //public void RemoveDocument_RemoveInexistentDocumentByInstance_ShouldSucceed ()
    //{
    //  var addInManager = new AddInManager();
    //  var apiFacade = new ApiFacade (_serviceManagerFactory, _eventDispatcherFactory, addInManager);

    //  var documentStub = MockRepository.GenerateStub<HtmlDocument>();
    //  documentStub.Stub (_ => _.InvokeScript (ApiFacade.GetDocumentIdentification)).Return (_consistentDocumentHandle.ToString());

    //  Assert.That (
    //      () => apiFacade.RemoveDocument (documentStub),
    //      Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, _consistentDocumentHandle)));
    //}

    //[Test]
    //public void RemoveDocument_RemoveInexistentDocumentByHandle_ShouldSucceed ()
    //{
    //  var addInManager = new AddInManager();
    //  var apiFacade = new ApiFacade (_serviceManagerFactory, _eventDispatcherFactory, addInManager);

    //  var documentStub = MockRepository.GenerateStub<HtmlDocument>();
    //  documentStub.Stub (_ => _.InvokeScript (ApiFacade.GetDocumentIdentification)).Return (_consistentDocumentHandle.ToString());

    //  Assert.That (
    //      () => apiFacade.RemoveDocument (_consistentDocumentHandle),
    //      Throws.InvalidOperationException.With.Message.EqualTo (string.Format (c_documentNotRegisteredFormatString, _consistentDocumentHandle)));
    //}


    //[Test]
    //public void AddDocument_AddADocument_ShouldSucceed ()
    //{
    //  var addInManager = new AddInManager();
    //  var apiFacade = new ApiFacade (_serviceManagerFactory, _eventDispatcherFactory, addInManager);

    //  var documentStub = MockRepository.GenerateStub<HtmlDocument>();
    //  documentStub.Stub (_ => _.InvokeScript (ApiFacade.GetDocumentIdentification)).Return (_consistentDocumentHandle.ToString());

    //  var serviceManagerCount = addInManager.ServiceManagerCount;
    //  var eventDispatcherCount = addInManager.EventDispatcherCount;

    //  Assert.That (() => apiFacade.AddDocument (documentStub), Throws.Nothing);

    //  Assert.That (serviceManagerCount, Is.EqualTo (addInManager.ServiceManagerCount));
    //  Assert.That (eventDispatcherCount, Is.EqualTo (addInManager.EventDispatcherCount));
    //}
  }
}