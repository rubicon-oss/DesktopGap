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
using DesktopGap.AddIns.Events;
using DesktopGap.Clients.Windows.Components;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;
using WFWebBrowser = System.Windows.Forms.WebBrowser;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class BrowserWindow
  {
    private readonly TridentWebBrowserFactory _browserFactory;

    public BrowserWindow (TridentWebBrowserFactory browserFactory)
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
      SystemEventHub.AddWebBrowser (browser);
      var browserTab = new BrowserTab (_tabControl, (WFWebBrowser) browser);

      browserTab.ExtendedWebBrowser.PageLoaded += (s, b) => _ConsoleListBox.Items.Add (b.ToString() + " loaded");
      browserTab.ExtendedWebBrowser.WindowOpen += OnWindowOpen; // TODO avoid stackoverflow
      ((TridentWebBrowser) browser).Output += ToConsole;
      return browserTab;
    }

    private void ToConsole (string s)
    {
      _ConsoleListBox.Items.Add (s + " passed");
      _ConsoleListBox.ScrollIntoView (_ConsoleListBox.Items[_ConsoleListBox.Items.Count - 1]);
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      var newTab = CreateBrowserTab (_browserFactory.CreateBrowser());
      newTab.ExtendedWebBrowser.Navigate (_urlTextBox.Text);

      _tabControl.Items.Add (newTab);
      newTab.Focus();
    }
  }
}