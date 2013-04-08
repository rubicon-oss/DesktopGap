﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.Script.Serialization;

namespace DesktopGap.AddIns.Events
{
  //Json.NET Attribute
  [PermissionSet (SecurityAction.Demand, Name = "FullTrust")]
  [ComVisible (true)]
  public class JsonData
  {
    public string EventId;
  }

  public sealed class ScriptEventArgs : EventArgs
  {
    public string Function { get; set; }

    public JsonData ScriptArgs { get; set; }

    public string Serialize ()
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize (ScriptArgs);
    }
  }

  public class FileScriptArgs : JsonData
  {
    public string Path { get; private set; }

    public FileScriptArgs (string path)
    {
      if (path == null)
        throw new ArgumentNullException ("path");
      Path = path;
    }
  }
}