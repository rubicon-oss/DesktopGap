﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Factories;
using DesktopGap.AddIns.Services;
using DesktopGap.Clients.Windows.WebBrowser;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Security.AddIns;
using DesktopGap.Security.Providers;
using DesktopGap.Security.Urls;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.View;

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
      var security = SecurityProvider.Create (@"C:\Development", "desktopgap-default.conf").GetConfiguration();
      IUrlRules urlRules = security.Urls;
      IAddInRules addInRules = security.AddIns;

      var catalog = new AggregateCatalog();
      var dirCatalog = new DirectoryCatalog (c_addInDirectory);
      catalog.Catalogs.Add (dirCatalog);
      var compositionContainer = new CompositionContainer (catalog);


      var addInProvider = new HtmlDocumentHandleRegistry (
          new ServiceManagerFactory (new CompositionBasedAddInFactory<ExternalServiceBase> (compositionContainer, new AddInFilter(addInRules))),
          new EventManagerFactory (new CompositionBasedAddInFactory<ExternalEventBase> (compositionContainer, new AddInFilter(addInRules))));


      var baseUri = new Uri ("http://localhost:3936");
      _browserFactory = new TridentWebBrowserFactory (addInProvider, new UrlFilter (baseUri, urlRules));

      try
      {
        var mainWindow = new BrowserWindow (_browserFactory);
        foreach (var arg in e.Args)
          ((IWebBrowserWindow) mainWindow).NewTab (arg);
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