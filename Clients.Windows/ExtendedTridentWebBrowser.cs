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
using System.Drawing;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;


namespace DesktopGap.Clients.Windows
{
  public class TridentWebBrowser : TridentWebBrowserBase, IExtendedWebBrowser, IDropTarget
  {
    public event EventHandler<IExtendedWebBrowser> PageLoaded;
    public event EventHandler<EventArgs> ContentReloaded;
    public event EventHandler<WindowOpenEventArgs> WindowOpen;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragLeave;

    /// <summary>
    /// 
    /// </summary>
    public event Action<string> Output;


    public TridentWebBrowser (APIFacade apiFacade)
    {
      ObjectForScripting = apiFacade;
      this._BrowserEvents = new DesktopGapBrowserEvents (this);
      Navigate ("about:blank");
      InstallCustomUIHandler (new DesktopGapDocumentUIHandler (this));
      apiFacade.EventManager.EventFired += OnEventFired;
    }

    protected override void Dispose (bool disposing)
    {
      PageLoaded = null;
      WindowOpen = null;
      DragDrop = null;
      DragLeave = null;

      base.Dispose (disposing);
    }

    public string Title
    {
      get { return Document == null ? String.Empty : Document.Title; }
    }

    public void OnLoadFinished ()
    {
      if (PageLoaded != null)
        PageLoaded (this, this);
    }

    public void OnNewWindow (WindowOpenEventArgs eventArgs)
    {
      if (WindowOpen != null)
        WindowOpen (this, eventArgs);
    }

    //TODO restructure
    private bool MayDrop (IExtendedWebBrowser browser, DragEventArgs e)
    {
      var browserControl = (Control) browser;
      var extendedBrowser = (TridentWebBrowser) browser;
      var doc = extendedBrowser.Document;
      var locationOnScreen = browserControl.PointToScreen (browserControl.Location);
      var elementAtPoint = doc.GetElementFromPoint (new Point (e.X - locationOnScreen.X, e.Y - locationOnScreen.Y));

      var currentElement = elementAtPoint;
      var isDropTarget = false;
      while (currentElement != null && ! Boolean.TryParse (currentElement.GetAttribute ("droptarget"), out isDropTarget))
      {
        currentElement = currentElement.Parent;
      }

      return isDropTarget;
    }


    public void OnDragEnter (ExtendedDragEventHandlerArgs e)
    {
      var ok = MayDrop (this, e);
      e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;
      e.Handled = true;
    }

    public void OnDragDrop (ExtendedDragEventHandlerArgs e)
    {
      var ok = MayDrop (this, e);
      e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;
      Output ("DragDrop: " + ok.ToString());
      DragDrop (this, e);

      e.Handled = true;
    }


    public new void OnDragLeave (EventArgs e)
    {
      //Output ("DragLeave" + MayDrop (this, e).ToString());
    }

    public void OnDragOver (ExtendedDragEventHandlerArgs e)
    {
      var ok = MayDrop (this, e);
      e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;
      e.Handled = true;
      // Output ("DragOver" + MayDrop (this, e).ToString());
    }

    private void OnEventFired (object sender, ScriptEventArgs args)
    {
      Document.InvokeScript (args.Function, new object[] { args.ScriptArgs });
    }
  }
}