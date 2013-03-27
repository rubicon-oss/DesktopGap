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
using System.Text;
using System.Threading.Tasks;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap
{
  public class GlobalEventRouter
  {
    private readonly IExtendedWebBrowser _webBrowser;
    public static event EventHandler<WindowOpenEventArgs> WindowOpen;
    public static event EventHandler<WindowOpenEventArgs> TabOpen;
 
    public event EventHandler<ExtendedDragEventHandlerArgs> DragEnter;
    public event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public event EventHandler<ExtendedDragEventHandlerArgs> DragOver;
    public event EventHandler DragLeave;

    public event EventHandler<EventArgs> ContentReloaded; // Reload event
    public event EventHandler<IExtendedWebBrowser> PageLoaded; // initial page downloaded event

    

    public GlobalEventRouter (IExtendedWebBrowser webBrowser)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);
      
      _webBrowser = webBrowser;
      InitializeEvents();
    }

    private void InitializeEvents ()
    {
      _webBrowser.DragDrop += _webBrowser_DragDrop;
      _webBrowser.DragEnter +=_webBrowser_DragEnter;
      _webBrowser.DragOver += _webBrowser_DragOver;
      _webBrowser.ContentReloaded += _webBrowser_ContentReloaded;
      _webBrowser.WindowOpen += _webBrowser_WindowOpen;
    }

    void _webBrowser_WindowOpen (object sender, WindowOpenEventArgs e)
    {
      throw new NotImplementedException ();
    }

    void _webBrowser_ContentReloaded (object sender, EventArgs e)
    {
      throw new NotImplementedException ();
    }

    void _webBrowser_DragOver (object sender, ExtendedDragEventHandlerArgs e)
    {
      throw new NotImplementedException ();
    }

    private void _webBrowser_DragEnter (object sender, ExtendedDragEventHandlerArgs e)
    {
      throw new NotImplementedException ();
    }

    private void _webBrowser_DragDrop (object sender, ExtendedDragEventHandlerArgs e)
    {
      throw new NotImplementedException();
    }
  }
}
