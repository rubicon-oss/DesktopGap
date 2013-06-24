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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DesktopGap.AddIns.Events;
using DesktopGap.Clients.Windows.Protocol.Wrapper;
using DesktopGap.Clients.Windows.Protocol.Wrapper.Factories;
using DesktopGap.Clients.Windows.WebBrowser;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Security;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using PowerArgs;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private IWebBrowserFactory _browserFactory;

    public App ()
    {
    }

    [STAThread]
    private void Application_Startup (object sender, StartupEventArgs e)
    {
      var args = Args.Parse<DesktopGapCommandLineArguments> (e.Args);

      //try
      //{

      var configurator = new DesktopGapConfigurator();

      configurator.LoadFrom (args.ManifestUri);
      configurator.SetInternetExplorerFeatures (TridentWebBrowserMode.ForcedIE10, true, true, true, true);

      var startupUri = args.StartupUri;

      if (configurator.EnableResourceFilter)
      {
        var filter = new ProtocolWrapperManager();
        filter.RegisterProtocol (new FilteredHttpProtocolFactory (configurator.ResourceFilter));
        filter.RegisterProtocol (new FilteredHttpsProtocolFactory (configurator.ResourceFilter));
      }

      var htmlDocumentHandleRegistry = configurator.CreateDocumentRegistry (".");

      var subscriptionHandler = (ISubscriptionProvider) htmlDocumentHandleRegistry;
      _browserFactory = new TridentWebBrowserFactory (
          htmlDocumentHandleRegistry,
          subscriptionHandler,
          configurator.NonApplicationUrlFilter,
          configurator.StartUpFilter,
          configurator.AddInAllowedFilter);

      //var converter = new ColorConverter();

      //var applicationTabBrush = new RadialGradientBrush (
      //    (Color) converter.ConvertFrom (configurator.ApplicationTabColorCode),
      //    Brushes.Transparent.Color);

      //var nonApplicationTabBrush = new RadialGradientBrush (
      //    (Color) converter.ConvertFrom (configurator.NonApplicationTabColorCode),
      //    Brushes.Transparent.Color);

      //var homeTabBrush = new RadialGradientBrush (
      //  (Color) converter.ConvertFrom (configurator.HomeTabColorCode), 
      //  Brushes.Transparent.Color);

      var converter = new BrushConverter();

      var applicationTabBrush = (Brush) converter.ConvertFrom (configurator.ApplicationTabColorCode);

      var nonApplicationTabBrush = (Brush) converter.ConvertFrom (configurator.NonApplicationTabColorCode);

      var homeTabBrush = (Brush) converter.ConvertFrom (configurator.HomeTabColorCode);


      var states = new Dictionary<Tuple<TargetAddressType, BrowserTab.TabType>, BrowserTabState>
                   {
                       {
                           Tuple.Create (TargetAddressType.Application, BrowserTab.TabType.CommonTab),
                           new BrowserTabState
                           {
                               HeaderColor = applicationTabBrush,
                               IsClosable = true,
                               ShowAddressBar = configurator.Application.AlwaysShowUrl
                           }
                       },
                       {
                           Tuple.Create (TargetAddressType.NonApplication, BrowserTab.TabType.CommonTab),
                           new BrowserTabState
                           {
                               HeaderColor = nonApplicationTabBrush,
                               IsClosable = true,
                               ShowAddressBar = true
                           }
                       },
                       {
                           Tuple.Create (TargetAddressType.Application, BrowserTab.TabType.HomeTab),
                           new BrowserTabState
                           {
                               HeaderColor = homeTabBrush,
                               IsClosable = false,
                               ShowAddressBar = configurator.Application.AlwaysShowUrl
                           }
                       },
                       {
                           Tuple.Create (TargetAddressType.NonApplication, BrowserTab.TabType.HomeTab),
                           new BrowserTabState
                           {
                               HeaderColor = nonApplicationTabBrush,
                               IsClosable = false,
                               ShowAddressBar = true
                           }
                       },
                   };

      var viewDispatcher = new TridentViewDispatcher (_browserFactory, subscriptionHandler, states);

      htmlDocumentHandleRegistry.DocumentRegistered += viewDispatcher.OnDocumentRegistered;
      htmlDocumentHandleRegistry.BeforeDocumentUnregister += viewDispatcher.OnBeforeDocumentUnregister;


      var homeUri = configurator.Application.BaseUri;

      if (configurator.Application.HomeUri != null)
        homeUri = configurator.Application.HomeUri;


      var mainWindow = new BrowserWindow (
          configurator.Application.Name,
          configurator.Application.IconUri,
          homeUri,
          viewDispatcher);
      if (configurator.Application.AlwaysOpenHomeUrl)
        mainWindow.NewStickyTab (homeUri, BrowserWindowStartMode.Active);

      if (startupUri != null && (configurator.StartUpFilter.IsAllowed (startupUri) || configurator.NonApplicationUrlFilter.IsAllowed (startupUri)))
        mainWindow.NewTab (startupUri, BrowserWindowStartMode.Active);

      mainWindow.Show();
      //}
      //catch (Exception ex)
      //{
      //  MessageBox.Show (ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      //  Shutdown();
      //}
    }

    private void Application_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      MessageBox.Show (e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      e.Handled = true;
      Shutdown();
    }
  }
}