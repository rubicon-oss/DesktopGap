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
using System.Security.Permissions;
using System.Windows.Forms;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  /// <summary>
  /// Base implementation for adding customized events and interfaces for advanced interaction with the .NET WebBrowser control based on the Trident engine.
  /// </summary>
  public abstract class TridentWebBrowserBase : System.Windows.Forms.WebBrowser
  {
    // Source: http://www.codeproject.com/Articles/13598/Extended-NET-2-0-WebBrowser-Control

    /// <summary>
    /// Avoid default registration as IDropTarget by the WebBrowser.
    /// </summary>
    protected bool _automaticallyRegisterAsDropTarget = false;

    protected bool _enableWebBrowserContextMenu = false;

    /// <summary>
    /// Object for returning the basic scripting interface when the .NET Framework demands it (Application property)
    /// </summary>
    private IWebBrowser2 _axIWebBrowser2;

    /// <summary>
    /// Connection point for attacing custom interfaces to the WebBrowser
    /// </summary>
    private AxHost.ConnectionPointCookie _cookie;

    /// <summary>
    /// Customized events
    /// </summary>
    protected DWebBrowserEvents2 _BrowserEvents { get; set; }

    private static readonly string[] s_validElements = new string[] { "text", "password" };
    private bool _controlPressed;
    protected readonly TridentFeatures _features;

    protected TridentWebBrowserBase ()
    {
      IsWebBrowserContextMenuEnabled = _enableWebBrowserContextMenu;
      WebBrowserShortcutsEnabled = false;
      _features = new TridentFeatures();
    }

    public bool IsGPUAccelerated
    {
      get { return _features.GpuAcceleration; }
      set { _features.GpuAcceleration = value; }
    }

    public WebBrowserMode BrowserMode
    {
      get { return _features.BrowserEmulationMode; }
      set { _features.BrowserEmulationMode = value; }
    }

    /// <summary>
    /// Retrieve the _axIWebBrowser2 implementation from the .NET WebBrowser. 
    /// </summary>
    /// <param name="nativeActiveXObject"></param>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void AttachInterfaces (object nativeActiveXObject)
    {
      _axIWebBrowser2 = (IWebBrowser2) nativeActiveXObject;

      base.AttachInterfaces (nativeActiveXObject);
    }

    /// <summary>
    /// Clean up properly after the interface is detached.
    /// </summary>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachInterfaces ()
    {
      _axIWebBrowser2 = null;
      base.DetachInterfaces();
    }

    /// <summary>
    /// Property that offers the scripting interface (required on connecting any other interface)
    /// </summary>
    public object Application
    {
      get { return _axIWebBrowser2.Application; }
    }

    /// <summary>
    /// This method will be called to give
    /// you a chance to create your own event sink
    /// </summary>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void CreateSink ()
    {
      // Make sure to call the base class or the normal events won't fire
      base.CreateSink();
      if (_BrowserEvents == null)
        return;

      _cookie = new AxHost.ConnectionPointCookie (ActiveXInstance, _BrowserEvents, typeof (DWebBrowserEvents2));
    }

    /// <summary>
    /// Detaches the event sink
    /// </summary>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachSink ()
    {
      if (_cookie == null)
        return;

      _cookie.Disconnect();
      _cookie = null;
    }

    /// <summary>
    /// Whether to register the WebBrowser's IDropTarget. 
    /// </summary>
    protected bool RegisterAsDropTarget
    {
      get { return _axIWebBrowser2 != null && _axIWebBrowser2.RegisterAsDropTarget; }
      set
      {
        if (_axIWebBrowser2 == null)
          return;

        _axIWebBrowser2.RegisterAsDropTarget = value;
      }
    }

    /// <summary>
    /// Use a custom IDocHostUIHandler instead of the default. Used for intercepting keyboard and mouse events.
    /// </summary>
    /// <param name="desktopGapDocumentUiHandler">The custom handler.</param>
    protected void InstallCustomUIHandler (IDocHostUIHandler desktopGapDocumentUiHandler)
    {
      ArgumentUtility.CheckNotNull ("desktopGapDocumentUiHandler", desktopGapDocumentUiHandler);
      RegisterAsDropTarget = false;

      var customDoc = (ICustomDoc) _axIWebBrowser2.Document;
      
      customDoc.SetUIHandler (desktopGapDocumentUiHandler);
    }

    /// <summary>
    /// Handler for KeyDown events to surpress default handling of well-known keyboard shortcuts.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal void OnBrowserKeyDown (object sender, KeyEventArgs e)
    {
      var browser = (TridentWebBrowserBase) sender;

      switch (e.KeyCode)
      {
        case Keys.F5:
        case Keys.BrowserStop:
        case Keys.BrowserSearch:
        case Keys.BrowserRefresh:
        case Keys.BrowserHome:
        case Keys.BrowserForward:
        case Keys.BrowserFavorites:
        case Keys.BrowserBack:
          e.Handled = true;
          break;

        case Keys.Back:
          if (browser.Document != null && browser.Document.ActiveElement != null)
          {
            bool isReadOnly;

            var activeElement = browser.Document.ActiveElement;
            Boolean.TryParse (activeElement.GetAttribute ("readonly"), out isReadOnly);

            var tagname = activeElement.TagName.ToLower();
            var type = activeElement.GetAttribute ("type").ToLower();

            if (((tagname == "input" && Array.IndexOf (s_validElements, type) > 0) || tagname == "textarea")
                && !isReadOnly)
              return;
          }

          e.Handled = true;
          break;

        case Keys.ControlKey:
          _controlPressed = true;
          break;

        case Keys.R:
        case Keys.P:
        case Keys.Print:
          e.Handled = _controlPressed || e.KeyCode == Keys.Print;
          break;

        default:
          _controlPressed = false;
          break;
      }
    }
  }
}