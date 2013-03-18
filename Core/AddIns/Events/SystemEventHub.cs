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
using DesktopGap.OleLibraryDependencies;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns.Events
{
  public static class SystemEventHub
  {
    public static event Action<IExtendedWebBrowser> PageLoaded;
    public static event Action<WindowOpenEventArgs> WindowOpen;

    public static event Action<ExtendedDragEventHandlerArgs> DragDrop;
    public static event Action<ExtendedDragEventHandlerArgs> DragLeave;


    public static void AddWebBrowser (IExtendedWebBrowser browser)
    {
      browser.PageLoaded += (e) =>
                            {
                              if (PageLoaded != null)
                                PageLoaded (e);
                            };
      browser.DragDrop += (e) =>
                          {
                            if (DragDrop != null)
                              DragDrop (e);
                          };
      browser.DragLeave += (e) =>
                           {
                             if (DragLeave != null)
                               DragLeave (e);
                           };
      browser.WindowOpen += (e) =>
                            {
                              if (WindowOpen != null)
                                WindowOpen (e);
                            };
    }
  }
}