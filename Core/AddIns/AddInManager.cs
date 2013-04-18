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
using System.Collections.Concurrent;
using System.Collections.Generic;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns
{
  public sealed class AddInManager : IAddInManager
  {
    private const string c_documentAlreadyRegisteredFormatString = "DocumentHandle '{0}' is already registered.";
    private const string c_documentNotRegisteredFormatString = "DocumentHandle '{0}' is not registered.";

    private readonly IDictionary<object, IEventDispatcher> _eventDispatchers = new ConcurrentDictionary<object, IEventDispatcher>();
    private readonly IDictionary<object, IServiceManager> _serviceManagers = new ConcurrentDictionary<object, IServiceManager>();

    /// <summary>
    /// Constuctor to specify the composition container directly, avoids recomposition on creation.
    /// </summary>
    public AddInManager ()
    {
    }

    public void Dispose ()
    {
      foreach (var serviceManager in _serviceManagers)
        serviceManager.Value.Dispose();

      foreach (var eventDispatcher in _eventDispatchers)
        eventDispatcher.Value.Dispose();
    }

    public int ServiceManagerCount
    {
      get { return _serviceManagers.Count; }
    }

    public int EventDispatcherCount
    {
      get { return _eventDispatchers.Count; }
    }

    public void AddEventDispatcher (HtmlDocumentHandle handle, IEventDispatcher eventDispatcher)
    {
      ArgumentUtility.CheckNotNull ("eventDispatcher", eventDispatcher);

      if (_eventDispatchers.ContainsKey (handle))
        throw new InvalidOperationException (string.Format (c_documentAlreadyRegisteredFormatString, handle));

      _eventDispatchers.Add (handle, eventDispatcher);
    }

    public IEventDispatcher GetEventDispatcher (HtmlDocumentHandle handle)
    {
      IEventDispatcher eventDispatcher;

      if (!_eventDispatchers.TryGetValue (handle, out eventDispatcher))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      return eventDispatcher;
    }

    public void RemoveEventDispatcher (HtmlDocumentHandle handle)
    {
      if (!_eventDispatchers.ContainsKey (handle))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      _eventDispatchers.Remove (handle);
    }

    public bool HasEventDispatcher (HtmlDocumentHandle handle)
    {
      return _eventDispatchers.ContainsKey (handle);
    }

    public void AddServiceManager (HtmlDocumentHandle handle, IServiceManager serviceManager)
    {
      ArgumentUtility.CheckNotNull ("serviceManager", serviceManager);

      if (_serviceManagers.ContainsKey (handle))
        throw new InvalidOperationException (string.Format (c_documentAlreadyRegisteredFormatString, handle));

      _serviceManagers.Add (handle, serviceManager);
    }

    public IServiceManager GetServiceManager (HtmlDocumentHandle handle)
    {
      IServiceManager serviceManager;

      if (!_serviceManagers.TryGetValue (handle, out serviceManager))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      return serviceManager;
    }

    public void RemoveServiceManager (HtmlDocumentHandle handle)
    {
      if (!_serviceManagers.ContainsKey (handle))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      _serviceManagers.Remove (handle);
    }

    public bool HasServiceManager (HtmlDocumentHandle handle)
    {
      return _serviceManagers.ContainsKey (handle);
    }
  }
}