using System;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.WebBrowser.View
{
  public interface IWebBrowserView : IDisposable
  {
    event EventHandler<EventArgs> Closing;

    IExtendedWebBrowser WebBrowser { get; }

    Guid Identifier { get; }

    void Show (BrowserWindowStartMode startMode);

    void Close (out bool cancel);
  }
}