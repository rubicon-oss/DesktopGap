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
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Events
{
  public class EventManager : IEventDispatcher, IEventHost, IPartImportsSatisfiedNotification
  {
    private readonly HtmlDocumentHandle _document;
    private const string c_moduleEventSeperator = ".";

    private readonly Dictionary<string, IList<KeyValuePair<string, Condition>>> _clients =
        new Dictionary<string, IList<KeyValuePair<string, Condition>>>();

    private IList<IEventAddIn> _sharedEvents;
    private IList<IEventAddIn> _nonSharedEvents;
    private IList<IEventAddIn> _sharedAddedEvents;


    public event EventHandler<ScriptEventArgs> EventFired;

    public EventManager (HtmlDocumentHandle document)
    {
      _document = document;

      _sharedAddedEvents = new List<IEventAddIn>();
    }

    public EventManager (HtmlDocumentHandle document, IList<IEventAddIn> sharedExternalEvents)
        : this (document)
    {
      ArgumentUtility.CheckNotNull ("sharedExternalEvents", sharedExternalEvents);

      _sharedAddedEvents = sharedExternalEvents;
    }

    public void Dispose ()
    {
      _sharedAddedEvents = null;

      foreach (var nonSharedEvent in _nonSharedEvents)
      {
        nonSharedEvent.OnBeforeUnload (_document);
        nonSharedEvent.Dispose();
      }

      foreach (var sharedEvent in _sharedEvents)
      {
        sharedEvent.OnBeforeUnload (_document);
        sharedEvent.UnregisterEvents (this);
      }
      EventFired = null;
    }

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.NonShared)]
    public IEnumerable<ExternalEventBase> NonSharedEvents
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _nonSharedEvents = value.ToList<IEventAddIn>();
      }
    }

    [ImportMany (typeof (ExternalEventBase), RequiredCreationPolicy = CreationPolicy.Shared)]
    public IEnumerable<ExternalEventBase> SharedEvents
    {
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);

        _sharedEvents = value.ToList<IEventAddIn>();
      }
    }

    public void Register (string eventName, string callbackName, string moduleName, Condition argument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNullOrEmpty ("callbackName", callbackName);

      var subscriptions = GetSubscriptions (eventName, moduleName);

      subscriptions.Add (new KeyValuePair<string, Condition> (callbackName, argument));
    }

    public void Unregister (string eventName, string callbackName, string moduleName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNullOrEmpty ("callbackName", callbackName);


      var subscriptions = GetSubscriptions (eventName, moduleName);
      foreach (var registration in subscriptions.Where (s => s.Key == callbackName))
        subscriptions.Remove (registration);
    }

    public bool HasEvent (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      IList<KeyValuePair<string, Condition>> e;

      return _clients.TryGetValue (name, out e);
    }

    public void RegisterEvent (IEventAddIn externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      ArgumentUtility.CheckNotNull ("externalEvent", externalEvent);
      ArgumentUtility.CheckNotNull ("scriptEvent", scriptEvent);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);

      scriptEvent += FireEvent;

      var name = string.Join (c_moduleEventSeperator, externalEvent.Name, eventName);

      InitializeClients (name);
    }

    public void UnregisterEvent (IEventAddIn externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      ArgumentUtility.CheckNotNull ("externalEvent", externalEvent);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("scriptEvent", scriptEvent);

      var name = string.Join (c_moduleEventSeperator, externalEvent.Name, eventName);

      var eventClients = GetClients (name);

      if (eventClients == null)
        throw new InvalidOperationException (string.Format ("Event {0} not found.", scriptEvent.Method.Name));
      
      _clients.Remove (name);

      scriptEvent -= FireEvent;
      eventClients.Clear();
    }

    private void LoadEvents (IEnumerable<IEventAddIn> events)
    {
      foreach (var evt in events)
      {
        evt.OnBeforeLoad (_document);
        evt.RegisterEvents (this);
      }
    }

    private IList<KeyValuePair<string, Condition>> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = string.Join (c_moduleEventSeperator, moduleName, eventName);

      IList<KeyValuePair<string, Condition>> registrations;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new InvalidOperationException (string.Format ("Event {0} in module {1} not found.", eventName, moduleName));

      return registrations;
    }

    private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw new InvalidOperationException (string.Format ("Event {0} already registered.", name));

      _clients[name] = new List<KeyValuePair<string, Condition>>();
    }


    private IList<KeyValuePair<string, Condition>> GetClients (string eventName)
    {
      IList<KeyValuePair<string, Condition>> eventClients;
      _clients.TryGetValue (eventName, out eventClients);
      return eventClients;
    }

    private void FireEvent (IEventAddIn source, string eventName, JsonData args)
    {
      var name = string.Join (c_moduleEventSeperator, source.Name, eventName);

      IList<KeyValuePair<string, Condition>> callbackNames;
      if (!_clients.TryGetValue (name, out callbackNames))
        throw new InvalidOperationException (string.Format ("Event {0} in module {1} never registered.", eventName, source.Name));

      foreach (var callback in callbackNames.Where (c => source.CheckRaiseCondition (c.Value)))
      {
        args.EventID = callback.Value.EventID;
        var eventArgs = new ScriptEventArgs { ScriptArgs = args, Function = callback.Key };
        EventFired (this, eventArgs);
      }
    }

    public void OnImportsSatisfied ()
    {
      foreach (var evnt in _sharedAddedEvents)
        _sharedEvents.Add (evnt);
      LoadEvents (_sharedEvents);
    }
  }
}