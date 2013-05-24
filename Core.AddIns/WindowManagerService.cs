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
using System.Runtime.InteropServices;
using DesktopGap.AddIns.Events.Subscriptions;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.AddIns
{
  [ComVisible (true)]
  [PartCreationPolicy (CreationPolicy.Shared)]
  public class WindowManagerService : ExternalServiceBase, IWindowEventSubscriber
  {
    private struct WindowPreparations
    {
      public string URL;
      public BrowserWindowTarget Type;
      public BrowserWindowStartMode StartMode;
    }

          private const string c_idPrefix = "_";


    private readonly IDictionary<string, WindowPreparations> _registeredPreparations = new Dictionary<string, WindowPreparations>();
    private readonly Dictionary<Guid, IWebBrowserView> _modalWindows = new Dictionary<Guid, IWebBrowserView>();

    
    public override void Dispose ()
    {
    }

    public override string Name
    {
      get { return "WindowManagerService"; }
    }


    public void ShowModal (string id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var identifier = MakeGuid (id);

      if (_modalWindows.ContainsKey (identifier))
        _modalWindows[identifier].Show (BrowserWindowStartMode.Modal);
      else
        throw new InvalidOperationException (string.Format ("Window with id {0} has not been prepared 'modal'", identifier));
    }

    public string PrepareNewWindow (string url, string type, string option)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("url", url);

      var id = MakeId (Guid.NewGuid());
      BrowserWindowTarget parsedType;
      if (!BrowserWindowTarget.TryParse (type, true, out parsedType))
        parsedType = BrowserWindowTarget.Tab;

      BrowserWindowStartMode startMode;
      if (!BrowserWindowStartMode.TryParse (option, true, out startMode))
        startMode = BrowserWindowStartMode.Active;

      _registeredPreparations[id] = new WindowPreparations { Type = parsedType, URL = url, StartMode = startMode };
      return id;
    }

    public void OpenTab (string url, string arguments)
    {
    }

    public void OnPrepareNavigation (object sender, NavigationEventArgs args)
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (args.Url.ToString() == "about:blank")
        return;

      WindowPreparations preparations;
      var isRegistered = _registeredPreparations.TryGetValue (args.TargetName, out preparations);
      args.Handled = isRegistered;

      if (!isRegistered)
        return;

      args.BrowserWindowTarget = preparations.Type;
      args.StartMode = preparations.StartMode;
      _registeredPreparations.Remove (args.TargetName);
    }

    public void OnViewCreated (object sender, NewViewEventArgs e)
    {
      if (e.StartMode == BrowserWindowStartMode.Modal)
        _modalWindows[e.View.Identifier] = e.View;
    }

    private string MakeId (Guid guid)
    {
      return c_idPrefix + guid.ToString();
    }
    private Guid MakeGuid (string id)
    {
      var guidstr = id.Remove(id.IndexOf(c_idPrefix, System.StringComparison.Ordinal), c_idPrefix.Length);
      return Guid.Parse (guidstr);
    }
  }
}