// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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
using System.Configuration;
using DesktopGap.Utilities;
using DesktopGap.Utilities.Web;
using Remotion.Dms.Shared.IO;

namespace DesktopGap.Configuration
{
  public class RemoteDesktopGapConfigurationProvider<TConfiguration> : DesktopGapConfigurationProviderBase<TConfiguration>
      where TConfiguration : ConfigurationSection
  {
    private readonly Uri _downloadUrl;

    private readonly IWebClient _webClient;
    private readonly IFileSystemHelper _helper;

    public RemoteDesktopGapConfigurationProvider (Uri downloadUrl, IWebClient webClient, IFileSystemHelper helper, string sectionName)
        : base (sectionName)
    {
      ArgumentUtility.CheckNotNull ("downloadUrl", downloadUrl);
      ArgumentUtility.CheckNotNull ("webClient", webClient);
      ArgumentUtility.CheckNotNull ("helper", helper);

      _downloadUrl = downloadUrl;
      _webClient = webClient;
      _helper = helper;
    }

    public Uri ApplicationMappingFile
    {
      get { return _downloadUrl; }
    }

    public override TConfiguration GetConfiguration ()
    {
      string file = null;
      try
      {
        file = _helper.GetTempFileName();
        _webClient.DownloadFile (_downloadUrl, file);
        return GetConfiguration (file);
      }
      finally
      {
        _helper.Delete (file);
      }
    }
  }
}