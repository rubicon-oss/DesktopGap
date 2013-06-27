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
using DesktopGap.AddIns.Events;
using DesktopGap.Security;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentViewDispatcher : ViewDispatcherBase
  {
    private readonly IDictionary<Tuple<TargetAddressType, BrowserTab.TabType>, BrowserTabState> _states;

    public TridentViewDispatcher (IWebBrowserFactory browserFactory, ISubscriptionProvider subscriptionProvider, IDictionary<Tuple<TargetAddressType, BrowserTab.TabType>, BrowserTabState> states)
        : base (browserFactory, subscriptionProvider)
    {
      ArgumentUtility.CheckNotNull ("states", states);
      
      _states = states;
    }


    public override void NewView (BrowserWindowTarget target, Uri uri, BrowserWindowStartMode startMode)
    {
      var browser = CreateBrowser();
      Prepare (browser, uri, startMode, target);
      browser.Navigate (uri.ToString()); // starts event chain (handled in base class)
    }

    protected override void Dispatch (IExtendedWebBrowser browser, BrowserWindowTarget target, BrowserWindowStartMode startMode, string targetName, TargetAddressType addressType)
    {
      var webBrowser = (TridentWebBrowser) browser;
      IWebBrowserView view;
      Guid id;
      if(!Guid.TryParse (targetName, out id))
        id = new Guid();

      if (target == BrowserWindowTarget.PopUp)
        view = new PopUpWindow (webBrowser, id);
      else
        view = new BrowserTab (webBrowser, id, _states);
      ViewCreationDone (view, startMode, addressType);
    }
  }
}