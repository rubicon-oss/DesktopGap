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
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.Clients.Windows.WebBrowser
{
  public class TridentWebBrowserFactory : IWebBrowserFactory
  {
    public TridentWebBrowserFactory (
        IHtmlDocumentHandleRegistry htmlDocumentHandleRegistry,
        ISubscriptionProvider subscriptionProvider,
        IUrlFilter pageFilter,
        IUrlFilter addInAllowedFilter)
    {
      ArgumentUtility.CheckNotNull ("htmlDocumentHandleRegistry", htmlDocumentHandleRegistry);
      ArgumentUtility.CheckNotNull ("subscriptionProvider", subscriptionProvider);
      ArgumentUtility.CheckNotNull ("pageFilter", pageFilter);
      ArgumentUtility.CheckNotNull ("addInAllowedFilter", addInAllowedFilter);

      PageFilter = pageFilter;
      AddInAllowedFilter = addInAllowedFilter;

      SubscriptionProvider = subscriptionProvider;
      HtmlDocumentHandleRegistry = htmlDocumentHandleRegistry;
    }

    public IUrlFilter PageFilter { get; private set; }
    public IUrlFilter AddInAllowedFilter { get; private set; }
    public IHtmlDocumentHandleRegistry HtmlDocumentHandleRegistry { get; private set; }
    public ISubscriptionProvider SubscriptionProvider { get; private set; }

    public IExtendedWebBrowser CreateBrowser ()
    {
      var browser = new TridentWebBrowser (HtmlDocumentHandleRegistry, SubscriptionProvider, PageFilter, AddInAllowedFilter);


      return browser;
    }
  }
}