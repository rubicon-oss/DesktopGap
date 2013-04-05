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

namespace DesktopGap.Clients.Windows.WebBrowser
{
  /// <summary>
  /// The enumatrion of the various BrowserModes for the BrowserEmulation Setting of the WebBrowser Control/IE
  /// 
  /// For more info see:
  /// http://blogs.msdn.com/b/ie/archive/2009/03/10/more-ie8-extensibility-improvements.aspx
  /// http://msdn.microsoft.com/en-us/library/ee330730%28v=vs.85%29.aspx#browser_emulation
  /// </summary>
  public enum WebBrowserMode
  {
    // ReSharper disable InconsistentNaming
    ForcedIE10 = 10001, // Forced = ignore !DOCTYPE
    IE10 = 10000,
    ForcedIE9 = 9999,
    IE9 = 9000,
    ForcedIE8 = 8888,
    IE8 = 8000,
    IE7 = 7000
    // ReSharper restore InconsistentNaming
  }
}