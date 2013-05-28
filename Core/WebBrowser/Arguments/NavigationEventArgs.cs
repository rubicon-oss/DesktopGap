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
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.WebBrowser.Arguments
{
  public class NavigationEventArgs : EventArgs
  {
    public BrowserWindowStartMode StartMode { get; set; }

    public BrowserWindowTarget BrowserWindowTarget { get; set; }

    public bool Cancel { get; set; }

    public Uri Url { get; private set; }

    public string TargetName { get; private set; }

    public bool Handled { get; set; }

    public NavigationEventArgs (
        BrowserWindowStartMode startMode,
        bool cancel,
        string targetURL,
        string target,
        BrowserWindowTarget windowTarget)
    {
      StartMode = startMode;
      Cancel = cancel;
      Uri uri;
      if (Uri.TryCreate (targetURL, UriKind.RelativeOrAbsolute, out uri))
        Url = uri;
      TargetName = target;
      BrowserWindowTarget = windowTarget;
    }
  }
}