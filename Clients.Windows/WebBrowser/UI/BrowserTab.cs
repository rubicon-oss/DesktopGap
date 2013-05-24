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
    public event EventHandler<EventArgs> Closing;

    private readonly WebBrowserHost _webBrowserHost;

    public BrowserTab (TridentWebBrowser webBrowser, Guid identifier, bool isCloseable = true)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);
      Identifier = identifier;
      webBrowser.DocumentTitleChanged += OnDocumentTitleChanged;
      IsCloseable = isCloseable;
      _webBrowserHost = new WebBrowserHost (webBrowser);
      ;
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
      Header = new CloseableTabHeader (
          _webBrowserHost.WebBrowser.Url != null ? _webBrowserHost.WebBrowser.Url.ToString() : string.Empty, true);

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
      if (Closing == null)
        return;

      Closing (this, e);

      CleanUp();
    }

    private void CleanUp ()
    {
      _webBrowserHost.Dispose();
    }
  }
}