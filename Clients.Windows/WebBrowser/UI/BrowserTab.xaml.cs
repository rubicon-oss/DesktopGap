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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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
  public partial class BrowserTab : IWebBrowserView, INotifyPropertyChanged
  {
    public event EventHandler<EventArgs> BeforeClose;
    private static readonly Uri s_defaultImageUri = new Uri ("/DesktopGap;component/Resources/new.png", UriKind.RelativeOrAbsolute);

    public BrowserTab (TridentWebBrowser webBrowser, Guid identifier, BrowserTabOption option = BrowserTabOption.Closeable)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      InitializeComponent();


      Identifier = identifier;
      Option = option;

      webBrowser.DocumentTitleChanged += OnDocumentTitleChanged;
      webBrowser.DocumentsFinished += (s, e) => Header.Icon = webBrowser.GetFavicon (s_defaultImageUri);
      webBrowser.Navigated += (s, e) => urlTextBox.Text = e.Url.ToString();

      WebBrowserHost = new WebBrowserHost (webBrowser);
      DataContext = this;


      Header = new CloseableTabHeader (string.Empty, new BitmapImage (s_defaultImageUri), option == BrowserTabOption.Closeable);
      Header.TabClose += (s, e) =>
                         {
                           if (ShouldClose())
                             CloseView();
                         };

      if (option == BrowserTabOption.Sticky)
        return;

      LostFocus += OnFocusLost;
      GotFocus += OnFocus;
    }

    public WebBrowserHost WebBrowserHost { get; private set; }

    public bool ShowAddressBar { get; set; }

    private void OnFocus (object s, RoutedEventArgs e)
    {
      Header.ShowCloseButton();
    }

    private void OnFocusLost (object s, RoutedEventArgs e)
    {
      Header.HideCloseButton();
    }


    public void Dispose ()
    {
      CleanUp();
    }

    public bool IsCloseable
    {
      get { return Header.IsCloseable; }
    }

    public new CloseableTabHeader Header
    {
      get { return (CloseableTabHeader) base.Header; }
      set { base.Header = value; }
    }

    public IExtendedWebBrowser WebBrowser
    {
      get { return WebBrowserHost.WebBrowser; }
    }

    public Guid Identifier { get; private set; }
    public BrowserTabOption Option { get; private set; }

    public void MakeStickyTab ()
    {
      LostFocus -= OnFocusLost;
      GotFocus -= OnFocus;
      Header.HideCloseButton();
      Option = BrowserTabOption.Sticky;
    }

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

    public bool ShouldClose ()
    {
      return WebBrowserHost.WebBrowser.ShouldClose();
    }

    public void CloseView ()
    {
      if (BeforeClose != null)
        BeforeClose (this, EventArgs.Empty);
      Dispose();
    }


    private void OnDocumentTitleChanged (object sender, EventArgs e)
    {
      Header.Text = string.IsNullOrEmpty (WebBrowser.Title) ? WebBrowser.Url.ToString() : WebBrowser.Title;
    }

    private void CleanUp ()
    {
      WebBrowserHost.Dispose();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged (string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler (this, new PropertyChangedEventArgs (propertyName));
    }

    private void OnGoClick (object sender, RoutedEventArgs e)
    {
      if (WebBrowser != null)
        WebBrowser.Navigate (CreateUri(urlTextBox .Text).ToString());
    }

    private void OnUrlTextKeyDown (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return && WebBrowser != null)
        WebBrowser.Navigate (CreateUri(urlTextBox .Text).ToString());
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