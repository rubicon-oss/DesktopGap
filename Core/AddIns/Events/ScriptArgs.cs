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

namespace DesktopGap.AddIns.Events
{
  //Json.NET Attribute
  public class JsonData
  {
    public string EventId { get; set; }
  }

  public class ScriptEventArgs : EventArgs
  {
    public string Function { get; set; }

    public JsonData ScriptArgs { get; set; }
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

    public override string ToString ()
    {
      return String.Format ("{{'EventId':'{0}', 'Path':'{1}' }}", EventId, Path);
    }
  }
}