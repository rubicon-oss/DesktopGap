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
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using DesktopGap.Clients.Windows.Components;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for BrowserTab.xaml
  /// </summary>
  public sealed class BrowserTab : TabItem
  {
    private readonly WebBrowserHost _webBrowserHost;

    private readonly ItemsControl _parent;
    private readonly bool _isCloseable;

    public BrowserTab (ItemsControl parent, WebBrowserHost webBrowserHost, bool isCloseable = true)
    {
      ArgumentUtility.CheckNotNull ("webBrowserHost", webBrowserHost);
      ArgumentUtility.CheckNotNull ("parent", parent);


      _webBrowserHost = webBrowserHost;
      _webBrowserHost.WebBrowser.PageLoaded += OnPageLoaded;
      GotFocus += OnTabFocussed;
      
      _parent = parent;
      _webBrowserHost = webBrowserHost;
      _isCloseable = isCloseable;

      Content = _webBrowserHost;
    }

    private void OnTabFocussed (object sender, EventArgs e)
    {
      _webBrowserHost.WebBrowser.OnFocussed(sender, e);
    }

    private void OnPageLoaded (object sender, IExtendedWebBrowser webBrowser)
    {
      var header = new CloseableTabHeader (_webBrowserHost.WebBrowser.Title, _isCloseable);
      if (_isCloseable)
        header.TabClose += OnTabClose;
      Header = header;
    }

    private void OnTabClose (object sender, EventArgs e)
    {
      _parent.Items.Remove (this);
      CleanUp();
    }

    private void CleanUp ()
    {
      // remove event handler to avoid memory leaks
      _webBrowserHost.WebBrowser.PageLoaded -= OnPageLoaded;
      _webBrowserHost.WebBrowser = null;
    }
  }
}