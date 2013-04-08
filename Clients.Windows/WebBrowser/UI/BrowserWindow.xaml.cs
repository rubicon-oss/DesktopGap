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
using System.Windows;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class BrowserWindow
  {
    private readonly IWebBrowserFactory _browserFactory;

    public BrowserWindow (IWebBrowserFactory browserFactory)
    {
      ArgumentUtility.CheckNotNull ("browserFactory", browserFactory);
      _browserFactory = browserFactory;
      InitializeComponent();
    }

    private void OnWindowOpen (object sender, WindowOpenEventArgs eventArgs)
    {
      var webBrowser = _browserFactory.CreateBrowser();

      var newTab = CreateBrowserTab (webBrowser);
      eventArgs.TargetWindow = webBrowser;

      _tabControl.Items.Add (newTab);

      if (!eventArgs.IsInBackground)
        newTab.Focus();
    }

    private BrowserTab CreateBrowserTab (IExtendedWebBrowser browser)
    {
      var browserTab = new BrowserTab (_tabControl, new WebBrowserHost((TridentWebBrowser) browser));

      browser.WindowOpen += OnWindowOpen; // TODO avoid stackoverflow
      return browserTab;
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      var browser = _browserFactory.CreateBrowser();
      var newTab = CreateBrowserTab (browser);
      browser.Navigate (_urlTextBox.Text);

      _tabControl.Items.Add (newTab);
      newTab.Focus();
    }
  }
}