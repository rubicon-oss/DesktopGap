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
using System.IO;
using System.Net;
using DesktopGap.Configuration;
using DesktopGap.Configuration.Security;
using DesktopGap.Utilities;
using DesktopGap.Utilities.Web;
using Remotion.Dms.Shared.IO;

namespace DesktopGap.Security.Providers
{
  public static class SecurityProvider
  {
    private const string c_securitySectionName = "SecurityManifest";

    public static IDesktopGapConfigurationProvider<SecurityManifestConfiguration> Create (string configurationBase, string source)
    {
      ArgumentUtility.CheckNotNull ("configurationBase", configurationBase);
      ArgumentUtility.CheckNotNull ("source", source);

      if (Uri.IsWellFormedUriString (source, UriKind.Absolute))
        return new RemoteDesktopGapConfigurationProvider<SecurityManifestConfiguration> (
            new Uri (source), new WebClientWrapper (new WebClient()), new FileSystemHelper(), c_securitySectionName);

      if (Path.IsPathRooted (source))
        return new LocalDesktopGapConfigurationProvider<SecurityManifestConfiguration> (source, c_securitySectionName);

      return new LocalDesktopGapConfigurationProvider<SecurityManifestConfiguration> (
          Path.Combine (configurationBase, source), c_securitySectionName);
    }
  }
}