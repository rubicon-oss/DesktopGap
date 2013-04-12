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

namespace DesktopGap
{
  public struct HtmlDocumentHandle
  {
    private readonly Guid _guid;

    public HtmlDocumentHandle (Guid guid)
    {
      _guid = guid;
    }

    public override string ToString ()
    {
      return _guid.ToString();
    }

    public override bool Equals (object obj)
    {
      if (obj is Guid)
        return _guid.Equals ((Guid) obj);
      else
        return _guid.Equals (((HtmlDocumentHandle) obj)._guid);
    }

    public override int GetHashCode ()
    {
      return _guid.GetHashCode();
    }

    public static bool operator != (HtmlDocumentHandle a, HtmlDocumentHandle b)
    {
      return !a.Equals (b);
    }

    public static bool operator == (HtmlDocumentHandle a, HtmlDocumentHandle b)
    {
      return a.Equals (b);
    }
  }

  public struct ResourceHandle
  {
    private readonly Guid _guid;

    public ResourceHandle (Guid guid)
    {
      _guid = guid;
    }

    public override string ToString ()
    {
      return _guid.ToString();
    }

    public override bool Equals (object obj)
    {
      if (obj is Guid)
        return _guid.Equals ((Guid) obj);
      else
        return _guid.Equals (((ResourceHandle) obj)._guid);
    }

    public override int GetHashCode ()
    {
      return _guid.GetHashCode();
    }

    public static bool operator != (ResourceHandle a, ResourceHandle b)
    {
      return !a.Equals (b);
    }

    public static bool operator == (ResourceHandle a, ResourceHandle b)
    {
      return a.Equals (b);
    }
  }
}