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

namespace DesktopGap.Configuration.Security
{
  public class AddInPermissionConfigurationElement : ConfigurationElement, IAddInRules
  {
    [ConfigurationProperty ("allow")]
    public AddInConfigurationElementCollection Allow
    {
      get { return (AddInConfigurationElementCollection) this["allow"]; }
    }

    [ConfigurationProperty ("deny")]
    public AddInConfigurationElementCollection Deny
    {
      get { return (AddInConfigurationElementCollection) this["deny"]; }
    }

    public IEnumerable<IAddInRule> Allowed
    {
      get { return Allow; }
    }

    public IEnumerable<IAddInRule> Denied
    {
      get { return Deny; }
    }
  }
}