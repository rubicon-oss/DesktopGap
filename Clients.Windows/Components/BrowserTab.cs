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
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using WFWebBrowser = System.Windows.Forms.WebBrowser;

namespace DesktopGap.Clients.Windows.Components
{
  /// <summary>
  /// Interaction logic for BrowserTab.xaml
  /// </summary>
  public sealed class BrowserTab : TabItem
  {
    private TridentWebBrowser _extendedWebBrowser;

    private readonly ItemsControl _parent;
    private readonly bool _isCloseable;

    public BrowserTab (ItemsControl parent, TridentWebBrowser extendedWebBrowser, bool isCloseable = false)
    {
      ArgumentUtility.CheckNotNull ("extendedWebBrowser", extendedWebBrowser);
      ArgumentUtility.CheckNotNull ("parent", parent);


      _extendedWebBrowser = extendedWebBrowser;
      _extendedWebBrowser.PageLoaded += OnPageLoaded;
      GotFocus += OnTabFocussed;
      
      _parent = parent;
      _isCloseable = isCloseable;

      var host = new WindowsFormsHost { Child = extendedWebBrowser };
      AddChild (host);
    }

    private void OnTabFocussed (object sender, EventArgs e)
    {
      _extendedWebBrowser.OnFocussed(sender, e);
    }

    private void OnPageLoaded (object sender, IExtendedWebBrowser webBrowser)
    {
      var header = new CloseableTabHeader (_extendedWebBrowser.Title, _isCloseable);
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
      _extendedWebBrowser.PageLoaded -= OnPageLoaded;
      _extendedWebBrowser = null;
    }
  }
}