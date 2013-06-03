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
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DesktopGap.Clients.Windows.Components;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for BrowserTab.xaml
  /// </summary>
  public sealed class BrowserTab : TabItem, IWebBrowserView
  {
    public event EventHandler<EventArgs> Closing;

    private readonly WebBrowserHost _webBrowserHost;

    private static readonly Uri s_defaultImageUri = new Uri ("/DesktopGap;component/Resources/new.png", UriKind.RelativeOrAbsolute);


    public BrowserTab (TridentWebBrowser webBrowser, Guid identifier, bool isCloseable = true, bool isHomeTab = false)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);
      Identifier = identifier;
      IsHomeTab = isHomeTab;

      webBrowser.DocumentTitleChanged += OnDocumentTitleChanged;
      webBrowser.DocumentsFinished += (s, e) => Header.Icon = webBrowser.GetFavicon (s_defaultImageUri);


      _webBrowserHost = new WebBrowserHost (webBrowser);
      Content = _webBrowserHost;

      Header = new CloseableTabHeader (string.Empty, new BitmapImage (s_defaultImageUri), isCloseable);
      Header.TabClose += (s, e) =>
                         {
                           bool cancel;
                           Close (out cancel);
                         };


      if (IsHomeTab)
        return;

      LostFocus += (s, e) => IsCloseable = false;
      GotFocus += (s, e) => IsCloseable = true;
    }


    public void Dispose ()
    {
      CleanUp();
    }

    public bool IsCloseable
    {
      get { return Header.IsCloseable; }
      set { Header.IsCloseable = !IsHomeTab && value; }
    }

    public new CloseableTabHeader Header
    {
      get { return (CloseableTabHeader) base.Header; }
      set { base.Header = value; }
    }

    public IExtendedWebBrowser WebBrowser
    {
      get { return _webBrowserHost.WebBrowser; }
    }

    public Guid Identifier { get; private set; }
    public bool IsHomeTab { get; set; }

    public void Show (BrowserWindowStartMode startMode)
    {
      switch (startMode)
      {
        case BrowserWindowStartMode.Active:
          Focus();
          break;
        case BrowserWindowStartMode.Background:
          break;
        default:
          throw new InvalidOperationException (string.Format ("Start mode '{0}' is not supported for BrowserTab.", startMode));
      }
    }

    public void Close (out bool cancel)
    {
      cancel = !_webBrowserHost.WebBrowser.ShouldClose();
      if (cancel)
        return;

      if (Closing != null)
        Closing (this, EventArgs.Empty);
      Dispose();
    }

    private void OnDocumentTitleChanged (object sender, EventArgs e)
    {
      Header.Text = string.IsNullOrEmpty (WebBrowser.Title) ? WebBrowser.Url.ToString() : WebBrowser.Title;
    }

    private void CleanUp ()
    {
      _webBrowserHost.Dispose();
    }
  }
}