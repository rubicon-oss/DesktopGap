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
using Remotion.Dms.Shared.Utilities;

namespace DesktopGap.AddIns.Services
{
  internal class ServiceManager : IServiceManager //TODO add something for built-in services
  {
    [ImportMany (typeof (IExternalService), RequiredCreationPolicy = CreationPolicy.Any)]
    public IEnumerable<IExternalService> Services
    {
      get { return _services; }
      set
      {
        _services = value;
        _servicesDict.Clear();
        foreach (var service in _services)
        {
          RegisterService (service);
        }
      }
    }

    private readonly IDictionary<string, IExternalService> _servicesDict =
        new Dictionary<string, IExternalService>();

    private IEnumerable<IExternalService> _services;

  public ServiceManager ()
    {
      Services = new List<IExternalService>();
    }


    public void RegisterService (IExternalService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);


      if (TryGetService (service) != null)
        throw new InvalidOperationException (String.Format ("Service {0} already registered", service.Name));

      _servicesDict[service.Name] = service;
    }

    public void UnregisterService (IExternalService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);


      if (TryGetService (service) == null)
        throw new InvalidOperationException (String.Format ("Service {0} not registered", service.Name));

      _servicesDict.Remove (service.Name);
    }

    public IExternalService GetService (string serviceName)
    {
      IExternalService service;
      if (!_servicesDict.TryGetValue (serviceName, out service))
        throw new InvalidOperationException (String.Format ("Service {0} not found", serviceName));
      return service;
    }

    private IExternalService TryGetService (IExternalService service)
    {
      IExternalService s;
      _servicesDict.TryGetValue (service.Name, out s);
      return s;
    }
  }
}