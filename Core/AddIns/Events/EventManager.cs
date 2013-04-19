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
using System.Linq;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Events
{
  public class EventManager : AddInManagerBase<IEventAddIn>, IEventDispatcher, IEventHost
  {
    private const string c_moduleEventSeperator = ".";

    private readonly Dictionary<string, IList<KeyValuePair<string, Condition>>> _clients =
        new Dictionary<string, IList<KeyValuePair<string, Condition>>>();

    public event EventHandler<ScriptEventArgs> EventFired;

    public EventManager ()
    {
    }

    public EventManager (IList<IEventAddIn> sharedAddIns, IList<IEventAddIn> nonSharedAddIns, HtmlDocumentHandle documentHandle)
        : base (sharedAddIns, nonSharedAddIns, documentHandle)
    {
      NonSharedAddInLoaded += (s, a) => a.AddIn.RegisterEvents (this);
      SharedAddInLoaded += (s, a) => a.AddIn.RegisterEvents (this);

      NonSharedAddInUnloaded += (s, a) => a.AddIn.UnregisterEvents (this);
      SharedAddInUnloaded += (s, a) => a.AddIn.UnregisterEvents (this);
    }

    protected override void Dispose (bool disposing)
    {
      EventFired = null;
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

    public bool HasEvent (string moduleName, string eventName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);

      var name = string.Join (c_moduleEventSeperator, moduleName, eventName);

      IList<KeyValuePair<string, Condition>> e;

      return _clients.TryGetValue (name, out e);
    }

    public void RegisterEvent (IEventAddIn externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      ArgumentUtility.CheckNotNull ("externalEvent", externalEvent);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);

      scriptEvent += FireEvent;

      var name = string.Join (c_moduleEventSeperator, externalEvent.Name, eventName);

      InitializeClients (name);
    }

    public void UnregisterEvent (IEventAddIn externalEvent, ref ScriptEvent scriptEvent, string eventName)
    {
      ArgumentUtility.CheckNotNull ("externalEvent", externalEvent);
      ArgumentUtility.CheckNotNull ("scriptEvent", scriptEvent);

      var name = string.Join (c_moduleEventSeperator, externalEvent.Name, eventName);

      var eventClients = GetClients (name);

      if (eventClients == null)
        throw new InvalidOperationException (string.Format ("Event '{0}' not found.", scriptEvent.Method.Name));

      _clients.Remove (name);

      scriptEvent -= FireEvent;
      eventClients.Clear();
    }


    private IList<KeyValuePair<string, Condition>> GetSubscriptions (string eventName, string moduleName)
    {
      var eventIdentifier = string.Join (c_moduleEventSeperator, moduleName, eventName);

      IList<KeyValuePair<string, Condition>> registrations;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw MissingRegistration (string.Join (c_moduleEventSeperator, eventName, moduleName));

      return registrations;
    }

    private void InitializeClients (string name)
    {
      if (GetClients (name) != null)
        throw DuplicateRegistration (name);

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
        throw MissingRegistration (name);

      foreach (var callback in callbackNames.Where (c => source.CheckRaiseCondition (c.Value)))
      {
        args.EventID = callback.Value.EventID;
        var eventArgs = new ScriptEventArgs { ScriptArgs = args, Function = callback.Key };
        EventFired (this, eventArgs);
      }
    }
  }
}