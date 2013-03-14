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
using System.Windows.Forms;
using DesktopGap.Browser;
using DesktopGap.Clients.Windows.Components;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class Browser : Window
  {
    public Browser ()
    {
      InitializeComponent();
    }

    private void OnWindowOpen (WindowOpenEventArgs eventArgs)
    {
      var newTab = CreateBrowserTab (new ExtendedTridentWebBrowser());
      eventArgs.TargetWindow = newTab.ExtendedWebBrowser;
      if (!eventArgs.IsInBackground)
        newTab.Focus();
    }

    private BrowserTab CreateBrowserTab (IExtendedWebBrowser browser)
    {
      var x = new BrowserTab (_tabControl, (WebBrowser) browser);

      x.ExtendedWebBrowser.PageLoaded += (b) => _ConsoleListBox.Items.Add (b.ToString() + " loaded");
      x.ExtendedWebBrowser.WindowOpen += OnWindowOpen; // TODO avoid stackoverflow
      ((ExtendedTridentWebBrowser) browser).Output += ToConsole;
      return x;
    }

    private void ToConsole (string s)
    {
      _ConsoleListBox.Items.Add (s + " passed");
      _ConsoleListBox.ScrollIntoView (_ConsoleListBox.Items[_ConsoleListBox.Items.Count - 1]);
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      var newTab = CreateBrowserTab (new ExtendedTridentWebBrowser());
      //newTab.ExtendedWebBrowser.Navigate (@"\\rubicon\home\claus.matzinger\test.html");
      newTab.ExtendedWebBrowser.Navigate (_urlTextBox.Text);

      _tabControl.Items.Add (newTab);
      newTab.Focus();
    }
  }
}