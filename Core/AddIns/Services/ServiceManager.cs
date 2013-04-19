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
using System.Linq;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Services
{
  public class ServiceManager : IServiceManager, IPartImportsSatisfiedNotification
  {
    private readonly IList<IServiceAddIn> _sharedAddedServices = new List<IServiceAddIn>();
    private readonly HtmlDocumentHandle _document;

    private readonly IDictionary<string, IServiceAddIn> _services =
        new Dictionary<string, IServiceAddIn>();

    private IList<IServiceAddIn> _nonSharedServices;
    private IList<IServiceAddIn> _sharedServices;

    public ServiceManager (HtmlDocumentHandle document)
    {
      ArgumentUtility.CheckNotNull ("document", document);
      _document = document;
    }

    public ServiceManager (HtmlDocumentHandle document, IList<IServiceAddIn> sharedAddedService)
        : this (document)
    {
      ArgumentUtility.CheckNotNull ("sharedAddedService", sharedAddedService);

      _sharedAddedServices = sharedAddedService;
    }

    public void Dispose ()
    {
      foreach (var service in _services)
        service.Value.OnBeforeUnload (_document);

      foreach (var service in _nonSharedServices)
      {
        service.OnBeforeUnload (_document);
        service.Dispose();
      }
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalServiceBase> Services
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _nonSharedServices = value.ToArray();
      }
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalServiceBase> SharedServices
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _sharedServices = value.ToArray();
      }
    }


    public IServiceAddIn GetService (string serviceName)
    {
      return GetService (serviceName, name => new InvalidOperationException (string.Format ("Service '{0}' not found.", name)));
    }

    public bool HasService (string name)
    {
      IServiceAddIn s;
      return _services.TryGetValue (name, out s);
    }


    public void OnImportsSatisfied ()
    {
      RegisterServices (_sharedAddedServices);
      RegisterServices (_sharedServices);
      RegisterServices (_nonSharedServices);
    }

    //
    // OTHER
    // 

    private void RegisterService (IServiceAddIn service)
    {
      if (HasService (service.Name))
        throw new InvalidOperationException (string.Format ("Service '{0}' already registered.", service.Name));
      _services[service.Name] = service;
    }

    private void RegisterServices (IEnumerable<IServiceAddIn> services)
    {
      foreach (var service in services)
        RegisterService (service);
    }

    private IServiceAddIn GetService<TException> (string serviceName, Func<string, TException> createServiceNotFoundException)
        where TException : Exception
    {
      IServiceAddIn service;
      if (!_services.TryGetValue (serviceName, out service))
        throw createServiceNotFoundException (serviceName);
      return service;
    }
  }
}