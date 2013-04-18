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
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.Resources;

namespace DesktopGap.UnitTests.Fakes
{
  public class FakeEventAddIn : IEventAddIn
  {
    public string FakeEventName { get; set; }

    private const string c_fakeModuleName = "some module";

    private ScriptEvent _scriptEvent;

    public FakeEventAddIn ()
    {
      FakeEventName = "some event";
    }

    public void Dispose ()
    {
    }
    
    public bool IsLoaded { get; private set; }

    public bool EventsRegistered { get; private set; }


    public IResourceManager ResourceManager { get; private set; }

    public string Name
    {
      get { return c_fakeModuleName; }
    }

    public void OnBeforeLoad (HtmlDocumentHandle document)
    {
      IsLoaded = true;
    }

    public void OnBeforeUnload (HtmlDocumentHandle document)
    {
      IsLoaded = false;
    }

    public bool CheckRaiseCondition (Condition argument)
    {
      return true;
    }

    public void RegisterEvents (IEventHost eventHost)
    {
      eventHost.RegisterEvent (this, ref _scriptEvent, FakeEventName);
      EventsRegistered = true;
    }

    public void UnregisterEvents (IEventHost eventHost)
    {
      eventHost.UnregisterEvent (this, ref _scriptEvent, FakeEventName);
      EventsRegistered = false;
    }

    public void RaiseEvent ()
    {
      var data = new FakeEventData { ContainsData = true, EventID = FakeEventName };
      _scriptEvent (this, FakeEventName, data);
    }
  }
}