﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns.Events
{
  public class EventManager : IEventDispatcher, IEventHost
  {
    private readonly DocumentHandle _document;
    private const string c_moduleEventSeperator = ".";

    private readonly Dictionary<string, IList<KeyValuePair<string, Condition>>> _clients =
        new Dictionary<string, IList<KeyValuePair<string, Condition>>>();

    private IList<ExternalEventBase> _eventsShared;
    private IList<ExternalEventBase> _eventsNonShared;


    public event EventHandler<ScriptEventArgs> EventFired;

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalEventBase> NonSharedEvents
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _eventsNonShared = value.ToArray();
      }
    }

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalEventBase> SharedEvents
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _eventsShared = value.ToArray();
      }
    }


    public EventManager (DocumentHandle document, IList<ExternalEventBase> sharedExternalEvents, IList<ExternalEventBase> nonSharedExternalEvents)
    {
      _document = document;
      ArgumentUtility.CheckNotNull ("nonSharedExternalEvents", nonSharedExternalEvents);
      ArgumentUtility.CheckNotNull ("sharedExternalEvents", sharedExternalEvents);
      ArgumentUtility.CheckNotNull ("document", document);


      foreach (var nonSharedExternalEvent in nonSharedExternalEvents)
        _eventsNonShared.Add (nonSharedExternalEvent);
      LoadEvents (_eventsNonShared);
      foreach (var sharedExternalEvent in sharedExternalEvents)
        _eventsShared.Add (sharedExternalEvent);
      LoadEvents (_eventsShared);
    }

    public void Dispose ()
    {
      foreach (var nonSharedEvent in _eventsNonShared)
      {
        nonSharedEvent.OnBeforeUnload (_document);
        nonSharedEvent.Dispose();
      }

      foreach (var sharedEvent in _eventsShared)
      {
        sharedEvent.OnBeforeUnload (_document);
        sharedEvent.UnregisterEvents (this);
      }
      EventFired = null;
    }


    public void Register (string eventName, string callbackName, string moduleName, Condition argument)
    {
      ArgumentUtility.CheckNotNull ("moduleName", moduleName);
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("callbackName", callbackName);

      var subscriptions = GetSubscriptions (eventName, moduleName);

      subscriptions.Add (new KeyValuePair<string, Condition> (callbackName, argument));
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

      IList<KeyValuePair<string, Condition>> e;

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

    private void LoadEvents (IEnumerable<ExternalEventBase> events)
    {
      foreach (var evt in events)
      {
        evt.OnBeforeLoad(_document);
        evt.RegisterEvents (this);
      }
    }

    private IList<KeyValuePair<string, Condition>> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = moduleName + c_moduleEventSeperator + eventName;

      IList<KeyValuePair<string, Condition>> registrations;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new InvalidOperationException (String.Format ("Event {0} in module {1} not found", eventName, moduleName));

      return registrations;
    }

    private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw new InvalidOperationException (String.Format ("Event {0} already registered", name));

      _clients[name] = new List<KeyValuePair<string, Condition>>();
    }


    private IList<KeyValuePair<string, Condition>> GetClients (string eventName)
    {
      IList<KeyValuePair<string, Condition>> eventClients;
      _clients.TryGetValue (eventName, out eventClients);
      return eventClients;
    }

    private void FireEvent (ExternalEventBase source, string eventName, JsonData args)
    {
      var name = source.Name + c_moduleEventSeperator + eventName;
      IList<KeyValuePair<string, Condition>> callbackNames;
      if (!_clients.TryGetValue (name, out callbackNames))
        throw new InvalidOperationException (String.Format ("Event {0} never registered", name));

      foreach (var callback in callbackNames.Where (c => source.CheckRaiseCondition (c.Value)))
      {
        args.EventId = callback.Value.EventID;
        var eventArgs = new ScriptEventArgs { ScriptArgs = args, Function = callback.Key };
        EventFired (this, eventArgs);
      }
    }
  }
}