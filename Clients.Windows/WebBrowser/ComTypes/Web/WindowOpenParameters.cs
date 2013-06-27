// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web
{
  /// <summary>
  /// From http://msdn.microsoft.com/en-us/library/ie/bb762518%28v=vs.85%29.aspx
  /// </summary>
  [Flags]
  internal enum WindowOpenParameters
  {
    None = 0x0,
    Unloading = 0x00000001,
    UserInitiated = 0x00000002,
    First = 0x00000004,
    OverrideKey = 0x00000008,
    ShowHelp = 0x00000010,
    HtmlDialog = 0x00000020,
    FromDialogChild = 0x00000040,
    UserRequested = 0x00000080,
    UserAllowed = 0x00000100,
    ForceWindow = 0x00010000,
    ForceTab = 0x00020000,
    SuggestWindow = 0x00040000,
    SuggestTab = 0x00080000,
    InactiveTab = 0x00100000
  }
}