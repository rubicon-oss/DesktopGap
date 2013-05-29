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
using DesktopGap.Security.Urls;

namespace DesktopGap.Configuration.Security
{
  public class UrlConfigurationElementCollection : ConfigurationElementCollection, IEnumerable<UrlRule>
  {
    private const string c_addElementName = "add";
    private const string c_removeElementName = "remove";

    public UrlConfigurationElement this [int index]
    {
      get { return (UrlConfigurationElement) BaseGet (index); }
    }

    protected override ConfigurationElement CreateNewElement ()
    {
      throw new NotSupportedException ("Elements of this collection can only be created from an element name.");
    }

    protected override ConfigurationElement CreateNewElement (string elementName)
    {
      switch (elementName)
      {
        case c_addElementName:
          return new AddUrlConfigurationElement();
        case c_removeElementName:
          return new RemoveUrlConfigurationElement();
        default:
          throw new NotSupportedException (
              string.Format ("Only elements called '{0}' or '{1}' are supported.", c_addElementName, c_removeElementName));
      }
    }

    protected override bool IsElementName (string elementName)
    {
      return elementName == c_addElementName || elementName == c_removeElementName;
    }

    public override ConfigurationElementCollectionType CollectionType
    {
      get { return ConfigurationElementCollectionType.BasicMap; }
    }


    protected override object GetElementKey (ConfigurationElement element)
    {
      return ((UrlConfigurationElement) element).Key;
    }

    public new IEnumerator<UrlRule> GetEnumerator ()
    {
      for (var i = 0; i < Count; i++)
        yield return this[i].GetRule();
    }
  }
}