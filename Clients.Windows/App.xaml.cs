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
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Factories;
using DesktopGap.AddIns.Services;
using DesktopGap.Clients.Windows.Protocol.Wrapper;
using DesktopGap.Clients.Windows.Protocol.Wrapper.Factories;
using DesktopGap.Clients.Windows.WebBrowser;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Configuration;
using DesktopGap.Security.AddIns;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
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
    private const string c_addInDirectory = @".";
    private IWebBrowserFactory _browserFactory;

    public App ()
    {
    }

    //public class DesktopGapConfigurator
    //{

    //  public DesktopGapConfigurator (Uri manifestLocation)
    //  {
    //    ArgumentUtility.CheckNotNull ("manifestLocation", manifestLocation);
        
    //    ManifestLocation = manifestLocation;
    //  }

    //  public IUrlFilter 

    //        public Uri ManifestLocation { get; set; }

    //}

    [STAThread]
    private void Application_Startup (object sender, StartupEventArgs e)
    {
      var args = Args.Parse<DesktopGapCommandLineArguments> (e.Args);

      //try
      //{
        var configuration = DesktopGapConfigurationProvider.Create (String.Empty, args.ManifestUri.ToString()).GetConfiguration();
        var baseUri = new Uri (configuration.Application.BaseUrl);
        var startupUri = args.StartupUri;

        var thirdPartyUrlRules = configuration.Security.NonApplicationUrlRules;
        var applicationUrlRules = configuration.Security.ApplicationUrlRules;
        var startUpUrlRules = configuration.Security.StartupUrlRules;

        var resourceUrls = thirdPartyUrlRules.Union (applicationUrlRules);

        var addInRules = configuration.Security.AddInRules;

        var resourceFilter = new UrlFilter (resourceUrls);
        var nonApplicationUrlFilter = new UrlFilter (thirdPartyUrlRules);
        var addInAllowedFilter = new UrlFilter (applicationUrlRules);
        var startupFilter = new UrlFilter (startUpUrlRules);


        var catalog = new AggregateCatalog();
        var dirCatalog = new DirectoryCatalog (c_addInDirectory);
        catalog.Catalogs.Add (dirCatalog);
        var compositionContainer = new CompositionContainer (catalog);

        var tridentFeatures = new TridentFeatures();
        tridentFeatures.BrowserEmulationMode = TridentWebBrowserMode.ForcedIE10;
        tridentFeatures.GpuAcceleration = true;

        var filter = new ProtocolWrapperManager();
        filter.RegisterProtocol (new FilteredHttpProtocolFactory (resourceFilter));
        //filter.RegisterProtocol (new FilteredHttpsProtocolFactory (new UrlFilter (baseUri, urlRules)));

        var addInFilter = new AddInFilter (addInRules);

        var htmlDocumentHandleRegistry = new HtmlDocumentHandleRegistry (
            new ServiceManagerFactory (new CompositionBasedAddInFactory<ExternalServiceBase> (compositionContainer, addInFilter)),
            new EventManagerFactory (new CompositionBasedAddInFactory<ExternalEventBase> (compositionContainer, addInFilter)));

        var subscriptionHandler = (ISubscriptionProvider) htmlDocumentHandleRegistry;
        _browserFactory = new TridentWebBrowserFactory (htmlDocumentHandleRegistry, subscriptionHandler, nonApplicationUrlFilter, startupFilter, addInAllowedFilter);

        var viewDispatcher = new TridentViewDispatcher (_browserFactory, subscriptionHandler);

        htmlDocumentHandleRegistry.DocumentRegistered += viewDispatcher.OnDocumentRegistered;
        htmlDocumentHandleRegistry.BeforeDocumentUnregister += viewDispatcher.OnBeforeDocumentUnregister;


        var mainWindow = new BrowserWindow (configuration.Application.Name, baseUri, viewDispatcher);
        mainWindow.NewTab (baseUri, BrowserWindowStartMode.Active);

        if (startupUri != null && (startupFilter.IsAllowed (startupUri) || nonApplicationUrlFilter.IsAllowed(startupUri)))
          mainWindow.NewTab (startupUri, BrowserWindowStartMode.Active);
        mainWindow.Icon = new BitmapImage (configuration.Application.GetIconUri());
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