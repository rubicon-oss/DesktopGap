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
using System.Security.Permissions;
using System.Windows.Forms;
using DesktopGap.Interfaces;


namespace DesktopGap.Clients.Windows
{
  public class ExtendedTridentWebBrowser : WebBrowser, IExtendedWebBrowser
  {
    public ExtendedTridentWebBrowser ()
    {
      this.Navigate ("http://www.google.com/");
    }

    public string Title
    {
      get { return Document != null ? Document.Title : String.Empty; }
    }

    #region Requirements for acting like the WebBrowser Component. see http://www.codeproject.com/Articles/13598/Extended-NET-2-0-WebBrowser-Control

    /// <summary>
    /// Object for returning the basic scripting interface when the .NET Framework demands it (Application property)
    /// </summary>
    private IWebBrowser2 _axIWebBrowser2;

    /// <summary>
    /// Retrieve the _axIWebBrowser2 implementation from the .NET WebBrowser. 
    /// </summary>
    /// <param name="nativeActiveXObject"></param>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void
        AttachInterfaces (object nativeActiveXObject)
    {
      this._axIWebBrowser2 =
          (IWebBrowser2) nativeActiveXObject;
      base.AttachInterfaces (nativeActiveXObject);
    }

    /// <summary>
    /// Clean up properly after the interface is detached.
    /// </summary>
    [PermissionSet (SecurityAction.LinkDemand, Name = "FullTrust")]
    protected override void DetachInterfaces ()
    {
      this._axIWebBrowser2 = null;
      base.DetachInterfaces();
    }

    /// <summary>
    /// Property that offers the scripting interface (required on connecting any other interface)
    /// </summary>
    public object Application
    {
      get { return _axIWebBrowser2.Application; }
    }
    #endregion
  }
}