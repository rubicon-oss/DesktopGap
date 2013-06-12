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
using System.IO;
using System.Linq;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Factories;
using DesktopGap.AddIns.Services;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Configuration;
using DesktopGap.Security.AddIns;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows
{
  public class DesktopGapConfigurator
  {
    private const string c_addInDirectory = @".";

    public struct ApplicationInfo
    {
      public string Name { get; set; }
      public Uri BaseUri { get; set; }
      public Uri IconUri { get; set; }
      public Uri ManifestUri { get; set; }
      public Uri HomeUri { get; set; }

      public bool AlwaysShowUrl { get; set; }
      public bool AllowCloseHomeTab { get; set; }
      public bool AlwaysOpenHomeUrl { get; set; }
    }

    public DesktopGapConfigurator ()
    {

    }

    public IUrlFilter ResourceFilter { get; private set; }
    public IUrlFilter AddInAllowedFilter { get; private set; }
    public IUrlFilter StartUpFilter { get; private set; }
    public IUrlFilter NonApplicationUrlFilter { get; private set; }

    public IAddInFilter AddInFilter { get; private set; }


    public ApplicationInfo Application { get; private set; }

    public void SetInternetExplorerFeatures (
        TridentWebBrowserMode mode, bool gpuAcceleration, bool localMachineLockdown, bool localObjectBlocking, bool restrictResourceProtocol)
    {
      new TridentFeatures
      {
          BrowserEmulationMode = mode,
          GpuAcceleration = gpuAcceleration,
          LocalMachineLockdown = localMachineLockdown,
          LocalObjectBlocking = localObjectBlocking,
          RestrictResourceProtocol = restrictResourceProtocol
      };
    }

    public void LoadFrom (Uri manifestLocation)
    {
      ArgumentUtility.CheckNotNull ("manifestLocation", manifestLocation);

      var configuration = DesktopGapConfigurationProvider.Create (String.Empty, manifestLocation.ToString()).GetConfiguration();

      Application = new ApplicationInfo
                    {
                        Name = configuration.Application.Name,
                        BaseUri = configuration.Application.GetBaseUri(),
                        IconUri = configuration.Application.GetIconUri(),
                        ManifestUri = manifestLocation,
                        AllowCloseHomeTab = configuration.Application.AllowCloseHomeTab,
                        AlwaysShowUrl = configuration.Application.AlwaysShowUrl,
                        AlwaysOpenHomeUrl = configuration.Application.AlwaysOpenHomeUrl,
                        HomeUri = configuration.Application.GetHomeUri()
                    };

      var thirdPartyUrlRules = configuration.Security.NonApplicationUrlRules;
      var applicationUrlRules = configuration.Security.ApplicationUrlRules;
      var startUpUrlRules = configuration.Security.StartupUrlRules;


      var resourceUrls = thirdPartyUrlRules.Union (applicationUrlRules);

      var addInRules = configuration.Security.AddInRules;

      ResourceFilter = new UrlFilter (resourceUrls);
      NonApplicationUrlFilter = new UrlFilter (thirdPartyUrlRules);
      AddInAllowedFilter = new UrlFilter (applicationUrlRules);
      StartUpFilter = new UrlFilter (startUpUrlRules);
      AddInFilter = new AddInFilter (addInRules);
    }

    public IHtmlDocumentHandleRegistry CreateDocumentRegistry (string addInDirectory = c_addInDirectory)
    {
            ArgumentUtility.CheckNotNull ("addInDirectory", addInDirectory);

      if (!Directory.Exists (addInDirectory))
        throw new ArgumentException (string.Format ("The configured AddIn directory '{0}' does not exist", addInDirectory));

      var catalog = new AggregateCatalog();

      var dirCatalog = new DirectoryCatalog (addInDirectory);
      catalog.Catalogs.Add (dirCatalog);
      var compositionContainer = new CompositionContainer (catalog);


      return new HtmlDocumentHandleRegistry (
          new ServiceManagerFactory (
              new CompositionBasedAddInFactory<ExternalServiceBase> (compositionContainer, AddInFilter)),
          new EventManagerFactory (new CompositionBasedAddInFactory<ExternalEventBase> (compositionContainer, AddInFilter)));
    }
  }
}