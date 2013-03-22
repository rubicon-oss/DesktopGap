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
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.AddIns.Events
{
  public static class SystemEventHub
  {
    public static event EventHandler<IExtendedWebBrowser> PageLoaded;
    public static event EventHandler<WindowOpenEventArgs> WindowOpen;

    public static event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public static event EventHandler DragLeave;


    public static void AddWebBrowser (IExtendedWebBrowser browser)
    {
      browser.PageLoaded += (s, e) =>
                            {
                              if (PageLoaded != null)
                                PageLoaded (s, e);
                            };
      browser.DragDrop += (s, e) =>
                          {
                            if (DragDrop != null)
                              DragDrop (s, e);
                          };
      browser.DragLeave += (s, e) =>
                           {
                             if (DragLeave != null)
                               DragLeave (s, e);
                           };
      browser.WindowOpen += (s, e) =>
                            {
                              if (WindowOpen != null)
                                WindowOpen (s, e);
                            };
    }
  }
}