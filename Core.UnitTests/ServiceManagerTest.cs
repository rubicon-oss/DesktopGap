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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using DesktopGap.AddIns.Services;
using DesktopGap.UnitTests.Fakes;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class ServiceManagerTest
  {
    private HtmlDocumentHandle? _consistentDocumentHandle;

    [Test]
    public void HasService_ServiceDoesNotExist_ShouldReturnFalse ()
    {
      var serviceManager = CreateServiceManager (new List<IServiceAddIn>());

      var serviceName = "some service";

      Assert.That (serviceManager.HasService (serviceName), Is.False);
    }

    [Test]
    public void GetService_ServiceDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var serviceManager = CreateServiceManager (new List<IServiceAddIn>());

      var serviceName = "some service";

      Assert.That (
          () => serviceManager.GetService (serviceName),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format ("Service '{0}' not found.", serviceName)));
    }

    [Test]
    public void Constructor_AddServiceDuplicate_ShouldThrowInvalidOperation ()
    {
      var serviceAddIn = new FakeServiceAddIn();

      var compositionException = Assert.Throws<CompositionException> (
          () => CreateServiceManager (new[] { serviceAddIn, serviceAddIn }));
      Assert.That (
          compositionException.RootCauses.First().InnerException, Is.InstanceOf<InvalidOperationException>());
    }

    [Test]
    public void Constructor_AddSharedExternalService_ShouldSucceed ()
    {
      var serviceAddIn = new FakeServiceAddIn();

      var serviceManager = CreateServiceManager (new[] { serviceAddIn });

      Assert.That (serviceManager, Is.Not.Null);
      Assert.That (serviceManager.HasService (serviceAddIn.Name), Is.True);
      Assert.That (serviceManager.GetService (serviceAddIn.Name), Is.SameAs (serviceAddIn));
    }

    private HtmlDocumentHandle GetDocumentHandle ()
    {
      if (_consistentDocumentHandle == null)
      {
        var guid = Guid.NewGuid();
        _consistentDocumentHandle = new HtmlDocumentHandle (guid);
      }
      return _consistentDocumentHandle.Value;
    }

    private ServiceManager CreateServiceManager (IList<IServiceAddIn> additionalEvents)
    {
      var handle = GetDocumentHandle();

      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add (new TypeCatalog());
      var compositionContainer = new CompositionContainer (catalog);

      var eventManager = new ServiceManager (handle, additionalEvents);
      compositionContainer.ComposeParts (eventManager);
      return eventManager;
    }
  }
}