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

    private readonly Dictionary<string, IList<KeyValuePair<string, IEventArgument>>> _clients =
        new Dictionary<string, IList<KeyValuePair<string, IEventArgument>>>();

    private IEnumerable<IExternalEvent> _eventsShared;
    private IEnumerable<IExternalEvent> _eventsNonShared;


    public event EventHandler<ScriptEventArgs> EventFired;

    [ImportMany (typeof (IExternalEvent), RequiredCreationPolicy = CreationPolicy.Any)]
    public IEnumerable<IExternalEvent> NonSharedEvents
    {
      get { return _eventsNonShared; }
      set { _eventsNonShared = value; }
    }

    //[ImportMany (typeof (IExternalEvent), RequiredCreationPolicy = CreationPolicy.Shared)]
    //public IEnumerable<IExternalEvent> SharedEvents
    //{
    //  get { return _eventsShared; }
    //  set { _eventsShared = value; }
    //}


    public EventManager ()
    {
      NonSharedEvents = new List<IExternalEvent>();
    }

    public void Dispose ()
    {
      foreach (var nonSharedEvent in NonSharedEvents)
      {
        nonSharedEvent.OnBeforeUnload();
        nonSharedEvent.Dispose();
      }
    }


    public void Register (string eventName, string callbackName, string moduleName, IEventArgument argument)
    {
      ArgumentUtility.CheckNotNull ("moduleName", moduleName);
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("callbackName", callbackName);

      var subscriptions = GetSubscriptions (eventName, moduleName);

      subscriptions.Add (new KeyValuePair<string, IEventArgument> (callbackName, argument));
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

      IExternalEvent e;
      return true;
    }

    public void RegisterEvent (IExternalEvent externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      scriptEvent += FireEvent;

      var name = externalEvent.Name + c_moduleEventSeperator + eventName;

      InitializeClients (name);
    }

    public void UnregisterEvent (IExternalEvent externalEvent, ref ScriptEvent scriptEvent, string eventName)
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

    private IList<KeyValuePair<string, IEventArgument>> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = moduleName + c_moduleEventSeperator + eventName;

      IList<KeyValuePair<string, IEventArgument>> registrations;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new InvalidOperationException (String.Format ("Event {0} in module {1} not found", eventName, moduleName));

      return registrations;
    }

    private void ReloadEvents ()
    {
      _clients.Clear();
      foreach (var evt in NonSharedEvents)
      {
        evt.OnBeforeLoad();
        evt.RegisterEvents (this);
      }
    }

    private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw new InvalidOperationException (String.Format ("Event {0} already registered", name));

      _clients[name] = new List<KeyValuePair<string, IEventArgument>>();
    }


    private IList<KeyValuePair<string, IEventArgument>> GetClients (string eventName)
    {
      IList<KeyValuePair<string, IEventArgument>> eventClients = null;
      _clients.TryGetValue (eventName, out eventClients);
      return eventClients;
    }

    private void FireEvent (IExternalEvent source, string eventName, ScriptEventArgs args)
    {
      var name = source.Name + c_moduleEventSeperator + eventName;
      IList<KeyValuePair<string, IEventArgument>> callbackNames;
      if (!_clients.TryGetValue (name, out callbackNames))
        throw new InvalidOperationException (String.Format ("Event {0} never registered", name));

      foreach (var callbackName in callbackNames)
      {
        if (source.CheckArgument (callbackName.Value))
          EventFired (this, args);
      }
    }
  }
}