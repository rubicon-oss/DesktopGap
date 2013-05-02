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
using System.Configuration;
using DesktopGap.Utilities;

namespace DesktopGap.Configuration
{
  public abstract class DesktopGapConfigurationProviderBase<TConfiguration> : IDesktopGapConfigurationProvider<TConfiguration>
      where TConfiguration : ConfigurationSection
  {
    private readonly string _sectionName;

    protected DesktopGapConfigurationProviderBase (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);
      _sectionName = sectionName;
    }

    public abstract TConfiguration GetConfiguration ();

    protected TConfiguration GetConfiguration (string configFileName)
    {
      var exeConfigurationFileMap = new ExeConfigurationFileMap();
      exeConfigurationFileMap.ExeConfigFilename = configFileName;
      var configuration = ConfigurationManager.OpenMappedExeConfiguration (exeConfigurationFileMap, ConfigurationUserLevel.None);
      return (TConfiguration) configuration.GetSection (_sectionName);
    }
  }
}