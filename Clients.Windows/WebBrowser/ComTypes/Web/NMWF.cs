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
  internal enum NWMF : uint
  {
    NWMF_UNLOADING = 0x00000001,
    NWMF_USERINITED = 0x00000002,
    NWMF_FIRST = 0x00000004,
    NWMF_OVERRIDEKEY = 0x00000008,
    NWMF_SHOWHELP = 0x00000010,
    NWMF_HTMLDIALOG = 0x00000020,
    NWMF_FROMDIALOGCHILD = 0x00000040,
    NWMF_USERREQUESTED = 0x00000080,
    NWMF_USERALLOWED = 0x00000100,
    NWMF_FORCEWINDOW = 0x00010000,
    NWMF_FORCETAB = 0x00020000,
    NWMF_SUGGESTWINDOW = 0x00040000,
    NWMF_SUGGESTTAB = 0x00080000,
    NWMF_INACTIVETAB = 0x00100000
  }
}