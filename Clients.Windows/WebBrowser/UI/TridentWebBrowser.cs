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
using DesktopGap.Clients.Windows.WebBrowser.Scripting;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentWebBrowser : TridentWebBrowserBase, IExtendedWebBrowser
  {
    private readonly Func<ApiFacade> _apiFacadeFactory;
    public event EventHandler<IExtendedWebBrowser> PageLoaded;
    public event EventHandler<EventArgs> ContentReloaded;
    public event EventHandler<WindowOpenEventArgs> WindowOpen;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragEnter;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragOver;
    public event EventHandler Focussed;


    private ApiFacade _apiFacade;

    public new event EventHandler DragLeave;

    public TridentWebBrowser (Func<ApiFacade> apiFacadeFactory)
    {
      ArgumentUtility.CheckNotNull ("apiFacadeFactory", apiFacadeFactory);
      _apiFacadeFactory = apiFacadeFactory;
      _BrowserEvents = new DesktopGapWebBrowserEvents (this);

      Navigate ("about:blank"); // bootstrap
      BrowserMode = TridentWebBrowserMode.IE10;
      InstallCustomUIHandler (new DesktopGapDocumentUIHandler (this));

      DocumentCompleted += TridentWebBrowser_DocumentCompleted;
    }

    protected override void Dispose (bool disposing)
    {
      PageLoaded = null;
      WindowOpen = null;
      DragDrop = null;
      DragLeave = null;
      DragOver = null;
      DragEnter = null;

      DisposeObjectForScripting();
      base.Dispose (disposing);
    }

    public string Title
    {
      get { return Document == null ? String.Empty : Document.Title; }
    }

    //
    // NAVIGATION EVENTS
    // 
    public void OnNewWindow (WindowOpenEventArgs eventArgs)
    {
      if (WindowOpen != null)
        WindowOpen (this, eventArgs);
    }

    public void OnBeforeNavigate (WindowOpenEventArgs navigationEventArgs)
    {
      InitializeScripting();
    }

    public void OnFocussed (object sender, EventArgs eventArgs)
    {
      if (Focussed != null)
        Focussed (sender, eventArgs);
    }

    //
    // INTERACTION EVENTS
    // 

    public void OnDragEnter (ExtendedDragEventHandlerArgs e)
    {
      e.Current = ElementAt (e.X, e.Y);
      e.Effect = DragDropEffects.Copy;

      if (DragEnter != null)
        DragEnter (this, e);

      e.Handled = true;
    }

    public void OnDragDrop (ExtendedDragEventHandlerArgs e)
    {
      e.Current = ElementAt (e.X, e.Y);
      if (DragDrop != null)
        DragDrop (this, e);

      e.Handled = true;
    }

    public new void OnDragLeave (EventArgs e)
    {
      if (DragLeave != null)
        DragLeave (this, e);
    }

    public void OnDragOver (ExtendedDragEventHandlerArgs e)
    {
      e.Current = ElementAt (e.X, e.Y);
      e.Effect = DragDropEffects.Copy;

      if (DragOver != null)
        DragOver (this, e);
      e.Handled = true;
    }

    public void OnBrowserKeyDown (TridentWebBrowser extendedTridentWebBrowser, KeyEventArgs keyEventArgs)
    {
      var keyCode = keyEventArgs.KeyCode | ModifierKeys;

      if (!Enum.IsDefined (typeof (Shortcut), (Shortcut) keyCode))
        return;

      switch (keyCode)
      {
        case Keys.Control | Keys.A: // Select All
        case Keys.Control | Keys.C: // Copy
        case Keys.Control | Keys.V: // Paste
        case Keys.Control | Keys.X: // Cut
        case Keys.Delete: // Delete
          keyEventArgs.Handled = !_enableWebBrowserEditingShortcuts;
          break;
        default:
          keyEventArgs.Handled = !_enableWebBrowserShortcuts;
          break;
      }
    }


    //
    // OTHER METHODS & EVENTS
    // 

    private void InitializeScripting ()
    {
      if (ObjectForScripting != null)
        return;

      _apiFacade = _apiFacadeFactory();

      ObjectForScripting = _apiFacade;
    }

    private void DisposeObjectForScripting ()
    {
      var apiFacade = (ApiFacade) ObjectForScripting;

      if (apiFacade == null)
        return;

      ObjectForScripting = null;
      apiFacade.Dispose();
    }

    private HtmlElement ElementAt (int x, int y)
    {
      if (Document == null)
        return null;

      var locationOnScreen = PointToScreen (Location);
      return Document.GetElementFromPoint (new Point (x - locationOnScreen.X, y - locationOnScreen.Y));
    }


    private void AddDocuments (HtmlWindow window)
    {
      if (window.Frames != null)
        foreach (HtmlWindow w in window.Frames)
          AddDocuments (w);

      _apiFacade.AddDocument (window.Document);
    }

    private void TridentWebBrowser_DocumentCompleted (object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      if (e.Url.AbsolutePath != Url.AbsolutePath)
        return;

      if (PageLoaded != null)
        PageLoaded (this, (IExtendedWebBrowser) sender);

      if (_apiFacade != null && Document != null && Document.Window != null)
        AddDocuments (Document.Window);
    }

    //
    //
    //
  }

  #region keep for later 

  //TODO restructure
  //private bool MayDrop (IExtendedWebBrowser browser, DragEventArgs e)
  //{
  //  var browserControl = (Control) browser;
  //  var extendedBrowser = (TridentWebBrowser) browser;
  //  var doc = extendedBrowser.Document;
  //  var locationOnScreen = browserControl.PointToScreen (browserControl.Location);
  //  var elementAtPoint = doc.GetElementFromPoint (new Point (e.X - locationOnScreen.X, e.Y - locationOnScreen.Y));

  //  var currentElement = elementAtPoint;
  //  var isDropTarget = false;
  //  while (currentElement != null && ! Boolean.TryParse (currentElement.GetAttribute ("droptarget"), out isDropTarget))
  //    currentElement = currentElement.Parent;

  //  return isDropTarget;
  //} 

  //  public void OnDragDrop (ExtendedDragEventHandlerArgs e)
  //{
  //  e.Current = ElementAt (e.X, e.Y);
  //  var ok = MayDrop (this, e);
  //  e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;
  //  Output ("DragDrop: " + ok.ToString());
  //  if (DragDrop != null)
  //    DragDrop (this, e);

  //  e.Handled = true;
  //}

  #endregion
}