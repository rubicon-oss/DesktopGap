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
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Services
{
  public class ServiceManager : IServiceManager //TODO add something for built-in services
  {
    private readonly IDictionary<string, ExternalServiceBase> _services =
        new Dictionary<string, ExternalServiceBase>();

    private IEnumerable<ExternalServiceBase> _nonSharedServices; 

    public ServiceManager ()
    {
    }

    public void Dispose ()
    {

      foreach(var service in _nonSharedServices)
      {
        service.OnBeforeUnload();
        service.Dispose();
      }
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalServiceBase> Services
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        RegisterServices (value);
        _nonSharedServices = value;
      }
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalServiceBase> SharedServices
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        RegisterServices (value);
      }
    }


    public ExternalServiceBase GetService (string serviceName)
    {
      return GetService (serviceName, name => new InvalidOperationException (string.Format ("Service '{0}' not found.", name)));
    }

    public bool HasService (string name)
    {
      ExternalServiceBase s;
      return _services.TryGetValue (name, out s);
    }

    private void RegisterService (ExternalServiceBase service)
    {
      if (HasService (service.Name))
        throw new InvalidOperationException (string.Format ("Service '{0}' already registered.", service.Name));
      _services[service.Name] = service;
    }

    public void UnregisterService (ExternalServiceBase service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      GetService (service.Name, name => new InvalidOperationException (string.Format ("Service '{0}' not registered.", name)));
      _services.Remove (service.Name);
    }


    private void RegisterServices (IEnumerable<ExternalServiceBase> services)
    {
      foreach(var service in services)
        RegisterService (service);
    }

    private ExternalServiceBase GetService<TException> (string serviceName, Func<string, TException> createServiceNotFoundException)
        where TException : Exception
    {
      ExternalServiceBase service;
      if (!_services.TryGetValue (serviceName, out service))
        throw createServiceNotFoundException (serviceName);
      return service;
    }
  }
}