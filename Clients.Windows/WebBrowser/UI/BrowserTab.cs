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
using System.Windows.Controls;
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
    public event EventHandler<EventArgs> TabClosing;


    private readonly WebBrowserHost _webBrowserHost;

    public BrowserTab (TridentWebBrowser webBrowser, Guid identifier, bool isCloseable = true)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      Identifier = identifier;
      webBrowser.DocumentTitleChanged += OnDocumentTitleChanged;
      IsCloseable = isCloseable;
      _webBrowserHost = new WebBrowserHost (webBrowser);;
      Content = _webBrowserHost;
    }


    public void Dispose ()
    {
      CleanUp();
    }

    public bool IsCloseable
    {
      get { return Header != null && Header.IsCloseable; }
      set
      {
        if (Header != null)
          Header.IsCloseable = value;
      }
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

    public void Show (BrowserWindowStartMode startMode)
    {
      Header = new CloseableTabHeader (_webBrowserHost.WebBrowser.Url.ToString(), IsCloseable);

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

    private void OnDocumentTitleChanged (object sender, EventArgs e)
    {
      var header = new CloseableTabHeader (_webBrowserHost.WebBrowser.Title, IsCloseable);
      if (IsCloseable)
        header.TabClose += OnTabClose;
      Header = header;
    }

    private void OnTabClose (object sender, EventArgs e)
    {
      if (TabClosing == null)
        return;

      TabClosing (this, e);

      CleanUp();
    }

    private void CleanUp ()
    {
      _webBrowserHost.Dispose();
    }
  }
}