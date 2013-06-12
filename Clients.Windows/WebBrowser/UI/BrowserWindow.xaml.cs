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
using System.Windows;
using System.Windows.Media.Imaging;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public partial class BrowserWindow : IWebBrowserWindow
  {
    private readonly Uri _homeUri;
    private readonly ViewDispatcherBase _viewDispatcher;

    private readonly List<IWebBrowserView> _subViews = new List<IWebBrowserView>();
    private bool _nextIsSticky;

    public BrowserWindow (string title, Uri iconUri, Uri homeUri, ViewDispatcherBase viewDispatcher)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("homeUri", homeUri);
      ArgumentUtility.CheckNotNull ("iconUri", iconUri);
      ArgumentUtility.CheckNotNull ("viewDispatcher", viewDispatcher);

      InitializeComponent();
      Title = title;
      _homeUri = homeUri;
      _viewDispatcher = viewDispatcher;
      _viewDispatcher.ViewCreated += OnSubViewCreated;
      Closing += (s, e) => e.Cancel = CloseWindow();

      Icon = new BitmapImage (iconUri);
    }

    public void Dispose ()
    {
      foreach (var disposable in _subViews)
        disposable.Dispose();
    }

    private bool CloseWindow ()
    {
      foreach (var browserView in _subViews.Where (view => view.ShouldClose()).ToList())
      {
        browserView.CloseView();
        RemoveSubView (browserView);
      }

      return _subViews.Count > 0;
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      NewTab (CreateUri (_urlTextBox.Text), BrowserWindowStartMode.Active);
    }

    public void NewStickyTab (Uri uri, BrowserWindowStartMode mode)
    {
      ArgumentUtility.CheckNotNull ("uri", uri);

      _nextIsSticky = true;
      NewTab (uri, mode);
    }

    public void NewTab (Uri uri, BrowserWindowStartMode mode)
    {
      ArgumentUtility.CheckNotNull ("uri", uri);

      _viewDispatcher.NewView (BrowserWindowTarget.Tab, uri, mode);
    }

    public void NewPopUp (Uri uri, BrowserWindowStartMode mode)
    {
      ArgumentUtility.CheckNotNull ("uri", uri);

      _viewDispatcher.NewView (BrowserWindowTarget.PopUp, uri, mode);
    }

    private void OnSubViewCreated (object sender, NewViewEventArgs args)
    {
      if (args.StartMode == BrowserWindowStartMode.Modal)
        return;

      var tab = args.View as BrowserTab;
      if (tab != null)
      {
        _tabControl.Items.Add (tab);
        if (_nextIsSticky)
        {
          tab.MakeStickyTab();
          _nextIsSticky = false;
        }
        args.View.BeforeClose += (s, e) => RemoveSubView (tab);
      }
      args.View.Show (args.StartMode);
      _subViews.Add (args.View);
    }

    private void RemoveSubView (IWebBrowserView view)
    {
      if (view is BrowserTab)
        _tabControl.Items.Remove (view);
      _subViews.Remove (view);
    }

    private void OnGotoHome (object sender, RoutedEventArgs e)
    {
      var homeTab = _subViews.First (view => view is BrowserTab && ((BrowserTab) view).Option == BrowserTabOption.Sticky);
      if (homeTab != null)
      {
        homeTab.WebBrowser.Navigate (_homeUri.ToString());
        ((BrowserTab)homeTab).Focus();
      }
      else
        NewTab (_homeUri, BrowserWindowStartMode.Active);
    }

    private void OnZoomIn (object sender, RoutedEventArgs e)
    {
      var currentBrowser = ((BrowserTab) _tabControl.Items[_tabControl.SelectedIndex]).WebBrowser;
      currentBrowser.Zoom (100);
    }

    private void OnPrint (object sender, RoutedEventArgs e)
    {
      var currentBrowser = ((BrowserTab) _tabControl.Items[_tabControl.SelectedIndex]).WebBrowser;
      currentBrowser.Print();
    }

    private void OnPrintPreview (object sender, RoutedEventArgs e)
    {
      var currentBrowser = ((BrowserTab) _tabControl.Items[_tabControl.SelectedIndex]).WebBrowser;
      currentBrowser.PrintPreview();
    }

    private Uri CreateUri (string url)
    {
      var protocolUrl = !url.Contains (Uri.SchemeDelimiter) ? string.Join (Uri.SchemeDelimiter, Uri.UriSchemeHttp, url) : url;
      Uri uri;
      if (!Uri.TryCreate (protocolUrl, UriKind.RelativeOrAbsolute, out uri))
        throw new Exception ("Invalid URL");
      return uri;
    }
  }
}