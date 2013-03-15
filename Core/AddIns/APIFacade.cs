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
using System.Linq;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns
{
  public class APIFacade : IAPIFacade
  {
    public IServiceManager ServiceManager { get; private set; }
    public IEventManager EventManager { get; private set; }

    public APIFacade (IServiceManager serviceManager, IEventManager eventManager)
    {
      if (serviceManager == null)
        throw new ArgumentNullException ("serviceManager");
      
      if (eventManager == null)
        throw new ArgumentNullException ("eventManager");

      ServiceManager = serviceManager;
      EventManager = eventManager;
    }

    public object GetService (string serviceName)
    {
      return ServiceManager.GetService (serviceName);
    }

    public string[] GetServiceNames ()
    {
      return (from s in ServiceManager.Services select s.Name).ToArray<string>();
    }

    public string[] GetEventNames ()
    {
      return (from e in EventManager.Events select e.Name).ToArray<string>();
    }

    public void addEventListener (string eventName, string callbackName, string moduleName)
    {
      EventManager.Register (eventName, callbackName, moduleName);
    }

    public void removeEventListener (string eventName, string callbackName, string moduleName)
    {
      EventManager.Unregister (eventName, callbackName, moduleName);
    }
  }
}