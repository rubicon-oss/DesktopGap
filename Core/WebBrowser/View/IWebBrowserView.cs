using System;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.WebBrowser.View
{
  public interface IWebBrowserView : IDisposable
  {
    event EventHandler<EventArgs> BeforeClose;

    IExtendedWebBrowser WebBrowser { get; }

    Guid Identifier { get; }

    void Show (BrowserWindowStartMode startMode);

    bool ShouldClose ();

    void CloseView ();
  }
}