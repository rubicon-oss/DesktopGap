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
using DesktopGap.Utilities;

namespace DesktopGap.WebBrowser
{
  public class HtmlElementData
  {
    public string ID { get; private set; }
    public IDictionary<string, string> Attributes { get; private set; }


    public HtmlElementData (string id, IDictionary<string, string> attributes)
    {
      ArgumentUtility.CheckNotNull ("attributes", attributes);

      ID = id;
      Attributes = attributes;
    }

    protected bool Equals (HtmlElementData other)
    {
      return string.Equals (ID, other.ID) && Equals (Attributes, other.Attributes);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        return ((ID != null ? ID.GetHashCode() : 0) * 397) ^ (Attributes != null ? Attributes.GetHashCode() : 0);
      }
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      if (ReferenceEquals (this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return Equals ((HtmlElementData) obj);
    }
  }
}