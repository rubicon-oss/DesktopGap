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
using DesktopGap.AddIns.Services;

namespace DesktopGap.AddIns
{
  public class ServiceManager : IServiceManager, IPartImportsSatisfiedNotification //TODO add something for built-in services
  {
    [ImportMany (typeof (IExternalService))]
    private IEnumerable<Lazy<IExternalService>> _services;

    public IEnumerable<IExternalService> Services { get; private set; }

    private readonly IDictionary<string, IExternalService> _servicesDict = new Dictionary<string, IExternalService>();

    public ServiceManager ()
    {
      Services = new List<IExternalService>();
    }


    public IExternalService GetService (string serviceName)
    {
      IExternalService service = null;
      if (!_servicesDict.TryGetValue (serviceName, out service))
        throw new Exception ("Service not found"); // TODO make proper exception classes
      return service;
    }

    private void ReregisterEvents ()
    {
      _servicesDict.Clear();

      Services = from e in _services select e.Value;

      foreach (var service in Services)
      {
        var name = service.Name;
        IExternalService result = null;
        if (!_servicesDict.TryGetValue (name, out result))
          _servicesDict[name] = service;
        //else
        // throw new Exception("Duplicate event found") // TODO throw exception here or just fail silently?
      }
    }

    public void OnImportsSatisfied ()
    {
      ReregisterEvents();
    }
  }
}