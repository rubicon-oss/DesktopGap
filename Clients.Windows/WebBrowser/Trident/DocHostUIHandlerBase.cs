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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DesktopGap.Clients.Windows.OleLibraryDependencies;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  /// <summary>
  /// Provides a implementation for all functions of the interface IDocHostUIHandler
  /// Most of the methods are "not implemented" which means that the default implementation is used
  /// 
  /// Override methods to provide a custom implementations
  /// </summary>
  public abstract class DocHostUIHandlerBase : IDocHostUIHandler
  {
    private readonly System.Windows.Forms.WebBrowser _browser;


    protected DocHostUIHandlerBase (System.Windows.Forms.WebBrowser browser)
    {
      _browser = browser;
    }

   
    public virtual int ShowContextMenu (uint dwID, ref tagPOINT pt, object pcmdtReserved, object pdispReserved)
    {
      return !_browser.IsWebBrowserContextMenuEnabled ? HResult.S_OK : HResult.S_FALSE;
    }

    
    public virtual int GetHostInfo (ref DOCHOSTUIINFO info)
    {
      info.cbSize = (uint) Marshal.SizeOf (info);
      info.dwDoubleClick = (uint) DOCHOSTUIDBLCLK.DOCHOSTUIDBLCLK_DEFAULT;
      info.dwFlags = (uint) (DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DBORDER | DOCHOSTUIFLAG.DOCHOSTUIFLAG_FLAT_SCROLLBAR | DOCHOSTUIFLAG.DOCHOSTUIFLAG_THEME);
      return HResult.S_OK;
    }

  
    public virtual int ShowUI (
        int dwID, IOleInPlaceActiveObject activeObject, IOleCommandTarget commandTarget, IOleInPlaceFrame frame, IOleInPlaceUIWindow doc)
    {
      return HResult.S_OK;
    }

  
    public virtual int HideUI ()
    {
      return HResult.S_OK;
    }

  
    public virtual int UpdateUI ()
    {
      return HResult.S_OK;
    }

  
    public virtual int EnableModeless (bool fEnable)
    {
      return HResult.S_OK;
    }


    public virtual int OnDocWindowActivate (bool fActivate)
    {
      return HResult.S_FALSE;
    }

  
    public virtual int OnFrameWindowActivate (bool fActivate)
    {
      return HResult.S_FALSE;
    }

 
    public virtual int ResizeBorder (ref tagRECT rect, IOleInPlaceUIWindow doc, bool fFrameWindow)
    {
      return HResult.S_FALSE;
    }

    public virtual int TranslateAccelerator (ref tagMSG msg, ref Guid group, uint nCmdID)
    {
      return HResult.S_FALSE;
    }

  
    public virtual int GetOptionKeyPath (out string pbstrKey, uint dw)
    {
      pbstrKey = null;
      return HResult.S_FALSE;
    }

    public virtual int GetDropTarget (IDropTarget pDropTarget, out IDropTarget ppDropTarget)
    {
      ppDropTarget = null;
      return HResult.E_NOTIMPL;
    }

    public virtual int GetExternal (out object ppDispatch)
    {
      if (_browser.ObjectForScripting != null)
      {
        ppDispatch = _browser.ObjectForScripting;
        return HResult.S_OK;
      }

      ppDispatch = null;
      return HResult.S_FALSE;
    }

  
    public virtual int TranslateUrl (uint dwTranslate, string strURLIn, out string pstrURLOut)
    {
      pstrURLOut = null;
      return HResult.S_FALSE;
    }

  
    public virtual int FilterDataObject (IDataObject pDO, out IDataObject ppDORet)
    {
      ppDORet = null;
      return HResult.S_FALSE;
    }
  }
}