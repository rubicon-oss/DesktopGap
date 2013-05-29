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
using System.Text.RegularExpressions;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Configuration.Security
{
  public abstract class UrlConfigurationElement : ConfigurationElement
  {
    private const string c_urlWildcard = "*";
    private const string c_regexWildCard = ".*";

    [ConfigurationProperty ("domain", IsRequired = true)]
    public string Domain
    {
      get { return (string) this["domain"]; }
    }

    [ConfigurationProperty ("path")]
    public string Path
    {
      get { return (string) this["path"]; }
    }

    [ConfigurationProperty ("requireSSL", DefaultValue = false)]
    public bool RequireSsl
    {
      get { return (bool) this["requireSSL"]; }
    }

    [ConfigurationProperty ("useRegex", DefaultValue = false)]
    public bool UseRegex
    {
      get { return (bool) this["useRegex"]; }
    }

    public string Key
    {
      get { return string.Format ("({0}) ({1})", Domain, Path); }
    }


    public abstract UrlRule GetRule ();

    protected string TranslateEndWildcard (string url)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      if (!url.EndsWith (c_urlWildcard))
        return Regex.Escape (url);

      var regexPath = url.Remove (url.LastIndexOf (c_urlWildcard, StringComparison.Ordinal));
      return Regex.Escape (regexPath) + c_regexWildCard;
    }

    protected string TranslateStartWildcard (string url)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      if (!url.StartsWith (c_urlWildcard))
        return Regex.Escape (url);

      var regexPath = url.Remove (url.IndexOf (c_urlWildcard, StringComparison.Ordinal));
      return c_regexWildCard + Regex.Escape (regexPath);
    }
  }
}