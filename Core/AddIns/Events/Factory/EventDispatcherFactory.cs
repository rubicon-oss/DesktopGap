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
using System.ComponentModel.Composition.Hosting;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Events.Factory
{
  public class EventDispatcherFactory : IEventDispatcherFactory
  {
    private readonly CompositionContainer _compositionContainer;
    private readonly IList<IEventAddIn> _preLoadedSharedEvents;
    private bool _factoryCalled;

    public EventDispatcherFactory (CompositionContainer compositionContainer)
    {
      ArgumentUtility.CheckNotNull ("compositionContainer", compositionContainer);
      _compositionContainer = compositionContainer;

      _preLoadedSharedEvents = new List<IEventAddIn>();
    }


    public void AddPreloadedEvent (IEventAddIn addIn)
    {
      if(_factoryCalled)
        throw new InvalidOperationException ("An instance has already been created, you cannot add things anymore");

      _preLoadedSharedEvents.Add (addIn);
    }

    public IEventDispatcher CreateEventDispatcher (HtmlDocumentHandle document)
    {
      try
      {
        var eventManager = new EventManager (document, _preLoadedSharedEvents);
        _compositionContainer.ComposeParts (eventManager);
        return eventManager;
      }
      finally
      {
        _factoryCalled = true;
      }
    }
  }
}