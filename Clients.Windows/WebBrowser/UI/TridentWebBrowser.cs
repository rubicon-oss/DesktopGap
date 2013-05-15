﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Events.Subscriptions;
using DesktopGap.Clients.Windows.WebBrowser.Scripting;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.Util;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using Point = System.Drawing.Point;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentWebBrowser : TridentWebBrowserBase, IExtendedWebBrowser, IScriptingHost
  {
    private class WindowPreparations
    {
      public BrowserWindowTarget Type { get; set; }

      public BrowserWindowStartMode StartMode { get; set; }
      public string TargetName { get; set; }
    }

    private const BrowserWindowStartMode c_defaultStartMode = BrowserWindowStartMode.Active;
    private const BrowserWindowTarget c_defaultWindowTarget = BrowserWindowTarget.Tab;

    private const DragDropEffects c_defaulEffect = DragDropEffects.Move;
    private const string c_blankSite = "about:blank";

    private static readonly string[] s_attributes = new[] { "dg_droptarget", "dg_dropcondition" };


    public event EventHandler<EventArgs> DocumentsFinished;
    public event EventHandler<NavigationEventArgs> AfterNavigate;
    public event EventHandler<WindowOpenEventArgs> WindowOpen;
    public event EventHandler<NavigationEventArgs> BeforeNavigate;
    public event EventHandler<NavigationEventArgs> PrepareNavigation;

    public new event EventHandler<ExtendedDragEventHandlerArgs> DragEnter;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragDrop;
    public new event EventHandler<ExtendedDragEventHandlerArgs> DragOver;
    public new event EventHandler DragLeave;

    public event EventHandler<int> WindowSetHeight;
    public event EventHandler<int> WindowSetLeft;
    public event EventHandler<int> WindowSetTop;
    public event EventHandler<int> WindowSetWidth;
    public event EventHandler<bool> WindowSetResizable;

    private IDictionary<HtmlDocumentHandle, HtmlDocument> _currentDocuments = new Dictionary<HtmlDocumentHandle, HtmlDocument>();

    private readonly IHtmlDocumentHandleRegistry _documentHandleRegistry;
    private readonly ISubscriptionHandler _subscriptionHandler;

    private WindowPreparations _windowPreparations;
    private bool _resizable = true;

    public TridentWebBrowser (IHtmlDocumentHandleRegistry documentHandleRegistry, ISubscriptionHandler subscriptionHandler, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("documentHandleRegistry", documentHandleRegistry);
      ArgumentUtility.CheckNotNull ("subscriptionHandler", subscriptionHandler);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);

      _BrowserEvents = new WebBrowserEvents (this, urlFilter);

      Navigate (c_blankSite); // bootstrap
      ObjectForScripting = new ApiFacade (documentHandleRegistry);
      _documentHandleRegistry = documentHandleRegistry;
      _subscriptionHandler = subscriptionHandler;

      BrowserMode = TridentWebBrowserMode.ForcedIE10;
      InstallCustomUIHandler (new DocumentHostUIHandler (this));

      DocumentCompleted += TridentWebBrowser_DocumentCompleted;
      _documentHandleRegistry.NewDocumentRegistered += OnNewDocumentRegistered;
      ScriptErrorsSuppressed = true;
    }

    protected override void Dispose (bool disposing)
    {
      BeforeNavigate = null;
      AfterNavigate = null;
      DocumentsFinished = null;
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

      if (_currentDocuments != null)
        _currentDocuments.Clear();
      _currentDocuments = null;

      base.Dispose (disposing);

    //  _browser.Dispose();

    //  var window = GetWindowElement (_browser);

    //  if (window == null)
    //    return;

    //  var field = typeof (Window).GetField ("_swh", BindingFlags.NonPublic | BindingFlags.Instance);

    //  var valueSwh = field.GetValue (window);
    //  var valueSourceWindow = valueSwh.GetType().GetField ("_sourceWindow", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (valueSwh);
    //  var valuekeyboardInput =
    //      valueSourceWindow.GetType().GetField ("_keyboardInputSinkChildren", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (
    //          valueSourceWindow);

    //  var inputSites = valuekeyboardInput as IEnumerable<IKeyboardInputSite>;

    //  if (inputSites == null)
    //    return;

    //  var currentSite = inputSites.FirstOrDefault (s => ReferenceEquals (s.Sink, _browser));

    //  if (currentSite != null)
    //    currentSite.Unregister();
    //}

    //private static Window GetWindowElement (DependencyObject element)
    //{
    //  while (element != null && !(element is Window))
    //  {
    //    element = VisualTreeHelper.GetParent (element);
    //  }

    //  return element as Window;
    }


    public string Title
    {
      get
      {
        var title = String.Empty;
        try
        {
          if (Document != null)
            title = Document.Title;
        }
        catch
        {
          // pass
        }
        return title;
      }
    }

    public bool IsResizable
    {
      get { return _resizable; }
      private set { _resizable = value; }
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
      if (WindowSetHeight != null)
        WindowSetHeight (this, height);

      Height = height;
    }

    public void OnWindowSetLeft (int left)
    {
      if (WindowSetLeft != null)
        WindowSetLeft (this, left);

      Left = left;
    }

    public void OnWindowSetTop (int top)
    {
      if (WindowSetTop != null)
        WindowSetTop (this, top);

      Top = top;
    }

    public void OnWindowSetWidth (int width)
    {
      if (WindowSetWidth != null)
        WindowSetWidth (this, width);

      Width = width;
    }


    public void OnWindowSetResizable (bool resizable)
    {
      if (WindowSetResizable != null)
        WindowSetResizable (this, resizable);

      IsResizable = resizable;
    }

    //
    // NAVIGATION EVENTS
    // 
    public void OnNewWindow (WindowOpenEventArgs eventArgs)
    {
      ArgumentUtility.CheckNotNull ("eventArgs", eventArgs);

      if (_windowPreparations != null)
        eventArgs.BrowserWindowTarget = _windowPreparations.Type;

      if (WindowOpen != null)
        WindowOpen (this, eventArgs);
    }

    public void OnBeforeNavigate (NavigationEventArgs navigationEventArgs)
    {
      ArgumentUtility.CheckNotNull ("navigationEventArgs", navigationEventArgs);

      if (PrepareNavigation != null)
        PrepareNavigation (this, navigationEventArgs);

      if (navigationEventArgs.Handled)
      {
        _windowPreparations = new WindowPreparations
                              {
                                  StartMode = navigationEventArgs.StartMode,
                                  Type = navigationEventArgs.BrowserWindowTarget,
                                  TargetName = navigationEventArgs.TargetName
                              };
      }

      if (navigationEventArgs.URL.ToString() == c_blankSite)
        return;


      if (BeforeNavigate != null)
        BeforeNavigate (this, navigationEventArgs);

      _windowPreparations = null;
    }


    //
    // INTERACTION EVENTS
    // 

    public void OnDragEnter (ExtendedDragEventHandlerArgs e)
    {
      var element = ElementAt (e.X, e.Y);

      var document = element != null ? element.Document : Document;

      if (element != null)
        e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));

      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (document);
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
      var element = ElementAt (e.X, e.Y);
      var document = element != null ? element.Document : Document;

      if (element != null)
        e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));

      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (document);
      e.Effect = DragDropEffects.None;

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
      var document = element != null ? element.Document : Document;

      if (element != null)
        e.Current = new HtmlElementData (element.Id, GetRelevantAttributes (element));

      e.Document = DocumentIdentifier.GetOrCreateDocumentHandle (document);
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

    public void OnExecute (object sender, ScriptEventArgs args)
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("sender", sender);


      HtmlDocument document;
      if (_currentDocuments.TryGetValue (args.DocumentHandle, out document))
        document.InvokeScript (args.Function, new object[] { args.Serialize() });
    }


    //
    // OTHER METHODS & EVENTS
    // 

    private void OnNewDocumentRegistered (object sender, DocumentRegisteredEventArgs e)
    {
      HtmlDocument document;
      if (_currentDocuments.TryGetValue (e.DocumentHandle, out document))
        document.InvokeScript ("DesktopGap_DocumentRegistered");
    }

    private HtmlElement ElementAt (int x, int y)
    {
      if (Document == null)
        return null;

      HtmlWindow previous = null;
      HtmlElement element = null;
      foreach (var frame in new FrameIterator (Document).GetFrames().TakeWhile (frame => frame.Position.X <= x && frame.Position.Y <= y))
        previous = frame;

      if (previous != null && previous.Document != null)
      {
        var point = new Point (x - previous.Position.X, y - previous.Position.Y);
        element = previous.Document.GetElementFromPoint (point);
      }
      return element;
    }

    private void AddSubscribedAddIns (ISubscriptionHandler subscriptionHandler, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionHandler.GetSubscribers<ISubscriber> (handle))
      {
        //Unsubscribe (subscriber);
        if (subscriber is IBrowserEventSubscriber)
          Subscribe (subscriber as IBrowserEventSubscriber);
        else if (subscriber is IWindowEventSubscriber)
          Subscribe (subscriber as IWindowEventSubscriber);
      }
    }

    private void AddDocument (HtmlDocumentHandle handle, HtmlDocument document)
    {
      _currentDocuments[handle] = document;
      _documentHandleRegistry.RegisterDocumentHandle (handle, this);
      AddSubscribedAddIns (_subscriptionHandler, handle);
    }

    private void RemoveSubscribedAddIns (ISubscriptionHandler subscriptionHandler, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionHandler.GetSubscribers<ISubscriber> (handle))
      {
        if (subscriber.GetType().IsAssignableFrom (typeof (IBrowserEventSubscriber)))
          Unsubscribe (subscriber as IBrowserEventSubscriber);
        else if (subscriber.GetType().IsAssignableFrom (typeof (IWindowEventSubscriber)))
          Unsubscribe (subscriber as IWindowEventSubscriber);
      }
    }

    private void RemoveDocument (HtmlDocumentHandle handle)
    {
      _currentDocuments.Remove (handle);
      RemoveSubscribedAddIns (_subscriptionHandler, handle);
      _documentHandleRegistry.UnregisterDocumentHandle (handle);
    }

    private void Subscribe (IBrowserEventSubscriber subscriber)
    {
      DragEnter += subscriber.OnDragEnter;
      DragDrop += subscriber.OnDragDrop;
      DragOver += subscriber.OnDragOver;
      DragLeave += subscriber.OnDragLeave;
    }

    private void Subscribe (IWindowEventSubscriber subscriber)
    {
      PrepareNavigation += subscriber.OnPrepareNavigation;
    }

    private void Unsubscribe (IBrowserEventSubscriber subscriber)
    {
      DragEnter -= subscriber.OnDragEnter;
      DragDrop -= subscriber.OnDragDrop;
      DragOver -= subscriber.OnDragOver;
      DragLeave -= subscriber.OnDragLeave;
    }

    private void Unsubscribe (IWindowEventSubscriber subscriber)
    {
      PrepareNavigation -= subscriber.OnPrepareNavigation;
    }

    private void TridentWebBrowser_DocumentCompleted (object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      try
      {
        if (ObjectForScripting == null || Document == null || Document.Window == null)
          return;
      }
      catch (UnauthorizedAccessException)
      {
        // pass
      }
      var activeHandles = new List<HtmlDocumentHandle>();
      foreach (var frame in new FrameIterator (Document).GetFrames())
      {
        try
        {
          var handle = DocumentIdentifier.GetOrCreateDocumentHandle (frame.Document);
          activeHandles.Add (handle);

          if (!_currentDocuments.ContainsKey (handle))
            AddDocument (handle, frame.Document);
        }
        catch (UnauthorizedAccessException)
        {
          // pass
        }
      }
      var inactiveHandles = _currentDocuments.Keys.Where (handle => !activeHandles.Contains (handle)).ToList();
      foreach (var handle in inactiveHandles)
        RemoveDocument (handle);


      if (e.Url.AbsolutePath == Url.AbsolutePath)
      {
        var startMode = c_defaultStartMode;
        var windowTarget = c_defaultWindowTarget;
        var targetName = String.Empty;
        if (_windowPreparations != null)
        {
          startMode = _windowPreparations.StartMode;
          windowTarget = _windowPreparations.Type;
          targetName = _windowPreparations.TargetName;
        }
        if (AfterNavigate != null)
          AfterNavigate (this, new NavigationEventArgs (startMode, false, e.Url.AbsolutePath, targetName, windowTarget));
      }
    }


    private IDictionary<string, string> GetRelevantAttributes (HtmlElement element)
    {
      var dict = new Dictionary<string, string> (s_attributes.Length);
      foreach (var attributeName in s_attributes)
        dict.Add (attributeName, element.GetAttribute (attributeName));

      return dict;
    }


    public void OnDocumentComplete (string url)
    {
      if (url != Url.ToString())
        return;

      if (DocumentsFinished != null)
        DocumentsFinished (this, new EventArgs());
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