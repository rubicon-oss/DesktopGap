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
using System.Configuration;
using DesktopGap.Security.AddIns;
using DesktopGap.Security.Urls;

namespace DesktopGap.Configuration.Security
{
  public class SecurityConfigurationElement : ConfigurationElement
  {
    [ConfigurationProperty ("enableResourceFilter", DefaultValue = false)]
    public bool EnableResourceFilter
    {
      get { return (bool) this["enableResourceFilter"]; }
    }

    [ConfigurationProperty ("startupUrls")]
    public UrlConfigurationElementCollection StartupUrls
    {
      get { return (UrlConfigurationElementCollection) this["startupUrls"]; }
    }

    public IEnumerable<PositiveUrlRule> StartupUrlRules
    {
      get { return StartupUrls; }
    }

    [ConfigurationProperty ("allowedNonApplicationUrls")]
    public UrlConfigurationElementCollection AllowedNonApplicationUrls
    {
      get { return (UrlConfigurationElementCollection) this["allowedNonApplicationUrls"]; }
    }

    public IEnumerable<PositiveUrlRule> NonApplicationUrlRules
    {
      get { return AllowedNonApplicationUrls; }
    }

    [ConfigurationProperty ("applicationUrls")]
    public UrlConfigurationElementCollection ApplicationUrls
    {
      get { return (UrlConfigurationElementCollection) this["applicationUrls"]; }
    }

    public IEnumerable<PositiveUrlRule> ApplicationUrlRules
    {
      get { return ApplicationUrls; }
    }

    [ConfigurationProperty ("addIns")]
    public AddInConfigurationElementCollection AddIns
    {
      get { return (AddInConfigurationElementCollection) this["addIns"]; }
    }

    public IEnumerable<AddInRule> AddInRules
    {
      get { return AddIns; }
    }
  }
}