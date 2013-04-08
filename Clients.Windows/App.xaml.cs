using DesktopGap.Clients.Windows.WebBrowser;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.WebBrowser;
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
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Threading;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private const string c_addInDirectory = @".";
    private IWebBrowserFactory _browserFactory;

    private void Application_Startup (object sender, StartupEventArgs e)
    {
      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add (new DirectoryCatalog (c_addInDirectory));
      _browserFactory = new TridentWebBrowserFactory(catalog);

      try
      {
        var mainWindow = new BrowserWindow(_browserFactory);
        mainWindow.Show();
      }
      catch (Exception ex)
      {
        MessageBox.Show (ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Shutdown();
      }
    }

    private void Application_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      MessageBox.Show (e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      e.Handled = true;
      Shutdown();
    }
  }
}