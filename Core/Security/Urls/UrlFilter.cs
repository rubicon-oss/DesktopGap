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
using System.Linq;
using System.Text.RegularExpressions;
using DesktopGap.Utilities;

namespace DesktopGap.Security.Urls
{
  public class UrlFilter : IUrlFilter
  {
    private class PredefinedUrlRule : IUrlRule
    {
      private const RegexOptions c_defaultDomainRegexOptions = RegexOptions.Compiled
                                                               | RegexOptions.IgnoreCase
                                                               | RegexOptions.Singleline;

      private const RegexOptions c_defaultPathRegexOptions = RegexOptions.Compiled
                                                             | RegexOptions.IgnoreCase
                                                             | RegexOptions.Singleline
                                                             | RegexOptions.RightToLeft;

      public PredefinedUrlRule (string domain, string path)
      {
        DomainExpression = new Regex (domain, c_defaultDomainRegexOptions);
        PathExpression = new Regex (path, c_defaultPathRegexOptions);
      }

      public Regex DomainExpression { get; private set; }
      public Regex PathExpression { get; private set; }

      public bool IsMatch (string url)
      {
        return DomainExpression.IsMatch (url) && PathExpression.IsMatch (url);
      }
    }

    private readonly IUrlRule[] _allowed;
    private readonly IUrlRule[] _denied;

    public UrlFilter (Uri baseUri, IUrlRules rules)
    {
      ArgumentUtility.CheckNotNull ("baseUri", baseUri);
      ArgumentUtility.CheckNotNull ("rules", rules);


      var escapedBaseUri = Regex.Escape (baseUri.Host);
      var escapedPath = Regex.Escape (baseUri.AbsolutePath) + "[?/].*";

      _allowed = new[] { new PredefinedUrlRule (escapedBaseUri, escapedPath) }.Concat (rules.Allowed).ToArray();
      _denied = rules.Denied.ToArray();
    }

    public bool IsAllowed (string url)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      return _allowed.Any (r => r.IsMatch (url)) && !_denied.Any (r => r.IsMatch (url));
    }
  }
}