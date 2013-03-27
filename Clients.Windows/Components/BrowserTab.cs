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
using DesktopGap.WebBrowser;
using WFWebBrowser = System.Windows.Forms.WebBrowser;

namespace DesktopGap.Clients.Windows.Components
{
  /// <summary>
  /// Interaction logic for BrowserTab.xaml
  /// </summary>
  public sealed class BrowserTab : TabItem
  {
    public IExtendedWebBrowser ExtendedWebBrowser { get; private set; }

    private readonly ItemsControl _parent;

    public BrowserTab (ItemsControl parent, WFWebBrowser extendedWebBrowser)
    {
      if (parent == null)
        throw new ArgumentNullException ("parent");

      if (extendedWebBrowser == null)
        throw new ArgumentNullException ("extendedWebBrowser");

      _parent = parent;

      var host = new WindowsFormsHost { Child = extendedWebBrowser };
      AddChild (host);

      var webBrowser = extendedWebBrowser as IExtendedWebBrowser;
      if (webBrowser == null)
        return;

      ExtendedWebBrowser = webBrowser;
      ExtendedWebBrowser.PageLoaded += OnPageLoaded;
    }

    private void OnPageLoaded (object sender, IExtendedWebBrowser webBrowser)
    {
      if (webBrowser == null)
        throw new ArgumentNullException ("webBrowser");
      var header = new CloseableTabHeader (ExtendedWebBrowser.Title);
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
      ExtendedWebBrowser.PageLoaded -= OnPageLoaded;
      ExtendedWebBrowser = null;
    }
  }
}