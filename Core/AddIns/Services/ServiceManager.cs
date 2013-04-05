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
    private readonly IDictionary<string, KeyValuePair<ExternalServiceBase, bool>> _services = new Dictionary<string, KeyValuePair<ExternalServiceBase, bool>>();

    public ServiceManager ()
    {
    }

    public void Dispose ()
    {
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalServiceBase> NonSharedServices
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _services.Clear();
        foreach (var service in value)
          RegisterService (service, false);
      }
    }

    [ImportMany (typeof (ExternalServiceBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalServiceBase> SharedServices
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _services.Clear();
        foreach (var service in value)
          RegisterService (service, true);
      }
    }

    private void RegisterService (ExternalServiceBase service, bool singletonBehavior)
    {
      if(HasService (service.Name))
        throw new InvalidOperationException (string.Format ("Service '{0}' already registered.", service.Name));
      _services[service.Name] = new KeyValuePair<ExternalServiceBase, bool>(service, singletonBehavior);
    }

    public void UnregisterService (ExternalServiceBase service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      GetService (service.Name, name => new InvalidOperationException (string.Format ("Service '{0}' not registered.", name)));
      _services.Remove (service.Name);
    }

    public ExternalServiceBase GetService (string serviceName)
    {
      return GetService (serviceName, name => new InvalidOperationException (string.Format ("Service '{0}' not found.", name)));
    }

    public bool HasService (string name)
    {
      KeyValuePair<ExternalServiceBase, bool> s;
      return _services.TryGetValue (name, out s);
    }


    private ExternalServiceBase GetService<TException> (string serviceName, Func<string, TException> createServiceNotFoundException)
        where TException : Exception
    {
      KeyValuePair<ExternalServiceBase, bool> service;
      if (!_services.TryGetValue (serviceName, out service))
        throw createServiceNotFoundException (serviceName);
      return service.Value? service.Key: service.Key;
    }
  }
}