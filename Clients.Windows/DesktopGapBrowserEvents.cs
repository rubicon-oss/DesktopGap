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
using DesktopGap.WebBrowser.EventArguments;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// DesktopGap's custom events.
  /// </summary>
  public class DesktopGapBrowserEvents : WebBrowserEventsBase
  {
    /// <summary>
    /// The corresponding WebBrowser control
    /// </summary>
    private readonly TridentWebBrowser _browserControl;

    public DesktopGapBrowserEvents (TridentWebBrowser browserControl)
    {
      _browserControl = browserControl;
    }

    public override void NavigateComplete2 (object pDisp, ref object URL)
    {
      _browserControl.OnDocumentLoaded();
    }

    public override void DownloadComplete ()
    {
      _browserControl.OnLoadFinished();
    }

    public override void NewWindow2 (ref object ppDisp, ref bool Cancel)
    {
      //NewWindow3 (ref ppDisp, ref Cancel, 0, null, null); // TODO what to do here?
    }

    public override void NewWindow3 (ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrl", bstrUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("bstrUrlContext", bstrUrlContext);


      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (false, Cancel, bstrUrl, false, "");
      _browserControl.OnNewWindow (eventArgs);
      if (eventArgs.TargetWindow != null)
        ppDisp = (eventArgs.TargetWindow as TridentWebBrowserBase).Application ?? ppDispOriginal; // TODO change to a new DesktopGap Window
      Cancel = eventArgs.Cancel;
    }
  }
}