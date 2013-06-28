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
using System.Security.Permissions;
using System.Windows.Forms;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  public abstract class TridentWebBrowserBase : System.Windows.Forms.WebBrowser
  {
    // Source: http://www.codeproject.com/Articles/13598/Extended-NET-2-0-WebBrowser-Control


    protected bool AutomaticallyRegisterAsDropTarget = false;

    protected bool EnableWebBrowserContextMenu = false;

    protected bool EnableWebBrowserShortcuts = false;


    /// <summary>
    /// Enables/disables shortcuts for editing (ctrl-A, ctrl-X, ctrl-C, ...)
    /// </summary>
    protected bool EnableWebBrowserEditingShortcuts = true;

    /// <summary>
    /// Object for returning the basic scripting interface when the .NET Framework demands it (Application property)
    /// </summary>
    protected IWebBrowser2 AxIWebBrowser2;

    private AxHost.ConnectionPointCookie _cookie;

    protected DWebBrowserEvents2 BrowserEvents { get; set; }

    public IDocHostUIHandler DocumentHostUiHandler { get; private set; }

    protected TridentWebBrowserBase ()
    {
      IsWebBrowserContextMenuEnabled = EnableWebBrowserContextMenu;
    }


    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void AttachInterfaces (object nativeActiveXObject)
    {
      AxIWebBrowser2 = (IWebBrowser2) nativeActiveXObject;

      base.AttachInterfaces (AxIWebBrowser2);
    }

    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachInterfaces ()
    {
      AxIWebBrowser2 = null;
      base.DetachInterfaces();
    }


    public object Application
    {
      get { return AxIWebBrowser2.Application; }
    }

    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void CreateSink ()
    {
      // Make sure to call the base class or the normal events won't fire
      base.CreateSink();
      if (BrowserEvents == null)
        return;

      _cookie = new AxHost.ConnectionPointCookie (ActiveXInstance, BrowserEvents, typeof (DWebBrowserEvents2));
    }


    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachSink ()
    {
      if (_cookie == null)
        return;

      _cookie.Disconnect();
      _cookie = null;
    }


    protected void InstallCustomUIHandler (IDocHostUIHandler docHostUiHandler)
    {
      ArgumentUtility.CheckNotNull ("docHostUiHandler", docHostUiHandler);
      DocumentHostUiHandler = docHostUiHandler;
      AxIWebBrowser2.RegisterAsDropTarget = false;
      var customDoc = (ICustomDoc) AxIWebBrowser2.Document;
      if (customDoc != null)
        customDoc.SetUIHandler (docHostUiHandler);
    }
  }
}