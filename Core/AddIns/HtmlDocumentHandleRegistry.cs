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
using System.Linq;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Factories;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns
{
  public sealed class HtmlDocumentHandleRegistry : IHtmlDocumentHandleRegistry, ISubscriptionHandler
  {
    private readonly IAddInManagerFactory<ServiceAddInBase> _serviceManagerFactory;
    private readonly IAddInManagerFactory<EventAddInBase> _eventManagerFactory;

    private const string c_documentAlreadyRegisteredFormatString = "DocumentHandle '{0}' is already registered.";
    private const string c_documentNotRegisteredFormatString = "DocumentHandle '{0}' is not registered.";

    private readonly IDictionary<object, IEventDispatcher> _eventDispatchers = new ConcurrentDictionary<object, IEventDispatcher>();
    private readonly IDictionary<object, IServiceManager> _serviceManagers = new ConcurrentDictionary<object, IServiceManager>();

    /// <summary>
    /// Constuctor to specify the composition container directly, avoids recomposition on creation.
    /// </summary>
    public HtmlDocumentHandleRegistry (
        IAddInManagerFactory<ServiceAddInBase> serviceManagerFactory,
        IAddInManagerFactory<EventAddInBase> eventManagerFactory)
    {
      ArgumentUtility.CheckNotNull ("serviceManagerFactory", serviceManagerFactory);
      ArgumentUtility.CheckNotNull ("eventManagerFactory", eventManagerFactory);


      _serviceManagerFactory = serviceManagerFactory;
      _eventManagerFactory = eventManagerFactory;
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

    public event EventHandler<DocumentRegisteredEventArgs> NewDocumentRegistered;

    public void RegisterDocumentHandle (HtmlDocumentHandle handle, IScriptingHost scriptingHost)
    {
      ArgumentUtility.CheckNotNull ("scriptingHost", scriptingHost);

      if (HasDocumentHandle (handle))
        throw new InvalidOperationException (string.Format (c_documentAlreadyRegisteredFormatString, handle));

      var eventDispatcher = (IEventDispatcher) _eventManagerFactory.CreateManager (handle);
      eventDispatcher.EventFired += scriptingHost.OnExecute;

      var serviceManager = (IServiceManager) _serviceManagerFactory.CreateManager (handle);

      _eventDispatchers.Add (handle, eventDispatcher);
      _serviceManagers.Add (handle, serviceManager);
      if (NewDocumentRegistered != null)
        NewDocumentRegistered (this, new DocumentRegisteredEventArgs (handle));
    }


    public void UnregisterDocumentHandle (HtmlDocumentHandle handle)
    {
      var eventDispatcher = GetEventDispatcher (handle);
      var serviceManager = GetServiceManager (handle);

      _eventDispatchers.Remove (handle);
      _serviceManagers.Remove (handle);

      eventDispatcher.Dispose();
      serviceManager.Dispose();
    }

    public bool HasDocumentHandle (HtmlDocumentHandle handle)
    {
      return HasEventDispatcher (handle) && HasServiceManager (handle);
    }


    public IEventDispatcher GetEventDispatcher (HtmlDocumentHandle handle)
    {
      IEventDispatcher eventDispatcher;

      if (! _eventDispatchers.TryGetValue (handle, out eventDispatcher))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      return eventDispatcher;
    }


    public bool HasEventDispatcher (HtmlDocumentHandle handle)
    {
      return _eventDispatchers.ContainsKey (handle);
    }


    public IServiceManager GetServiceManager (HtmlDocumentHandle handle)
    {
      IServiceManager serviceManager;

      if (!_serviceManagers.TryGetValue (handle, out serviceManager))
        throw new InvalidOperationException (string.Format (c_documentNotRegisteredFormatString, handle));

      return serviceManager;
    }

    public bool HasServiceManager (HtmlDocumentHandle handle)
    {
      return _serviceManagers.ContainsKey (handle);
    }

    public IEnumerable<TSubscriber> GetSubscribers<TSubscriber> (HtmlDocumentHandle handle) where TSubscriber : ISubscriber
    {
      var serviceBases = GetServiceManager (handle).GetSubscribers<TSubscriber>();
      var eventBases = GetEventDispatcher (handle).GetSubscribers<TSubscriber>();

      return eventBases.Union (serviceBases);
    }
  }
}