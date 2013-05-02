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

namespace DesktopGap.Configuration.Security
{
  public class UrlConfigurationElement : ConfigurationElement, IUrlRule, IRuleIdentification
  {
    private Regex _pathExpression;
    private Regex _domainExpression;

    private const RegexOptions c_defaultDomainRegexOptions = RegexOptions.Compiled
                                                             | RegexOptions.IgnoreCase
                                                             | RegexOptions.Singleline;

    private const RegexOptions c_defaultPathRegexOptions = RegexOptions.Compiled
                                                           | RegexOptions.IgnoreCase
                                                           | RegexOptions.Singleline
                                                           | RegexOptions.RightToLeft;


    [ConfigurationProperty ("domain", IsRequired = true)]
    public string Domain
    {
      get { return (string) this["domain"]; }
    }


    [ConfigurationProperty ("path")]
    public string Path
    {
      get { return this["path"] as string; }
    }

    public string Key
    {
      get { return string.Format ("({0}) ({1})", Domain, Path); }
    }

    public Regex DomainExpression
    {
      get { return _domainExpression ?? (_domainExpression = new Regex (Domain, c_defaultDomainRegexOptions)); }
    }

    public Regex PathExpression
    {
      get { return _pathExpression ?? (_pathExpression = new Regex (Path, c_defaultPathRegexOptions)); }
    }

    public bool IsMatch (string url)
    {
      return DomainExpression.IsMatch (url) && PathExpression.IsMatch (url);
    }
  }
}