﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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

namespace DesktopGap.WebBrowser.EventArguments
{
  public class WindowOpenEventArgs
  {
    public bool IsModal { get; private set; }

    public BrowserWindowStartMode StartMode { get; private set; }

    public bool IsInBackground { get; private set; }

    public bool Cancel { get; set; }

    public IExtendedWebBrowser TargetWindow { get; set; }

    public string URL { get; private set; }

    public string Target { get; private set; }

    /// <summary>
    /// C'tor for the WindowOpen event arguments.
    /// </summary>
    /// <param name="mode"> </param>
    /// <param name="cancel">Should the action be cancelled?</param>
    /// <param name="targetURL">The URL the window is going to open.</param>
    /// <param name="target"> </param>
    /// <param name="targetWindow">Where should the new window be openend? (null means new DesktopGap window)</param>
    public WindowOpenEventArgs (BrowserWindowStartMode mode, bool cancel, string targetURL, string target, IExtendedWebBrowser targetWindow = null)
    {
      IsModal = mode == BrowserWindowStartMode.ModalPopUp;
      Cancel = cancel;
      URL = targetURL;
      IsInBackground = mode == BrowserWindowStartMode.BackgroundTab;
      TargetWindow = targetWindow;
      Target = target;
      StartMode = mode;
    }
  }
}