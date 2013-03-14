// This file is part of DesktopGap (desktopgap.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Windows.Forms;
using DesktopGap.Browser;
using DesktopGap.Clients.Windows.TridentWebBrowser.Defaults;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// DesktopGap's custom events.
  /// </summary>
  public class DesktopGapBrowserEvents : DefaultWebBrowserEvents
  {
    /// <summary>
    /// The corresponding WebBrowser control
    /// </summary>
    private readonly ExtendedTridentWebBrowser _browserControl;

    public DesktopGapBrowserEvents (ExtendedTridentWebBrowser browserControl)
    {
      _browserControl = browserControl;
    }

    public override void DownloadComplete ()
    {
      _browserControl.OnLoadFinished();
    }

    public override void NewWindow2 (ref object ppDisp, ref bool Cancel)
    {
      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (false, Cancel, null, false);
      _browserControl.OnNewWindow (eventArgs);
      ppDisp = (WebBrowser) eventArgs.TargetWindow ?? ppDispOriginal; // TODO change to a new DesktopGap Window
      Cancel = eventArgs.Cancel;
    }

    public override void NewWindow3 (ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    {
      var ppDispOriginal = ppDisp;
      var eventArgs = new WindowOpenEventArgs (false, Cancel, bstrUrl, false);
      _browserControl.OnNewWindow (eventArgs);
      ppDisp = (WebBrowser) eventArgs.TargetWindow ?? ppDispOriginal; // TODO change to a new DesktopGap Window
      Cancel = eventArgs.Cancel;

    }
  }
}