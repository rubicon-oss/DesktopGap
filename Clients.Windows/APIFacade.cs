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
using System.Runtime.InteropServices;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows
{
  [ComVisible (true)]
  public class ApiFacade : IDisposable
  {
    public event EventHandler<ScriptEventArgs> EventDispatched;

    public IServiceManager ServiceManager { get; private set; }
    public IEventDispatcher EventManager { get; private set; }

    public ApiFacade (IServiceManager serviceManager, IEventDispatcher eventManager)
    {
      ArgumentUtility.CheckNotNull ("eventManager", eventManager);
      ArgumentUtility.CheckNotNull ("serviceManager", serviceManager);


      ServiceManager = serviceManager;
      EventManager = eventManager;
      EventManager.EventFired += OnEventFired;
    }

    public void Dispose ()
    {
      ServiceManager.Dispose();
      EventManager.Dispose();
    }

    //
    // Services
    //

    public object GetService (string serviceName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serviceName", serviceName);

      return ServiceManager.GetService (serviceName);
    }

    public bool HasService (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return ServiceManager.HasService (name);
    }

    //
    // Events
    //

    public void AddEventListener (string eventName, string callbackName, string moduleName, dynamic argument)
    {
      EventArgument eventArgument = null;

      if (argument != null)
      {
        try // TODO find a better solution
        {
          eventArgument = new EventArgument (argument);
        }
        catch (Exception ex)
        {
          throw new Exception ("argument is the wrong class"); // TODO use proper exception class
        }


        //var x = new ConvertBinder (IEventArgument, true);

        //((DynamicObject) argument).TryConvert(ConvertBinder.)
        //  // eventArgument = argument as IEventArgument;
        //if (eventArgument == null)
      }
      EventManager.Register (eventName, callbackName, moduleName, eventArgument);
    }

    public void RemoveEventListener (string eventName, string callbackName, string moduleName)
    {
      EventManager.Unregister (eventName, callbackName, moduleName);
    }

    public bool HasEvent (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return EventManager.HasEvent (name);
    }

    private void OnEventFired (object sender, ScriptEventArgs args)
    {
      if (EventDispatched != null)
        EventDispatched (sender, args);
    }
  }
}