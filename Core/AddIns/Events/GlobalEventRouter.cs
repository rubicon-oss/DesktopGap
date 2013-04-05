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
using System.ComponentModel.Composition;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.AddIns.Events
{
  [Export (typeof (IGlobalEventRouter))]
  public class GlobalEventRouter : IGlobalEventRouter, IGlobalEventSubscriber
  {
    public event EventHandler<WindowOpenEventArgs> WindowOpen;
    public event EventHandler<WindowOpenEventArgs> TabOpen;

    public event EventHandler<ExtendedDragEventHandlerArgs> DragEnter;
    public event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public event EventHandler<ExtendedDragEventHandlerArgs> DragOver;
    public event EventHandler DragLeave;

    public event EventHandler<EventArgs> ContentReloaded; // Reload event
    public event EventHandler<IExtendedWebBrowser> PageLoaded; // initial page downloaded event

    public void SubscribeTo (IExtendedWebBrowser webBrowser)
    {
      webBrowser.DragDrop += (s, e) =>
                             {
                               if (DragDrop != null)
                                 DragDrop (s, e);
                             };

      webBrowser.DragEnter += (s, e) =>
                              {
                                if (DragEnter != null)
                                  DragEnter (s, e);
                              };

      webBrowser.DragOver += (s, e) =>
                             {
                               if (DragOver != null)
                                 DragOver (s, e);
                             };


      webBrowser.ContentReloaded += (s, e) =>
                                    {
                                      if (ContentReloaded != null)
                                        ContentReloaded (s, e);
                                    };
      webBrowser.WindowOpen += (s, e) =>
                               {
                                 if (WindowOpen != null)
                                   WindowOpen (s, e);
                               };
    }

    public void SubscribeTo (IWebBrowserWindow webBrowserWindow)
    {
    }
  }
}