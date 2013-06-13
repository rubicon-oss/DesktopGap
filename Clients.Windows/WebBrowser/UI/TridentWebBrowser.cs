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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Events.Subscriptions;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;
using DesktopGap.Clients.Windows.WebBrowser.Scripting;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.Util;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public class TridentWebBrowser : TridentWebBrowserBase, IExtendedWebBrowser, IScriptingHost
  {
    private const BrowserWindowStartMode c_defaultStartMode = BrowserWindowStartMode.Active;
    private const BrowserWindowTarget c_defaultWindowTarget = BrowserWindowTarget.Tab;
    private const string c_registrationDoneCallback = "DesktopGap_DocumentRegistered";
    private const string c_defaultFaviconPath = "/favicon.ico";

    private const DragDropEffects c_defaulEffect = DragDropEffects.Move;
    private const string c_blankSite = "about:blank";

    private static readonly string[] s_attributes = new[] { "dg_droptarget", "dg_dropcondition" };

    private static readonly Regex _linkExpression = new Regex (
        @"<link.*rel=\""[ \w]*icon\"".*/>",
        RegexOptions.CultureInvariant
        | RegexOptions.IgnoreCase
        | RegexOptions.Compiled);

    private static readonly Regex _faviconExpression = new Regex (
        @"<.*href=\""(.*)\"".*/>",
        RegexOptions.CultureInvariant
        | RegexOptions.IgnoreCase
        | RegexOptions.Compiled);

    public event EventHandler<EventArgs> DocumentsFinished;

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
    public event EventHandler<bool> WindowSetResizable;


    private IDictionary<HtmlDocumentHandle, HtmlDocument> _currentDocuments = new Dictionary<HtmlDocumentHandle, HtmlDocument>();

    private readonly IHtmlDocumentHandleRegistry _documentHandleRegistry;
    private readonly ISubscriptionProvider _subscriptionProvider;
    private readonly IUrlFilter _applicationUrlFiler;

    private bool _resizable = true;
    private bool _allowCalls;

    public TridentWebBrowser (
        IHtmlDocumentHandleRegistry documentHandleRegistry,
        ISubscriptionProvider subscriptionProvider,
        IUrlFilter nonApplicationUrlFilter,
        IUrlFilter entryPointFilter,
        IUrlFilter applicationUrlFilter)
    {
      ArgumentUtility.CheckNotNull ("documentHandleRegistry", documentHandleRegistry);
      ArgumentUtility.CheckNotNull ("subscriptionProvider", subscriptionProvider);
      ArgumentUtility.CheckNotNull ("nonApplicationUrlFilter", nonApplicationUrlFilter);
      ArgumentUtility.CheckNotNull ("entryPointFilter", entryPointFilter);
      ArgumentUtility.CheckNotNull ("applicationUrlFilter", applicationUrlFilter);

      BrowserEvents = new WebBrowserEvents (this, nonApplicationUrlFilter, applicationUrlFilter, entryPointFilter);
      Navigate (c_blankSite); // bootstrap

      _documentHandleRegistry = documentHandleRegistry;
      _subscriptionProvider = subscriptionProvider;
      _applicationUrlFiler = applicationUrlFilter;

      InstallCustomUIHandler (new DocumentHostUIHandler (this));

      DocumentCompleted += TridentWebBrowser_DocumentCompleted;
      _documentHandleRegistry.DocumentRegistered += OnDocumentRegistered;
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

    //private new ApiFacade ObjectForScripting
    //{
    //  get { return (ApiFacade) base.ObjectForScripting; }
    //  set { base.ObjectForScripting = value; }
    //}

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

      if (WindowOpen != null)
        WindowOpen (this, eventArgs);
    }

    public void OnBeforeNavigate (NavigationEventArgs navigationEventArgs)
    {
      ArgumentUtility.CheckNotNull ("navigationEventArgs", navigationEventArgs);

      if (navigationEventArgs.Url.ToString() == c_blankSite)
        return;

      _allowCalls = _applicationUrlFiler.IsAllowed (navigationEventArgs.Url);
      if (_allowCalls)
        ObjectForScripting = new ApiFacade (_documentHandleRegistry);


      if (BeforeNavigate != null)
        BeforeNavigate (this, navigationEventArgs);
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

      if (keyCode == (Keys.Control | Keys.NumPad0)) // TODO improve
        Zoom (100);


      if (ModifierKeys == 0 || !Enum.IsDefined (typeof (Shortcut), (Shortcut) keyCode))
        return;

      switch (keyCode)
      {
        case Keys.Control | Keys.A: // Select All
        case Keys.Control | Keys.C: // Copy
        case Keys.Control | Keys.V: // Paste
        case Keys.Control | Keys.X: // Cut
        case Keys.Control | Keys.F: // Find
        case Keys.Control | Keys.NumPad0: // reset zoom
        case Keys.Delete: // Delete
          keyEventArgs.Handled = !EnableWebBrowserEditingShortcuts;
          break;
        default:
          keyEventArgs.Handled = !EnableWebBrowserShortcuts;
          break;
      }
    }

    public void OnExecute (object sender, ScriptEventArgs args)
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("sender", sender);


      HtmlDocument document;
      if (_allowCalls && _currentDocuments.TryGetValue (args.DocumentHandle, out document))
        document.InvokeScript (args.Function, new object[] { args.Serialize() });
    }


    //
    // OTHER METHODS & EVENTS
    // 


    public BitmapImage GetFavicon (Uri defaultUri)
    {
      var linkElements = _linkExpression.Matches (DocumentText);
      String faviconPath = null;
      foreach (Match match in linkElements)
      {
        var favicon = _faviconExpression.Match (match.ToString());
        if (favicon.Success)
          faviconPath = favicon.Groups[1].Value;
      }
      if (string.IsNullOrEmpty (faviconPath) || Uri.IsWellFormedUriString (faviconPath, UriKind.Relative))
        faviconPath = c_defaultFaviconPath;

      try
      {
        var uri = new Uri (Url.GetLeftPart (UriPartial.Authority) + faviconPath);
        // Probe if icon exists
        var requ = WebRequest.Create (uri);
        requ.Method = "HEAD";
        requ.Timeout = 1000;

        var resp = requ.GetResponse();
        return new BitmapImage (uri);
      }
      catch (Exception ex)
      {
        return new BitmapImage (defaultUri);
      }
    }

    public void Zoom (int factor)
    {
      object pvaIn = factor;
      AxIWebBrowser2.ExecWB (
          OLECMDID.OLECMDID_OPTICAL_ZOOM,
          OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER,
          ref pvaIn,
          IntPtr.Zero);
    }

    public void PrintPreview ()
    {
      ShowPrintPreviewDialog();
    }

    public new void Print ()
    {
      ShowPrintDialog();
    }

    private void OnDocumentRegistered (object sender, DocumentRegisterationEventArgs e)
    {
      HtmlDocument document;
      if (_currentDocuments != null && _currentDocuments.TryGetValue (e.DocumentHandle, out document))
        document.InvokeScript (c_registrationDoneCallback);
    }

    private HtmlElement ElementAt (int x, int y)
    {
      if (Document == null)
        return null;

      HtmlWindow previous = null;
      HtmlElement element = null;
      foreach (var frame in new FrameIterator (Document).GetFrames())
      {
        try // this way, only a small portion of the code is under try/catch safety - leaving out the recursive stuff for performance
        {
          if (!(frame.Position.X <= x && frame.Position.Y <= y))
            break;
        }
        catch (UnauthorizedAccessException unauthorizedException)
        {
          Debug.WriteLine (unauthorizedException.ToString());
          break;
        }
        previous = frame;
      }
    
      if (previous != null && previous.Document != null)
      {
        var point = new Point (x - previous.Position.X, y - previous.Position.Y);
        element = previous.Document.GetElementFromPoint (point);
      }
      return element;
    }

    private void AddSubscribedAddIns (ISubscriptionProvider subscriptionProvider, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionProvider.GetSubscribers<IBrowserEventSubscriber> (handle))
        Subscribe (subscriber);
    }

    private void AddDocument (HtmlDocumentHandle handle, HtmlDocument document)
    {
      _currentDocuments[handle] = document;
      _documentHandleRegistry.RegisterDocumentHandle (handle, this);
      AddSubscribedAddIns (_subscriptionProvider, handle);
    }

    private void RemoveSubscribedAddIns (ISubscriptionProvider subscriptionProvider, HtmlDocumentHandle handle)
    {
      foreach (var subscriber in subscriptionProvider.GetSubscribers<IBrowserEventSubscriber> (handle))
      {
        Unsubscribe (subscriber);
      }
    }

    private void RemoveDocument (HtmlDocumentHandle handle)
    {
      _currentDocuments.Remove (handle);
      RemoveSubscribedAddIns (_subscriptionProvider, handle);
      _documentHandleRegistry.UnregisterDocumentHandle (handle);
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

        if (AfterNavigate != null)
          AfterNavigate (this, new NavigationEventArgs (startMode, false, e.Url, targetName, windowTarget));
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

    public bool ShouldClose ()
    {
      // known issue: http://support.microsoft.com/kb/253201/en-us TODO needs work
      object shouldClose = 0;
      object dummy = 0;
      try
      {
        AxIWebBrowser2.ExecWB ( // TODO exception on premature close
            OLECMDID.OLECMDID_ONUNLOAD,
            OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT,
            ref dummy,
            ref shouldClose);
      }
      catch (Exception ex)
      {
        Debug.WriteLine (ex);
      }
      return (bool) shouldClose;
    }
  }
}