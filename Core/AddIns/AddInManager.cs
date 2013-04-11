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
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns
{
  public class AddInManager : IAddInManager
  {
    private readonly IDictionary<object, IEventDispatcher> _eventDispatchers =
        new ConcurrentDictionary<object, IEventDispatcher>();

    private readonly IDictionary<object, IServiceManager> _serviceManagers =
        new ConcurrentDictionary<object, IServiceManager>();

    /// <summary>
    /// Constuctor to specify the composition container directly, avoids recomposition on creation.
    /// </summary>
    public AddInManager ()
    {
    }

    public void AddEventDispatcher (HtmlDocumentHandle key, IEventDispatcher eventDispatcher)
    {
      _eventDispatchers.Add (key, eventDispatcher);
    }

    public IEventDispatcher GetEventDispatcher (HtmlDocumentHandle handle)
    {
      return _eventDispatchers[handle];
    }

    public void RemoveEventDispatcher (HtmlDocumentHandle handle)
    {
      _eventDispatchers.Remove (handle);
    }

    public void AddServiceManager (HtmlDocumentHandle key, IServiceManager serviceManager)
    {
      _serviceManagers.Add (key, serviceManager);
    }

    public IServiceManager GetServiceManager (HtmlDocumentHandle handle)
    {
      return _serviceManagers[handle];
    }

    public void RemoveServiceManager (HtmlDocumentHandle handle)
    {
      _serviceManagers.Remove (handle);
    }

    public void Dispose ()
    {
      foreach (var serviceManager in _serviceManagers)
      {
        serviceManager.Value.Dispose();
      }

      foreach (var eventDispatcher in _eventDispatchers)
        eventDispatcher.Value.Dispose();
    }
  }
}