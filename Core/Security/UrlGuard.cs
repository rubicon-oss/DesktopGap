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
using System.Linq;
using System.Text.RegularExpressions;
using DesktopGap.Utilities;

namespace DesktopGap.Security
{
  public class UrlGuard : IGuard
  {
    private class Rule : IEquatable<Rule>
    {
      public Rule (Regex domain, Regex path, bool isAllowed = true)
      {
        ArgumentUtility.CheckNotNull ("path", path);
        ArgumentUtility.CheckNotNull ("domain", domain);

        if (!path.RightToLeft)
          throw new ArgumentException ("'Path' regular expression needs to be RightToLeft");

        _domain = domain;
        _path = path;
        _isAllowed = isAllowed;
      }

      private readonly Regex _domain;
      private readonly Regex _path;
      private readonly bool _isAllowed;

      public bool Match (string url)
      {
        return _domain.IsMatch (url) && _path.IsMatch (url) && _isAllowed;
      }

      public bool Equals (Rule other)
      {
        if (ReferenceEquals (null, other))
          return false;
        if (ReferenceEquals (this, other))
          return true;
        return Equals (_domain.ToString(), other._domain.ToString()) && Equals (_path.ToString(), other._path.ToString());
      }

      public override bool Equals (object obj)
      {
        return !ReferenceEquals (null, obj)
               && (ReferenceEquals (this, obj)
                   || obj.GetType() == GetType()
                   && Equals ((Rule) obj));
      }

      public override int GetHashCode ()
      {
        unchecked
        {
          return _domain.ToString().GetHashCode() * 397 ^ _path.ToString().GetHashCode();
        }
      }
    }

    
    private const RegexOptions c_defaultDomainRegexOptions = RegexOptions.Compiled
                                                             | RegexOptions.IgnoreCase
                                                             | RegexOptions.Singleline;

    private const RegexOptions c_defaultPathRegexOptions = RegexOptions.Compiled
                                                           | RegexOptions.IgnoreCase
                                                           | RegexOptions.Singleline
                                                           | RegexOptions.RightToLeft;

    private readonly ISet<Rule> _allowed = new HashSet<Rule>();

    public bool IsAllowed (string url)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      return _allowed.Any (r => r.Match (url));
    }

    public void ChangeRule (string domainExpression, string pathExpression, bool isAllowed)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("domainExpression", domainExpression);
      ArgumentUtility.CheckNotNull ("pathExpression", pathExpression);

      var rule = new Rule (
          new Regex (domainExpression, c_defaultDomainRegexOptions),
          new Regex (pathExpression, c_defaultPathRegexOptions));

      if (_allowed.Contains (rule) && isAllowed)
        throw new InvalidOperationException ("Rule is already present.");

      if (isAllowed)
        _allowed.Add (rule);
      else
        _allowed.Remove (rule);
    }
  }
}