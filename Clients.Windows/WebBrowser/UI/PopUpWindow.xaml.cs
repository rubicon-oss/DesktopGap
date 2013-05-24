using System;
using System.Windows;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for PopUpWindow.xaml
  /// </summary>
  public partial class PopUpWindow : IWebBrowserView
  {
    private readonly WebBrowserHost _browserHost;


    public PopUpWindow (TridentWebBrowser webBrowser, Guid identifier)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      InitializeComponent();

      _browserHost = new WebBrowserHost (webBrowser);
      Content = _browserHost;

      Width = _browserHost.WebBrowser.Width;
      _browserHost.WebBrowser.WindowSetWidth += (s, w) => Width = w;

      Height = _browserHost.WebBrowser.Height;
      _browserHost.WebBrowser.WindowSetHeight += (s, h) => Height = h;

      Left = _browserHost.WebBrowser.Left;
      _browserHost.WebBrowser.WindowSetLeft += (s, l) => Left = l;

      Top = _browserHost.WebBrowser.Top;
      _browserHost.WebBrowser.WindowSetTop += (s, t) => Top = t;

      ResizeMode = _browserHost.WebBrowser.IsResizable ? ResizeMode.CanResize : ResizeMode.NoResize;
      _browserHost.WebBrowser.WindowSetResizable += (s, r) => ResizeMode = r ? ResizeMode.CanResize : ResizeMode.NoResize;

      _browserHost.WebBrowser.DocumentTitleChanged += (s, e) => Title = _browserHost.WebBrowser.Title;

      Identifier = identifier;
      base.Closing += (s, e) =>
                      {
                        if (Closing != null)
                          Closing (s, e);
                      };
    }


    public void Dispose ()
    {
      _browserHost.Dispose();
    }


    public new event EventHandler<EventArgs> Closing;

    public IExtendedWebBrowser WebBrowser
    {
      get { return _browserHost.WebBrowser; }
    }

    public Guid Identifier { get; private set; }


    public void Show (BrowserWindowStartMode startMode)
    {
      switch (startMode)
      {
        case BrowserWindowStartMode.Active:
          Show();
          break;
        case BrowserWindowStartMode.Modal:
          ShowDialog();
          break;
        default:
          throw new InvalidOperationException (string.Format ("Start mode '{0}' is not supported for PopUpWindow.", startMode));
      }
    }
  }
}