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

namespace DesktopGap.AddIns.Events
{
  public class EventManager : IEventDispatcher, IEventHost
  {
    private const string c_moduleEventSeperator = ".";

    private readonly Dictionary<string, IList<KeyValuePair<string, EventArgument>>> _clients =
        new Dictionary<string, IList<KeyValuePair<string, EventArgument>>>();

    private IEnumerable<ExternalEventBase> _eventsShared;
    private IEnumerable<ExternalEventBase> _eventsNonShared;


    public event EventHandler<ScriptEventArgs> EventFired;

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalEventBase> NonSharedEvents
    {
      get { return _eventsNonShared; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _eventsNonShared = value;

        foreach (var evt in value)
        {
          evt.OnBeforeLoad();
          evt.RegisterEvents (this);
        }
      }
    }

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalEventBase> SharedEvents
    {
      get { return _eventsShared; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _eventsShared = value;
        foreach (var evt in value)
        {
          evt.RegisterEvents (this);
        }
      }
    }


    public EventManager ()
    {
      NonSharedEvents = new List<ExternalEventBase>();
    }

    public void Dispose ()
    {
      foreach (var nonSharedEvent in NonSharedEvents)
      {
        nonSharedEvent.OnBeforeUnload();
        nonSharedEvent.Dispose();
      }

      foreach (var sharedEvent in SharedEvents)
      {
        sharedEvent.UnregisterEvents (this);
      }
      EventFired = null;
    }


    public void Register (string eventName, string callbackName, string moduleName, EventArgument argument)
    {
      ArgumentUtility.CheckNotNull ("moduleName", moduleName);
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("callbackName", callbackName);

      var subscriptions = GetSubscriptions (eventName, moduleName);

      subscriptions.Add (new KeyValuePair<string, EventArgument> (callbackName, argument));
    }

    public void Unregister (string eventName, string callbackName, string moduleName)
    {
      ArgumentUtility.CheckNotNull ("moduleName", moduleName);
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("callbackName", callbackName);


      var subscriptions = GetSubscriptions (eventName, moduleName);
      foreach (var registration in subscriptions.Where (s => s.Key == callbackName))
      {
        subscriptions.Remove (registration);
      }
    }

    public bool HasEvent (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      IList<KeyValuePair<string, EventArgument>> e;

      return _clients.TryGetValue (name, out e);
    }

    public void RegisterEvent (ExternalEventBase externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      scriptEvent += FireEvent;

      var name = externalEvent.Name + c_moduleEventSeperator + eventName;

      InitializeClients (name);
    }

    public void UnregisterEvent (ExternalEventBase externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      if (scriptEvent == null)
        throw new ArgumentNullException ("scriptEvent");
      var name = externalEvent.Name + c_moduleEventSeperator + eventName;

      var eventClients = GetClients (name);

      if (eventClients == null)
        throw new InvalidOperationException (String.Format ("Event {0} not found", scriptEvent.Method.Name));


      _clients.Remove (name);

      scriptEvent -= FireEvent;
      eventClients.Clear();
    }

    private IList<KeyValuePair<string, EventArgument>> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = moduleName + c_moduleEventSeperator + eventName;

      IList<KeyValuePair<string, EventArgument>> registrations;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new InvalidOperationException (String.Format ("Event {0} in module {1} not found", eventName, moduleName));

      return registrations;
    }

  private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw new InvalidOperationException (String.Format ("Event {0} already registered", name));

      _clients[name] = new List<KeyValuePair<string, EventArgument>>();
    }


    private IList<KeyValuePair<string, EventArgument>> GetClients (string eventName)
    {
      IList<KeyValuePair<string, EventArgument>> eventClients = null;
      _clients.TryGetValue (eventName, out eventClients);
      return eventClients;
    }

    private void FireEvent (ExternalEventBase source, string eventName, JsonData args)
    {
      var name = source.Name + c_moduleEventSeperator + eventName;
      IList<KeyValuePair<string, EventArgument>> callbackNames;
      if (!_clients.TryGetValue (name, out callbackNames))
        throw new InvalidOperationException (String.Format ("Event {0} never registered", name));

      foreach (var callback in callbackNames.Where (c => source.CheckArgument (c.Value)))
      {
        args.EventId = callback.Value.EventID;
        var eventArgs = new ScriptEventArgs() { ScriptArgs = args, Function = callback.Key };
        EventFired (this, eventArgs);
      }
    }
  }
}