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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.Clients.Windows.WebBrowser.Scripting;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentWebBrowser : TridentWebBrowserBase, IExtendedWebBrowser, IScriptingHost
  {
    private class DocumentIdentifier
    {
      private const string c_getDocumentIdentification = "dg_retrieveID";
      private const string c_addDocumentIdentification = "dg_assignID";

      public static HtmlDocumentHandle GetOrCreateDocumentHandle (HtmlDocument htmlDocument)
      {
        var guidObj = htmlDocument.InvokeScript (c_getDocumentIdentification);

        var docID = guidObj != null ? new HtmlDocumentHandle (Guid.Parse (guidObj.ToString())) : new HtmlDocumentHandle (Guid.NewGuid());

        htmlDocument.InvokeScript (c_addDocumentIdentification, new object[] { docID.ToString() });


        return docID;
      }
    }

    private class FrameIterator
    {
      private readonly HtmlDocument _htmlDocument;

      public FrameIterator (HtmlDocument htmlDocument)
      {
        ArgumentUtility.CheckNotNull ("htmlDocument", htmlDocument);

        _htmlDocument = htmlDocument;
      }

      private IEnumerable<HtmlWindow> WalkFrame (HtmlWindow frame)
      {
        if (frame == null) //|| frame.Url == null || frame.Url.AbsoluteUri == c_blankSite)
          yield break;

        yield return frame;

        if (frame.Frames != null)
          foreach (var subframe in frame.Frames.Cast<HtmlWindow>().SelectMany (WalkFrame))
            yield return subframe;
      }

      public IEnumerable<HtmlWindow> GetFrames ()
      {
        return WalkFrame (_htmlDocument.Window);
      }
    }

    private const DragDropEffects c_defaulEffect = DragDropEffects.Move;
    private const string c_blankSite = "about:blank";

    private static readonly string[] s_attributes = new[] { "dg_droptarget", "dg_dropcondition" };


    public event EventHandler<IExtendedWebBrowser> PageLoadFinished;
    public event EventHandler<NavigationEventArgs> AfterNavigate;
    public event EventHandler<WindowOpenEventArgs> WindowOpen;
    public event EventHandler<NavigationEventArgs> BeforeNavigate;

    public new event EventHandler<ExtendedDragEventHandlerArgs> DragEnter;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragOver;
    public new event EventHandler DragLeave;

    public event EventHandler<int> WindowSetHeight;
    public event EventHandler<int> WindowSetLeft;
    public event EventHandler<int> WindowSetTop;
    public event EventHandler<int> WindowSetWidth;

    private ISet<HtmlDocumentHandle> _activeHandles;

    private readonly IHtmlDocumentHandleRegistry _documentHandleRegistry;
    private readonly ISubscriptionHandler _subscriptionHandler;


    public TridentWebBrowser (
        IHtmlDocumentHandleRegistry documentHandleRegistry, ISubscriptionHandler subscriptionHandler)
    {
      ArgumentUtility.CheckNotNull ("documentHandleRegistry", documentHandleRegistry);
      ArgumentUtility.CheckNotNull ("subscriptionHandler", subscriptionHandler);

      _BrowserEvents = new DesktopGapWebBrowserEvents (this);

      Navigate (c_blankSite); // bootstrap
      ObjectForScripting = new ApiFacade (documentHandleRegistry);
      _documentHandleRegistry = documentHandleRegistry;
      _subscriptionHandler = subscriptionHandler;

      BrowserMode = TridentWebBrowserMode.Edge;
      InstallCustomUIHandler (new DesktopGapDocumentUIHandler (this));

      DocumentCompleted += TridentWebBrowser_DocumentCompleted;
    }


    protected override void Dispose (bool disposing)
    {
      BeforeNavigate = null;
      AfterNavigate = null;
      PageLoadFinished = null;

      WindowOpen = null;

      DragDrop = null;
      DragLeave = null;
      DragOver = null;
      DragEnter = null;

      ObjectForScripting = null;

      WindowSetHeight = null;
      WindowSetLeft = null;
      WindowSetTop = null;
      WindowSetWidth = null;

      if (_activeHandles != null)
        _activeHandles.Clear();
      _activeHandles = null;

      base.Dispose (disposing);
    }

    public string Title
    {
      get { return Document == null ? String.Empty : Document.Title; }
    }


    private new ApiFacade ObjectForScripting
    {
      get { return (ApiFacade) base.ObjectForScripting; }
      set { base.ObjectForScripting = value; }
    }

    //
    // WINDOW EVENTS
    //

    public void OnWindowSetHeight (int height)
    {
      ArgumentUtility.CheckNotNull ("height", height);

      if (WindowSetHeight != null)
        WindowSetHeight (this, height);
    }

    public void OnWindowSetLeft (int left)
    {
      ArgumentUtility.CheckNotNull ("left", left);

      if (WindowSetLeft != null)
        WindowSetLeft (this, left);
    }

    public void OnWindowSetTop (int top)
    {
      ArgumentUtility.CheckNotNull ("top", top);

      if (WindowSetTop != null)
        WindowSetTop (this, top);
    }

    public void OnWindowSetWidth (int width)
    {
      ArgumentUtility.CheckNotNull ("width", width);

      if (WindowSetWidth != null)
        WindowSetWidth (this, width);
    }

    //
    // NAVIGATION EVENTS
    // 
    public void OnNewWindow (WindowOpenEventArgs eventArgs)
    {
      ArgumentUtility.CheckNotNull ("eventArgs", eventArgs);

      if (WindowOpen != null)
        WindowOpen (this, eventArgs);
    }

    public void OnBeforeNavigate (NavigationEventArgs navigationEventArgs)
    {
      ArgumentUtility.CheckNotNull ("navigationEventArgs", navigationEventArgs);

      if (navigationEventArgs.URL == c_blankSite)
        return;

      _startMode = navigationEventArgs.StartMode;

      if (BeforeNavigate != null)
        BeforeNavigate (this, navigationEventArgs);
    }

    //
    // INTERACTION EVENTS
    // 

    public void OnDragEnter (ExtendedDragEventHandlerArgs e)
    {
      var element = ElementAt (e.X, e.Y);

      e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));
      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (element.Document);
      e.Effect = DragDropEffects.None;

      if (DragEnter != null)
        DragEnter (this, e);

      if (e.Droppable && e.Effect == DragDropEffects.None)
        e.Effect = c_defaulEffect;
      else if (!e.Droppable)
        e.Effect = DragDropEffects.None;

      e.Handled = true;
    }

    public void OnDragDrop (ExtendedDragEventHandlerArgs e)
    {
      e.Effect = DragDropEffects.None;
      var element = ElementAt (e.X, e.Y);
      e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));
      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (element.Document);
      if (DragDrop != null)
        DragDrop (this, e);

      if (e.Droppable && e.Effect == DragDropEffects.None)
        e.Effect = c_defaulEffect;
      else if (!e.Droppable)
        e.Effect = DragDropEffects.None;

      e.Handled = true;
    }

    public new void OnDragLeave (EventArgs e)
    {
      if (DragLeave != null)
        DragLeave (this, e);
    }

    public void OnDragOver (ExtendedDragEventHandlerArgs e)
    {
      var element = ElementAt (e.X, e.Y);
      e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));
      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (element.Document);
      e.Effect = DragDropEffects.None;

      if (DragOver != null)
        DragOver (this, e);

      if (e.Droppable && e.Effect == DragDropEffects.None)
        e.Effect = c_defaulEffect;
      else if (!e.Droppable)
        e.Effect = DragDropEffects.None;
      e.Handled = true;
    }

    public void OnBrowserKeyDown (TridentWebBrowser extendedTridentWebBrowser, KeyEventArgs keyEventArgs)
    {
      var keyCode = keyEventArgs.KeyCode | ModifierKeys;

      if (ModifierKeys == 0 || !Enum.IsDefined (typeof (Shortcut), (Shortcut) keyCode))
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

    private HtmlElement ElementAt (int x, int y)
    {
      if (Document == null)
        return null;

      var locationOnScreen = PointToScreen (Location);
      return Document.GetElementFromPoint (new Point (x - locationOnScreen.X, y - locationOnScreen.Y));
    }

    private void AddSubscribedAddIns (ISubscriptionHandler subscriptionHandler, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionHandler.GetSubscribers<IBrowserEventSubscriber> (handle))
      {
        //Unsubscribe (subscriber);
        Subscribe (subscriber);
      }
    }

    private void RemoveSubscribedAddIns (ISubscriptionHandler subscriptionHandler, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionHandler.GetSubscribers<IBrowserEventSubscriber> (handle))
        Unsubscribe (subscriber);
    }

    private void Subscribe (IBrowserEventSubscriber subscriber)
    {
      DragEnter += subscriber.OnDragEnter;
      DragDrop += subscriber.OnDragDrop;
      DragOver += subscriber.OnDragOver;
      DragLeave += subscriber.OnDragLeave;
    }

    private void Unsubscribe (IBrowserEventSubscriber subscriber)
    {
      DragEnter -= subscriber.OnDragEnter;
      DragDrop -= subscriber.OnDragDrop;
      DragOver -= subscriber.OnDragOver;
      DragLeave -= subscriber.OnDragLeave;
    }

    private void TridentWebBrowser_DocumentCompleted (object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      if (e.Url.AbsolutePath != Url.AbsolutePath)
        return;

      if (PageLoadFinished != null)
        PageLoadFinished (this, this);

      if (AfterNavigate != null)
        AfterNavigate (this, new NavigationEventArgs (_startMode, false, Url.AbsolutePath, ""));

      if (ObjectForScripting == null || Document == null || Document.Window == null)
        return;

      var handles = new HashSet<HtmlDocumentHandle>();
      foreach (var frame in new FrameIterator (Document).GetFrames())
      {
        try
        {
          var handle = DocumentIdentifier.GetOrCreateDocumentHandle (frame.Document);
          handles.Add (handle);

          if (!_documentHandleRegistry.HasDocumentHandle (handle))
          {
            _documentHandleRegistry.RegisterDocumentHandle (handle, this);
            AddSubscribedAddIns (_subscriptionHandler, handle);
          }
        }
        catch (UnauthorizedAccessException)
        {
          // pass
        }
      }


      if (_activeHandles != null)
        foreach (var handle in _activeHandles.Except (handles))
        {
          RemoveSubscribedAddIns (_subscriptionHandler, handle);

          _documentHandleRegistry.UnregisterDocumentHandle (handle);
        }
      _activeHandles = handles;
    }

    private IDictionary<string, string> GetRelevantAttributes (HtmlElement element)
    {
      var dict = new Dictionary<string, string> (s_attributes.Length);
      foreach (var attributeName in s_attributes)
        dict.Add (attributeName, element.GetAttribute (attributeName));

      return dict;
    }

    public void OnExecute (object sender, ScriptEventArgs args)
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (Document != null)
        Document.InvokeScript (args.Function, new object[] { args.Serialize() });
    }
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