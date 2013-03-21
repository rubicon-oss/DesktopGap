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
using System.Reflection;

namespace DesktopGap.AddIns.Events
{
  internal class EventManager : IEventManager
  {
    private const string c_moduleEventSeperator = ".";

    private readonly IDictionary<string, IList<string>> _clients = new Dictionary<string, IList<string>>();

    private IEnumerable<IExternalEvent> _events;

    public event EventHandler<ScriptEventArgs> EventFired;

    [ImportMany (typeof (IExternalEvent), RequiredCreationPolicy = CreationPolicy.Any)]
    public IEnumerable<IExternalEvent> Events
    {
      get { return _events; }
      set
      {
        _events = value;
        ReloadEvents();
      }
    }


    public EventManager ()
    {
      Events = new List<IExternalEvent>();
    }


    public void Register (string eventName, string callbackName, string moduleName)
    {
      IList<string> subscriptions = GetSubscriptions (eventName, moduleName);
      subscriptions.Add (callbackName);
    }

    public void Unregister (string eventName, string callbackName, string moduleName)
    {
      IList<string> subscriptions = GetSubscriptions (eventName, moduleName);
      subscriptions.Remove (callbackName);
    }

    public void RegisterEvent (IExternalEvent externalEvent, ref ScriptEvent scriptEvent)
    {
      scriptEvent += FireEvent;

      var name = externalEvent.Name + c_moduleEventSeperator + scriptEvent.GetMethodInfo().Name;
      InitializeClients (name);
    }

    public void UnregisterEvent (IExternalEvent externalEvent, ref ScriptEvent scriptEvent)
    {
      if (scriptEvent == null)
        throw new ArgumentNullException ("scriptEvent");
      var name = externalEvent.Name + c_moduleEventSeperator + scriptEvent.Method.Name;

      var eventClients = GetClients (name);

      if (eventClients == null)
        throw new InvalidOperationException (String.Format ("Event {0} not found", scriptEvent.Method.Name));


      _clients.Remove (name);

      scriptEvent -= FireEvent;
      eventClients.Clear();
    }

    private IList<string> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = moduleName + c_moduleEventSeperator + eventName;

      IList<string> registrations = null;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new InvalidOperationException (String.Format ("Event {0} in module {1} not found", eventName, moduleName));

      return registrations;
    }

    private void ReloadEvents ()
    {
      _clients.Clear();
      foreach (var evt in Events)
      {
        evt.OnBeforeLoad();
        evt.RegisterEvents (this);
      }
    }

    private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw new InvalidOperationException (String.Format ("Event {0} already registered", name));

      _clients[name] = new List<string>();
    }


    private IList<string> GetClients (string eventName)
    {
      IList<string> eventClients = null;
      _clients.TryGetValue (eventName, out eventClients);
      return eventClients;
    }

    private void FireEvent (EventInfo sender, ScriptEventArgs args)
    {
      var name = sender.ReflectedType + c_moduleEventSeperator + sender.Name;
      IList<string> callbackNames;
      if (!_clients.TryGetValue (name, out callbackNames))
        throw new InvalidOperationException (String.Format ("Event {0} not registered", name));

      foreach (var callbackName in callbackNames)
      {
        EventFired (this, args);
      }
    }
  }
}