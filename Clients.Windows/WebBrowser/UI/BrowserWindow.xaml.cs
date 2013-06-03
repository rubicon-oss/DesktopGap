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
using System.Linq;
using System.Windows;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public partial class BrowserWindow : IWebBrowserWindow
  {
    private readonly Uri _baseUrl;
    private readonly ViewDispatcherBase _viewDispatcher;

    private const string c_protocolSeparator = "://";
    private const string c_httpProtocolHandler = "http";

    public BrowserWindow (string title, Uri baseUri, ViewDispatcherBase viewDispatcher)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("baseUri", baseUri);
      ArgumentUtility.CheckNotNull ("viewDispatcher", viewDispatcher);

      InitializeComponent();
      Title = title;
      _baseUrl = baseUri;
      _viewDispatcher = viewDispatcher;
      _viewDispatcher.ViewCreated += OnSubViewCreated;
      Closing += (s, e) => e.Cancel = CloseWindow();
    }

    public void Dispose ()
    {
      foreach (var disposable in _tabControl.Items.OfType<IDisposable>())
        disposable.Dispose();
    }

    private bool CloseWindow ()
    {
      var canCloseWindow = true;
      foreach (var browserTab in _tabControl.Items.OfType<BrowserTab>())
      {
        bool cancel;
        browserTab.Close (out cancel);
        canCloseWindow = canCloseWindow && !cancel;
      }
      return canCloseWindow;
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      NewTab (CreateUri (_urlTextBox.Text), BrowserWindowStartMode.Active);
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
        args.View.Closing += (s, e) => RemoveTab (tab);
      }
      args.View.Show (args.StartMode);
    }

    private void RemoveTab (BrowserTab browserTab)
    {
      _tabControl.Items.Remove (browserTab);
    }

    private void OnGotoHome (object sender, RoutedEventArgs e)
    {
      NewTab (_baseUrl, BrowserWindowStartMode.Active);
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
      var protocolUrl = !url.Contains (c_protocolSeparator) ? string.Join (c_protocolSeparator, c_httpProtocolHandler, url) : url;
      Uri uri;
      if (!Uri.TryCreate (protocolUrl, UriKind.RelativeOrAbsolute, out uri))
        throw new Exception ("Invalid URL");
      return uri;
    }
  }
}