// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DesktopGap.Clients.Windows.Components;
using DesktopGap.Security;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  public struct BrowserTabState
  {
    public Brush HeaderColor { get; set; }
    public bool IsClosable { get; set; }
    public bool ShowAddressBar { get; set; }
  }


  public partial class BrowserTab : IWebBrowserView, INotifyPropertyChanged
  {
    private readonly IDictionary<Tuple<TargetAddressType, TabType>, BrowserTabState> _states;

    public enum TabType
    {
      HomeTab,
      CommonTab
    }


    private static readonly Uri s_defaultImageUri = new Uri ("/DesktopGap;component/Resources/new.png", UriKind.RelativeOrAbsolute);

    private Visibility _addressBarVisibility;
    private TabType _type;


    public event EventHandler<EventArgs> BeforeClose;


    public BrowserTab (TridentWebBrowser webBrowser, Guid identifier, IDictionary<Tuple<TargetAddressType, TabType>, BrowserTabState> states)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);
      ArgumentUtility.CheckNotNull ("states", states);

      InitializeComponent();

      Identifier = identifier;
      _states = states;
      TabHeaderViewModel = new TabHeaderViewModel (
          Visibility.Collapsed, new BitmapImage (s_defaultImageUri), webBrowser.Url.ToString(), Brushes.Transparent);


      TabHeaderViewModel.CloseClicked += (s, e) =>
                                         {
                                           if (ShouldClose())
                                             CloseView();
                                         };

      webBrowser.DocumentTitleChanged += OnDocumentTitleChanged;
      webBrowser.DocumentsFinished += (s, e) => TabHeaderViewModel.Icon = webBrowser.GetFavicon (s_defaultImageUri);
      webBrowser.Navigated += (s, e) => OnPropertyChanged("Url");

      WebBrowserHost = new WebBrowserHost (webBrowser);
      DataContext = this;

      webBrowser.BeforeNavigate += OnBeforeNavigate;


      Type = TabType.CommonTab;
    }

    private void OnBeforeNavigate (object sender, NavigationEventArgs e)
    {
      ChangeState (_states[Tuple.Create (e.AddressType, Type)]);
    }

    public void Dispose ()
    {
      CleanUp();
    }

    public WebBrowserHost WebBrowserHost { get; private set; }

    public Visibility AddressBarVisibility
    {
      get { return _addressBarVisibility; }
      set
      {
        _addressBarVisibility = value;
        OnPropertyChanged ("AddressBarVisibility");
      }
    }

    public TabType Type
    {
      get { return _type; }
      set
      {
        _type = value;

        LostFocus -= OnFocusLost;
        GotFocus -= OnFocus;

        ChangeState (_states[Tuple.Create (((TridentWebBrowser) WebBrowser).CurrentAddressType, Type)]);
        if (value != TabType.CommonTab)
          return;

        LostFocus += OnFocusLost;
        GotFocus += OnFocus;
      }
    }

    public IExtendedWebBrowser WebBrowser
    {
      get { return WebBrowserHost.WebBrowser; }
    }

    public Guid Identifier { get; private set; }

    public TabHeaderViewModel TabHeaderViewModel { get; private set; }

    public string Url
    {
      get { return WebBrowser.Url.ToString(); }
      set { }
    }

    public void Show (BrowserWindowStartMode startMode)
    {
      switch (startMode)
      {
        case BrowserWindowStartMode.Active:
          Focus();
          break;
        case BrowserWindowStartMode.Background:
          TabHeaderViewModel.CloseButtonVisibility = Visibility.Collapsed;

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


    private void OnFocus (object s, RoutedEventArgs e)
    {
      TabHeaderViewModel.CloseButtonVisibility = Visibility.Visible;
    }

    private void OnFocusLost (object s, RoutedEventArgs e)
    {
      TabHeaderViewModel.CloseButtonVisibility = Visibility.Collapsed;
    }


    private void OnDocumentTitleChanged (object sender, EventArgs e)
    {
      TabHeaderViewModel.Text = string.IsNullOrEmpty (WebBrowser.Title) ? WebBrowser.Url.ToString() : WebBrowser.Title;
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


    private void OnUrlTextKeyDown (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return && WebBrowser != null)
        WebBrowser.Navigate (CreateUri (urlTextBox.Text).ToString());
    }

    private Uri CreateUri (string url)
    {
      var protocolUrl = !url.Contains (Uri.SchemeDelimiter) ? string.Join (Uri.SchemeDelimiter, Uri.UriSchemeHttp, url) : url;
      Uri uri;
      if (!Uri.TryCreate (protocolUrl, UriKind.RelativeOrAbsolute, out uri))
        throw new Exception ("Invalid URL");
      return uri;
    }

    private void ChangeState (BrowserTabState state)
    {
      TabHeaderViewModel.BackgroundBrush = state.HeaderColor;
      TabHeaderViewModel.IsCloseable = state.IsClosable && IsFocused;

      AddressBarVisibility = state.ShowAddressBar ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}