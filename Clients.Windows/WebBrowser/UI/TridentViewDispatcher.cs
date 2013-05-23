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
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentViewDispatcher : ViewDispatcherBase
  {
    public TridentViewDispatcher (IWebBrowserFactory browserFactory, ISubscriptionProvider subscriptionProvider)
        : base (browserFactory, subscriptionProvider)
    {
    }


    public override void NewView (BrowserWindowTarget target, string url, BrowserWindowStartMode startMode)
    {
      var browser = CreateBrowser();
      Prepare (browser, url, startMode, target);
      browser.Navigate (url); // starts event chain (handled in base class)
    }

    protected override void Dispatch (IExtendedWebBrowser browser, BrowserWindowTarget target, BrowserWindowStartMode startMode, string targetName)
    {
      var webBrowser = (TridentWebBrowser) browser;
      IWebBrowserView view;
      Guid id;
      if(!Guid.TryParse (targetName, out id))
        id = new Guid();

      if (target == BrowserWindowTarget.PopUp)
        view = new PopUpWindow (webBrowser, id);
      else
        view = new BrowserTab (webBrowser, id);
      ViewCreationDone (view, startMode);
    }
  }
}