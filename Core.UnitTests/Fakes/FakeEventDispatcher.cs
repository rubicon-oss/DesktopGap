// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;

namespace DesktopGap.UnitTests.Fakes
{
  public class FakeEventDispatcher : AddInManagerBase<EventAddInBase>, IEventDispatcher
  {
    public event EventHandler<ScriptEventArgs> EventFired;

    protected override void Dispose (bool disposing)
    {
      
    }

    public void Register (string eventName, string callbackName, string moduleName, Condition argument)
    {
    }

    public void Unregister (string eventName, string callbackName, string moduleName)
    {
    }

    public bool HasEvent (string moduleName, string eventName)
    {
      return true;
    }

    public new IEnumerable<TSubscription> GetSubscribers<TSubscription> ()
    {
      return Enumerable.Empty<TSubscription>();
    }
  }
}