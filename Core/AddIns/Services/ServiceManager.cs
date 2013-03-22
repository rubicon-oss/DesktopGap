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
    private readonly IDictionary<string, IExternalService> _services = new Dictionary<string, IExternalService>();

    public ServiceManager ()
    {
    }

    public void Dispose ()
    {
    }

    [ImportMany (typeof (IExternalService), RequiredCreationPolicy = CreationPolicy.Any)]
    public IEnumerable<IExternalService> Services
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _services.Clear();
        foreach (var service in value)
          RegisterService (service);
      }
    }

    public void RegisterService (IExternalService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      GetService (service.Name, name => new InvalidOperationException (string.Format ("Service '{0}' already registered.", name)));
      _services[service.Name] = service;
    }

    public void UnregisterService (IExternalService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      GetService (service.Name, name => new InvalidOperationException (string.Format ("Service '{0}' not registered.", name)));
      _services.Remove (service.Name);
    }

    public IExternalService GetService (string serviceName)
    {
      return GetService (serviceName, name => new InvalidOperationException (string.Format ("Service '{0}' not found.", name)));
    }

    public bool HasService (string name)
    {
      IExternalService s;
      return _services.TryGetValue (name, out s);
    }

    private IExternalService GetService<TException> (string serviceName, Func<string, TException> createServiceNotFoundException)
        where TException : Exception
    {
      IExternalService service;
      if (!_services.TryGetValue (serviceName, out service))
        throw createServiceNotFoundException (serviceName);
      return service;
    }
  }
}