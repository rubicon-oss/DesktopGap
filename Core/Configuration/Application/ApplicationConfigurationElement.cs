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

namespace DesktopGap.Configuration.Application
{
  public class ApplicationConfigurationElement : ConfigurationElement
  {
    [ConfigurationProperty ("name")]
    public string Name
    {
      get { return (string) this["name"]; }
    }

    [ConfigurationProperty ("baseUrl", IsRequired = true)]
    public string BaseUrl
    {
      get { return (string) this["baseUrl"]; }
    }

      public Uri GetBaseUri ()
    {
      Uri uri;
      if (!Uri.TryCreate (BaseUrl, UriKind.RelativeOrAbsolute, out uri))
        throw new InvalidOperationException (string.Format ("Location '{0}' is not a valid URI", Icon));
      return uri;
    }

    [ConfigurationProperty ("homeUrl")]
    public string HomeUrl
    {
      get { return (string) this["homeUrl"]; }
    }

    [ConfigurationProperty ("icon")]
    public string Icon
    {
      get { return (string) this["icon"]; }
    }

    public Uri GetIconUri ()
    {
      Uri uri;
      if (!Uri.TryCreate (Icon, UriKind.RelativeOrAbsolute, out uri))
        throw new InvalidOperationException (string.Format ("Location '{0}' is not a valid URI", Icon));
      return uri;
    }

    [ConfigurationProperty ("alwaysShowUrl")]
    public bool AlwaysShowUrl
    {
      get { return (bool) this["alwaysShowUrl"]; }
    }


    [ConfigurationProperty ("alwaysOpenHomeUrl")]
    public bool AlwaysOpenHomeUrl
    {
      get { return (bool) this["alwaysOpenHomeUrl"]; }
    }

    [ConfigurationProperty ("allowCloseHomeTab")]
    public bool AllowCloseHomeTab
    {
      get { return (bool) this["allowCloseHomeTab"]; }
    }


    [ConfigurationProperty ("maxFrameNestingDepth", DefaultValue = (uint)10)]
    public uint MaxFrameNestingDepth
    {
      get { return (uint) this["maxFrameNestingDepth"]; }
    }

  }
}