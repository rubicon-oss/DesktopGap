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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DesktopGap.Clients.Windows.TridentWebBrowser.Low.Common;
using DesktopGap.Clients.Windows.TridentWebBrowser.Low.UIHandler;

namespace DesktopGap.Clients.Windows.TridentWebBrowser.Defaults
{
  /// <summary>
  /// Provides a implementation for all functions of the interface IDocHostUIHandler
  /// Most of the methods are "not implemented" which means that the default implementation is used
  /// 
  /// It's possible to ovveride one ore more methods of this class, to provide a custom implementations
  /// 
  /// For more info go to: 
  /// http://msdn.microsoft.com/en-us/library/aa753260%28v=vs.85%29.aspx
  /// </summary>
  public abstract class DefaultDocHostUIHandler : IDocHostUIHandler
  {
    
    private readonly System.Windows.Forms.WebBrowser _browser;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="browser">The Browser Object for which the IDocHostUIHandler should be registered</param>
    protected DefaultDocHostUIHandler (System.Windows.Forms.WebBrowser browser)
    {
      _browser = browser;
    }

    /// <summary>
    /// Enables you to provide a custom context menu, or hide the original one
    /// </summary>
    /// <param name="dwID">A DWORD that specifies the identifier of the shortcut menu to be displayed. These values are defined in Mshtmhst.h. </param>
    /// <param name="pt">A pointer to a POINT structure containing the screen coordinates for the menu.</param>
    /// <param name="pcmdtReserved">A pointer to the IUnknown of an IOleCommandTarget interface used to query command status and execute commands on this object.</param>
    /// <param name="pdispReserved"> A pointer to an IDispatch interface of the object at the screen coordinates specified in ppt. This enables a host to pass particular objects, such as anchor tags and images, to provide more specific context.</param>
    /// <returns>Returns one of the following values.
    /// S_OK 	Host displayed its UI. MSHTML will not attempt to display its UI.
    /// S_FALSE 	Host did not display its UI. MSHTML will display its UI.
    /// DOCHOST_E_UNKNOWN 	Menu identifier is unknown. MSHTML might attempt an alternative identifier from a previous version.
    /// </returns>
    public virtual int ShowContextMenu (uint dwID, ref tagPOINT pt, object pcmdtReserved, object pdispReserved)
    {
      return !_browser.IsWebBrowserContextMenuEnabled ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    /// <summary>
    /// Gets the UI capabilities of the application that is hosting MSHTML.
    /// </summary>
    /// <param name="info">A pointer to a DOCHOSTUIINFO structure that receives the host's UI capabilities.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.></returns>
    public virtual int GetHostInfo (ref DOCHOSTUIINFO info)
    {
      info.cbSize = (uint) Marshal.SizeOf (info);
      info.dwDoubleClick = (uint) DOCHOSTUIDBLCLK.DOCHOSTUIDBLCLK_DEFAULT;
      info.dwFlags = (uint) (DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DBORDER | DOCHOSTUIFLAG.DOCHOSTUIFLAG_FLAT_SCROLLBAR | DOCHOSTUIFLAG.DOCHOSTUIFLAG_THEME);
      return HRESULT.S_OK;
    }

    /// <summary>
    /// Enables the host to replace MSHTML menus and toolbars.
    /// </summary>
    /// <param name="dwID">A DWORD that receives a DOCHOSTUITYPE value that indicates the type of UI. </param>
    /// <param name="activeObject">A pointer to an IOleInPlaceActiveObject interface for the active object.</param>
    /// <param name="commandTarget">A pointer to an IOleCommandTarget interface for the object.</param>
    /// <param name="frame">A pointer to an IOleInPlaceFrame interface for the object. Menus and toolbars must use this parameter.</param>
    /// <param name="doc">A pointer to an IOleInPlaceUIWindow interface for the object. Toolbars must use this parameter.</param>
    /// <returns>
    /// Returns one of the following values.
    ///S_OK 	Host displays its UI. MSHTML will not display its UI.
    ///S_FALSE 	Host did not display its UI. MSHTML will display its UI.
    ///DOCHOST_E_UNKNOWN 	Host did not recognize the UI identifier. MSHTML will either try an alternative identifier for compatibility with a previous version, or display its UI.       
    /// </returns>
    public virtual int ShowUI (
        int dwID, IOleInPlaceActiveObject activeObject, IOleCommandTarget commandTarget, IOleInPlaceFrame frame, IOleInPlaceUIWindow doc)
    {
      return HRESULT.S_OK;
    }

    /// <summary>
    /// Enables the host to remove its menus and toolbars.
    /// </summary>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int HideUI ()
    {
      return HRESULT.S_OK;
    }

    /// <summary>
    /// Notifies the host that the command state has changed.
    /// </summary>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int UpdateUI ()
    {
      return HRESULT.S_OK;
    }

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::EnableModeless. Also called when MSHTML displays a modal UI.
    /// </summary>
    /// <param name="fEnable"> A BOOL that indicates if the host's modeless dialog boxes are enabled or disabled.
    ///TRUE
    ///  Modeless dialog boxes are enabled.
    ///FALSE
    /// Modeless dialog boxes are disabled.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int EnableModeless (bool fEnable)
    {
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject.OnDocWindowActivate
    /// </summary>
    /// <param name="fActivate">
    /// TRUE
    /// The window is activated.
    ///FALSE
    /// The window is deactivated.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int OnDocWindowActivate (bool fActivate)
    {
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::OnFrameWindowActivate.
    /// </summary>
    /// <param name="fActivate">
    /// TRUE
    /// The frame window is activated.
    ///FALSE
    /// The frame window is deactivated.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int OnFrameWindowActivate (bool fActivate)
    {
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::ResizeBorder.
    /// </summary>
    /// <param name="rect">A constant pointer to a RECT for the new outer rectangle of the border.</param>
    /// <param name="doc"> A pointer to an IOleInPlaceUIWindow interface for the frame or document window whose border is to be changed.</param>
    /// <param name="fFrameWindow">A BOOL that is TRUE if the frame window is calling IDocHostUIHandler.ResizeBorder, or FALSE otherwise. </param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int ResizeBorder (ref tagRECT rect, IOleInPlaceUIWindow doc, bool fFrameWindow)
    {
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Called by MSHTML when IOleInPlaceActiveObject.TranslateAccelerator or IOleControlSite.TranslateAccelerator is called.
    /// </summary>
    /// <param name="msg">A pointer to a MSG structure that specifies the message to be translated.</param>
    /// <param name="group">A pointer to a GUID for the command group identifier.</param>
    /// <param name="nCmdID">A DWORD that specifies a command identifier.</param>
    /// <returns>Returns one of the following values.
    ///S_OK 	The message was handled. Prevent the host default behavior.
    ///S_FALSE 	The message was not handled. Host default behavior is allowed. </returns>
    public virtual int TranslateAccelerator (ref tagMSG msg, ref Guid group, uint nCmdID)
    {
      return HRESULT.S_FALSE;
    }

    /// <summary>
    /// Gets a registry subkey path that overrides the default Windows Internet Explorer registry settings.
    /// </summary>
    /// <param name="pbstrKey">A pointer to an LPOLESTR that receives the registry subkey string where the host stores its registry settings.</param>
    /// <param name="dw">Reserved. Must be set to NULL.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int GetOptionKeyPath (out string pbstrKey, uint dw)
    {
      pbstrKey = null;
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Enables the host to supply an alternative IDropTarget interface.
    /// </summary>
    /// <param name="pDropTarget">A pointer to an IDropTarget interface for the current drop target object supplied by MSHTML.</param>
    /// <param name="ppDropTarget">A pointer variable that receives an IDropTarget interface pointer for the alternative drop target object supplied by the host.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int GetDropTarget (IDropTarget pDropTarget, out IDropTarget ppDropTarget)
    {
      ppDropTarget = pDropTarget;
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Gets the host's IDispatch interface.
    /// </summary>
    /// <param name="ppDispatch">A pointer to a variable that receives an IDispatch interface pointer for the host application.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    public virtual int GetExternal (out object ppDispatch)
    {
      if (_browser.ObjectForScripting != null)
      {
        ppDispatch = _browser.ObjectForScripting;
        return HRESULT.S_OK;
      }

      ppDispatch = null;
      return HRESULT.E_NOTIMPL;
    }

    /// <summary>
    /// Enables the host to modify the URL to be loaded.
    /// </summary>
    /// <param name="dwTranslate">Reserved. Must be set to NULL.</param>
    /// <param name="strURLIn">A pointer to an OLECHAR that specifies the current URL for navigation.</param>
    /// <param name="pstrURLOut">A pointer variable that receives an OLECHAR pointer containing the new URL.</param>
    /// <returns>Returns S_OK if the URL was translated, or S_FALSE if the URL was not translated. </returns>
    public virtual int TranslateUrl (uint dwTranslate, string strURLIn, out string pstrURLOut)
    {
      pstrURLOut = null;
      return HRESULT.S_FALSE;
    }

    /// <summary>
    /// Enables the host to replace the MSHTML data object.
    /// </summary>
    /// <param name="pDO">A pointer to an IDataObject interface supplied by MSHTML.</param>
    /// <param name="ppDORet"> A pointer variable that receives an IDataObject interface pointer supplied by the host.</param>
    /// <returns>Returns S_OK if the data object is replaced, or S_FALSE if it is not replaced.</returns>
    public virtual int FilterDataObject (IDataObject pDO, out IDataObject ppDORet)
    {
      ppDORet = null;
      return HRESULT.S_FALSE;
    }

  }
}