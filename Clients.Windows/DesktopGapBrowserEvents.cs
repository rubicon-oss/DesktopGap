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
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// DesktopGap's custom events.
  /// </summary>
  public class DesktopGapWebBrowserEvents : WebBrowserEventsBase
  {
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
      BrowserWindowStartMode mode = BrowserWindowStartMode.Self;
      var target = "";
      if (TargetFrameName != null)
      {
        target = TargetFrameName.ToString();

        switch (target.ToLower()) // TODO redo this
        {
          case "_modal":
            mode = BrowserWindowStartMode.ModalPopUp;
            break;
          case "_backgroundtab":
            mode = BrowserWindowStartMode.BackgroundTab;
            break;

          case "_tab":
            mode = BrowserWindowStartMode.Tab;
            break;
          case "_popup":
            mode = BrowserWindowStartMode.PopUp;
            break;
        }
      }

      var eventArgs = new WindowOpenEventArgs (mode, Cancel, URL.ToString(), target);
      _browserControl.OnBeforeNavigate (eventArgs);

      Cancel = eventArgs.Cancel;
    }

    // TODO not pretty
    public override void NewWindow2 (ref object ppDisp, ref bool Cancel)
    {
      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (BrowserWindowStartMode.Tab, Cancel, "", "");
      _browserControl.OnNewWindow (eventArgs);
      if (eventArgs.TargetWindow != null)
        ppDisp = (eventArgs.TargetWindow as TridentWebBrowserBase).Application ?? ppDispOriginal; // TODO change to a new DesktopGap Window
      Cancel = eventArgs.Cancel;
    }

    public override void NewWindow3 (ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrl", bstrUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrlContext", bstrUrlContext);


      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (BrowserWindowStartMode.Tab, Cancel, bstrUrl, "");
      _browserControl.OnNewWindow (eventArgs);
      if (eventArgs.TargetWindow != null)
        ppDisp = (eventArgs.TargetWindow as TridentWebBrowserBase).Application ?? ppDispOriginal; // TODO change to a new DesktopGap Window
      Cancel = eventArgs.Cancel;
    }
  }
}