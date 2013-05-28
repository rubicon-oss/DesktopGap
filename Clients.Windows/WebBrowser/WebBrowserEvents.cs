using System;
using System.Diagnostics;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.Clients.Windows.WebBrowser
{
  public class WebBrowserEvents : WebBrowserEventsBase
  {
    private readonly TridentWebBrowser _browserControl;
    private readonly IUrlFilter _thirdPartyUrlFilter;


    public WebBrowserEvents (TridentWebBrowser browserControl, IUrlFilter thirdPartyUrlFilter)
    {
      ArgumentUtility.CheckNotNull ("browserControl", browserControl);
      ArgumentUtility.CheckNotNull ("thirdPartyUrlFilter", thirdPartyUrlFilter);

      _browserControl = browserControl;
      _thirdPartyUrlFilter = thirdPartyUrlFilter;
    }


    public override void BeforeNavigate2 (
        object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
    {
      if (!_thirdPartyUrlFilter.IsAllowed (URL.ToString()))
      {
        Uri uri;
        if(Uri.TryCreate(URL.ToString(), UriKind.RelativeOrAbsolute, out uri))
          Process.Start (uri.ToString());
        Cancel = true;
        return;
      }

      var target = String.Empty;
      if (TargetFrameName != null)
        target = TargetFrameName.ToString();

      var eventArgs = new NavigationEventArgs (BrowserWindowStartMode.Active, Cancel, URL.ToString(), target, BrowserWindowTarget.Tab);

      _browserControl.OnBeforeNavigate (eventArgs);

      Cancel = eventArgs.Cancel;
    }


    public override void WindowSetResizable (bool Resizable)
    {
      _browserControl.OnWindowSetResizable (Resizable);
    }

    public override void WindowSetHeight (int height)
    {
      _browserControl.OnWindowSetHeight (height);
    }

    public override void WindowSetLeft (int left)
    {
      _browserControl.OnWindowSetLeft (left);
    }

    public override void WindowSetTop (int top)
    {
      _browserControl.OnWindowSetTop (top);
    }

    public override void WindowSetWidth (int width)
    {
      _browserControl.OnWindowSetWidth (width);
    }

    // TODO not pretty
    public override void NewWindow2 (ref object ppDisp, ref bool Cancel)
    {
      NewWindow3 (ref ppDisp, ref Cancel, 0, String.Empty, String.Empty);
    }

    public override void NewWindow3 (ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    {
      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (BrowserWindowTarget.PopUp, Cancel, new Uri(bstrUrl, UriKind.Absolute));

      _browserControl.OnNewWindow (eventArgs);

      if (eventArgs.TargetView != null)
        ppDisp = ((TridentWebBrowserBase) eventArgs.TargetView).Application ?? ppDispOriginal;

      Cancel = eventArgs.Cancel;
    }

    public override void DocumentComplete (object pDisp, ref object URL)
    {
      if (URL != null)
        _browserControl.OnDocumentComplete (URL.ToString());
    }
  }
}