// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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
using DesktopGap.Security;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.WebBrowser.Arguments
{
  public class NewViewEventArgs : EventArgs
  {
    public BrowserWindowStartMode StartMode { get; private set; }
    public TargetAddressType AddressType { get; private set; }
    public IWebBrowserView View { get; private set; }

    public NewViewEventArgs (IWebBrowserView view, BrowserWindowStartMode startMode, TargetAddressType addressType)
    {
      ArgumentUtility.CheckNotNull ("view", view);
      StartMode = startMode;
      AddressType = addressType;
      View = view;
    }
  }
}