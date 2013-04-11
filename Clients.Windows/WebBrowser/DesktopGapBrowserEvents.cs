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
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Clients.Windows.WebBrowser.UI;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.Clients.Windows.WebBrowser
{
  /// <summary>
  /// DesktopGap's custom events.
  /// </summary>
  public class DesktopGapWebBrowserEvents : WebBrowserEventsBase
  {
    private const string c_ModalTargetPrefix = "_modal";
    private const string c_BackgroundTargetPrefix = "_background";


    /// <summary>
    /// The corresponding WebBrowser control
    /// </summary>
    private readonly TridentWebBrowser _browserControl;

    public DesktopGapWebBrowserEvents (TridentWebBrowser browserControl)
    {
      _browserControl = browserControl;
    }


    public override void BeforeNavigate2 (
        object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
    {
      var mode = BrowserWindowStartMode.Active;
      var target = String.Empty;
      if (TargetFrameName != null)
      {
        target = TargetFrameName.ToString();

        switch (target.ToLower())
        {
          case c_ModalTargetPrefix:
            mode = BrowserWindowStartMode.Modal;
            break;
          case c_BackgroundTargetPrefix:
            mode = BrowserWindowStartMode.InBackground;
            break;
        }
      }
      var eventArgs = new NavigationEventArgs (mode, Cancel, URL.ToString(), target);

      _browserControl.OnBeforeNavigate (eventArgs);

      Cancel = eventArgs.Cancel;
    }


    public override void DownloadComplete ()
    {
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
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrl", bstrUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrlContext", bstrUrlContext);

      var targetControl = BrowserWindowTarget.Tab;
      switch ((WindowOpenParameters) dwFlags)
      {
          // DO NOTHING ON THESE FLAGS (= open in a tab)
          //case WindowOpenParameters.None:
          //case WindowOpenParameters.Unloading:
          //case WindowOpenParameters.UserInitiated:
          //case WindowOpenParameters.First:
          //case WindowOpenParameters.OverrideKey:
          //case WindowOpenParameters.ShowHelp:
          //case WindowOpenParameters.ForceTab:
          //case WindowOpenParameters.SuggestWindow:
          //case WindowOpenParameters.SuggestTab:
          //case WindowOpenParameters.FromDialogChild:
          //case WindowOpenParameters.UserRequested:
          //case WindowOpenParameters.UserAllowed:

        case WindowOpenParameters.UserInitiated | WindowOpenParameters.First | WindowOpenParameters.SuggestWindow:
        case WindowOpenParameters.UserInitiated | WindowOpenParameters.First | WindowOpenParameters.HtmlDialog:
        case WindowOpenParameters.InactiveTab | WindowOpenParameters.HtmlDialog:
        case WindowOpenParameters.InactiveTab | WindowOpenParameters.SuggestWindow:

          targetControl = BrowserWindowTarget.PopUp;
          break;
      }

      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (targetControl, Cancel, bstrUrl);
      _browserControl.OnNewWindow (eventArgs);
      if (eventArgs.TargetView != null)
        ppDisp = ((TridentWebBrowserBase) eventArgs.TargetView.WebBrowser).Application ?? ppDispOriginal;
      Cancel = eventArgs.Cancel;
    }
  }
}