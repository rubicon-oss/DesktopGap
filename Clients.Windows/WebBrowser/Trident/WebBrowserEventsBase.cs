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
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  /// <summary>
  /// Provides a default implementation of the events interface(s) for easier handling. The default implementations don't do anything but waiting to be overridden.
  /// </summary>
  public abstract class WebBrowserEventsBase : DWebBrowserEvents2
  {
    public virtual void StatusTextChange (string Text)
    {
    }

    public virtual void ProgressChange (int Progress, int ProgressMax)
    {
    }

    public virtual void CommandStateChange (int Command, bool Enable)
    {
    }

    public virtual void DownloadBegin ()
    {
    }

    public virtual void DownloadComplete ()
    {
    }

    public virtual void TitleChange (string Text)
    {
    }

    public virtual void PropertyChange (string szProperty)
    {
    }

    public virtual void BeforeNavigate2 (
        object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
    {
    }

    public virtual void NewWindow2 (ref object ppDisp, ref bool Cancel)
    {
    }

    public virtual void NavigateComplete2 (object pDisp, ref object URL)
    {
    }

    public virtual void DocumentComplete (object pDisp, ref object URL)
    {
    }

    public virtual void OnQuit ()
    {
    }

    public virtual void OnVisible (bool Visible)
    {
    }

    public virtual void OnToolBar (bool ToolBar)
    {
    }

    public virtual void OnMenuBar (bool MenuBar)
    {
    }

    public virtual void OnStatusBar (bool StatusBar)
    {
    }

    public virtual void OnFullScreen (bool FullScreen)
    {
    }

    public virtual void OnTheaterMode (bool TheaterMode)
    {
    }

    public virtual void WindowSetResizable (bool Resizable)
    {
    }

    public virtual void WindowSetLeft (int Left)
    {
    }

    public virtual void WindowSetTop (int Top)
    {
    }

    public virtual void WindowSetWidth (int Width)
    {
    }

    public virtual void WindowSetHeight (int Height)
    {
    }

    public virtual void WindowClosing (bool IsChildWindow, ref bool Cancel)
    {
    }

    public virtual void ClientToHostWindow (ref int CX, ref int CY)
    {
    }

    public virtual void SetSecureLockIcon (int SecureLockIcon)
    {
    }

    public virtual void FileDownload (ref bool Cancel)
    {
    }

    public virtual void NavigateError (object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
    {
    }

    public virtual void PrintTemplateInstantiation (object pDisp)
    {
    }

    public virtual void PrintTemplateTeardown (object pDisp)
    {
    }

    public virtual void UpdatePageStatus (object pDisp, ref object nPage, ref object fDone)
    {
    }

    public virtual void PrivacyImpactedStateChange (bool bImpacted)
    {
    }

    public virtual void NewWindow3 (ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    {
    }


  }
}
