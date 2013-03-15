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
using DesktopGap.AddIns.Events;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns
{
  public class EventManager : IEventManager, IPartImportsSatisfiedNotification
  {
    private const string c_moduleEventSeperator = ".";

    [ImportMany (typeof (IExternalEvent))]
    private IEnumerable<Lazy<IExternalEvent>> _events;

    private readonly IDictionary<string, IList<string>> _clients = new Dictionary<string, IList<string>>();

    public IEnumerable<IExternalEvent> Events { get; private set; }

    private readonly ICallbackHost _callbackHost;

    public EventManager (ICallbackHost callbackHost)
    {
      if (callbackHost == null)
        throw new ArgumentNullException ("callbackHost");

      _callbackHost = callbackHost;
      Events = new List<IExternalEvent>();
    }

    private IList<string> GetSubscriptions (string eventName, string callbackName, string moduleName)
    {
      if (String.IsNullOrEmpty (eventName))
        throw new ArgumentNullException ("eventName");

      if (String.IsNullOrEmpty (callbackName))
        throw new ArgumentNullException ("callbackName");

      if (String.IsNullOrEmpty (moduleName))
        throw new ArgumentNullException ("moduleName");


      var eventIdentifier = moduleName + c_moduleEventSeperator + eventName;

      IList<string> registrations = null;
      if (!_clients.TryGetValue (eventIdentifier, out registrations))
        throw new Exception ("Event not found"); // TODO make proper exception class(es)

      return registrations;
    }

    public void Register (string eventName, string callbackName, string moduleName)
    {
      IList<string> subscriptions = GetSubscriptions (eventName, callbackName, moduleName);

      subscriptions.Add (callbackName);
    }

    public void Unregister (string eventName, string callbackName, string moduleName)
    {
      IList<string> subscriptions = GetSubscriptions (eventName, callbackName, moduleName);

      subscriptions.Remove (callbackName);
    }

    public void RegisterEvent (ref ScriptEvent scriptEvent)
    {
      if (scriptEvent == null)
        throw new ArgumentNullException ("scriptEvent");

      scriptEvent += FireEvent;
    }

    public void UnregisterEvent (ref ScriptEvent scriptEvent)
    {
      if (scriptEvent == null)
        throw new ArgumentNullException ("scriptEvent");

      scriptEvent -= FireEvent;
    }

    private void ReregisterEvents ()
    {
      _clients.Clear();

      Events = from e in _events select e.Value;

      foreach (var eventModule in Events)
      {
        var name = eventModule.Name;
        IList<string> moduleEvents = (from evt in eventModule.GetType().GetEvents()
                                      where evt.GetType() == typeof(ScriptEvent)
                                      select name + "." + evt.Name).ToList<string>();

        foreach (var eventName in moduleEvents)
        {
          IList<string> result = null;
          if (!_clients.TryGetValue (eventName, out result))
            _clients[eventName] = new List<string>();
          //else
          // throw new Exception("Duplicate event found") // TODO throw exception here or just fail silently?
        }
      }
    }

    public void OnImportsSatisfied ()
    {
      ReregisterEvents();
    }

    private void FireEvent (ScriptEvent sender, ScriptArgs args)
    {
      var name = sender.GetType().FullName; // TODO does this work like this?

      IList<string> callbackNames = _clients[name]; // TODO throw exception if event not found?

      foreach (var callbackName in callbackNames)
        _callbackHost.Call (callbackName, args);
    }
  }
}