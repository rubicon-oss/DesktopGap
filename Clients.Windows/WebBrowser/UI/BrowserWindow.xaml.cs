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
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.Session;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class BrowserWindow : IWebBrowserWindow
  {
    private class NewTabEventArgs : EventArgs
    {
      public BrowserTab BrowserTab { get; private set; }

      public NewTabEventArgs (BrowserTab browserTab)
      {
        ArgumentUtility.CheckNotNull ("browserTab", browserTab);

        BrowserTab = browserTab;
      }
    }

    private class NewPopupEventArgs : EventArgs
    {
      public PopUpWindow PopUpWindow { get; private set; }

      public NewPopupEventArgs (PopUpWindow popUpWindow)
      {
        ArgumentUtility.CheckNotNull ("popUpWindow", popUpWindow);

        PopUpWindow = popUpWindow;
      }
    }

    private class WindowPreparer
    {
      private IExtendedWebBrowser Browser { get; set; }

      public string Url { get; private set; }

      public event EventHandler<NewTabEventArgs> NewTab;

      public event EventHandler<NewPopupEventArgs> NewPopup;

      public WindowPreparer (IExtendedWebBrowser browser, string url)
      {
        ArgumentUtility.CheckNotNull ("browser", browser);
        ArgumentUtility.CheckNotNull ("url", url);

        Browser = browser;
        Url = url;
      }

      public IWebBrowserView Create (BrowserWindowTarget target)
      {
        IWebBrowserView view;
        switch (target)
        {
          case BrowserWindowTarget.PopUp:
            var newPopUp = new PopUpWindow (new WebBrowserHost ((TridentWebBrowser) Browser));
            if (NewPopup != null)
              NewPopup (this, new NewPopupEventArgs (newPopUp));
            view = newPopUp;
            break;

          default:
            var newTab = new BrowserTab (new WebBrowserHost ((TridentWebBrowser) Browser));
            if (NewTab != null)
              NewTab (this, new NewTabEventArgs (newTab));
            view = newTab;
            break;
        }
        return view;
      }
    }

    private readonly IWebBrowserFactory _browserFactory;

    private WindowPreparer _preparer;

    public BrowserWindow (IWebBrowserFactory browserFactory)
    {
      ArgumentUtility.CheckNotNull ("browserFactory", browserFactory);
      InitializeComponent();

      _browserFactory = browserFactory;
    }

    public void Dispose ()
    {
      foreach (var browserTab in _tabControl.Items.Cast<BrowserTab>())
        browserTab.Dispose();
    }

    public ISession CurrentSession { get; private set; }

    public void NewTab (string url)
    {
      var browser = _browserFactory.CreateBrowser();
      _preparer = new WindowPreparer (browser, url);
      browser.BeforeNavigate += OnBeforeNavigate;

      browser.Navigate (url);

      var view = _preparer.Create (BrowserWindowTarget.Tab);
    }

    private void OnWindowOpen (object sender, WindowOpenEventArgs eventArgs)
    {
      var webBrowser = _browserFactory.CreateBrowser();
      eventArgs.TargetView = webBrowser;
      _preparer = new WindowPreparer (webBrowser, eventArgs.Url);
      webBrowser.BeforeNavigate += OnBeforeNavigate;
    }


    private void OnBeforeNavigate (object sender, NavigationEventArgs e)
    {
      if (_preparer == null || _preparer.Url != e.URL)
        return;

      switch (e.BrowserWindowTarget)
      {
        case BrowserWindowTarget.PopUp:
          _preparer.NewPopup += OnNewPopup;
          break;

        case BrowserWindowTarget.Tab:
          _preparer.NewTab += OnNewTab;
          break;

        case BrowserWindowTarget.Window:
          break;
      }


      var view = _preparer.Create (e.BrowserWindowTarget);
      view.Show (e.StartMode);
    }

    private void OnNewTab (object sender, NewTabEventArgs eventArgs)
    {
      var browserTab = eventArgs.BrowserTab;
      var browser = browserTab.WebBrowser;

      browser.WindowOpen += OnWindowOpen; // TODO avoid stackoverflow?
      //browser.AfterNavigate += browserTab.OnAfterNavigate;
      browserTab.TabClosing += (s, e) => _tabControl.Items.Remove (s);
      _tabControl.Items.Add (browserTab);
      _preparer.NewTab -= OnNewTab;
    }

    private void OnNewPopup (object sender, NewPopupEventArgs eventArgs)
    {
      var popUp = eventArgs.PopUpWindow;
      var browser = popUp.WebBrowser;
      browser.WindowOpen += OnWindowOpen; // TODO what?!
      _preparer.NewPopup -= OnNewPopup;
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      NewTab (_urlTextBox.Text);
    }
  }
}