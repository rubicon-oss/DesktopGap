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

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler
{
  public enum DOCHOSTUITYPE
  {
    DOCHOSTUITYPE_BROWSE = 0,
    DOCHOSTUITYPE_AUTHOR = 1
  }

  public enum DOCHOSTUIDBLCLK
  {
    DOCHOSTUIDBLCLK_DEFAULT = 0,
    DOCHOSTUIDBLCLK_SHOWPROPERTIES = 1,
    DOCHOSTUIDBLCLK_SHOWCODE = 2
  }

  [Flags]
  public enum DOCHOSTUIFLAG
  {
    DOCHOSTUIFLAG_DIALOG = 0x00000001,
    DOCHOSTUIFLAG_DISABLE_HELP_MENU = 0x00000002,
    DOCHOSTUIFLAG_NO3DBORDER = 0x00000004,
    DOCHOSTUIFLAG_SCROLL_NO = 0x00000008,
    DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE = 0x00000010,
    DOCHOSTUIFLAG_OPENNEWWIN = 0x00000020,
    DOCHOSTUIFLAG_DISABLE_OFFSCREEN = 0x00000040,
    DOCHOSTUIFLAG_FLAT_SCROLLBAR = 0x00000080,
    DOCHOSTUIFLAG_DIV_BLOCKDEFAULT = 0x00000100,
    DOCHOSTUIFLAG_ACTIVATE_CLIENTHIT_ONLY = 0x00000200,
    DOCHOSTUIFLAG_OVERRIDEBEHAVIORFACTORY = 0x00000400,
    DOCHOSTUIFLAG_CODEPAGELINKEDFONTS = 0x00000800,
    DOCHOSTUIFLAG_URL_ENCODING_DISABLE_UTF8 = 0x00001000,
    DOCHOSTUIFLAG_URL_ENCODING_ENABLE_UTF8 = 0x00002000,
    DOCHOSTUIFLAG_ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
    DOCHOSTUIFLAG_ENABLE_INPLACE_NAVIGATION = 0x00010000,
    DOCHOSTUIFLAG_IME_ENABLE_RECONVERSION = 0x00020000,
    DOCHOSTUIFLAG_THEME = 0x00040000,
    DOCHOSTUIFLAG_NOTHEME = 0x00080000,
    DOCHOSTUIFLAG_NOPICS = 0x00100000,
    DOCHOSTUIFLAG_NO3DOUTERBORDER = 0x00200000,
    DOCHOSTUIFLAG_DISABLE_EDIT_NS_FIXUP = 0x400000,
    DOCHOSTUIFLAG_LOCAL_MACHINE_ACCESS_CHECK = 0x800000,
    DOCHOSTUIFLAG_DISABLE_UNTRUSTEDPROTOCOL = 0x1000000
  }

  [StructLayout (LayoutKind.Sequential)]
  public struct DOCHOSTUIINFO
  {
    public uint cbSize;
    public uint dwFlags;
    public uint dwDoubleClick;

    [MarshalAs (UnmanagedType.BStr)]
    public string pchHostCss;

    [MarshalAs (UnmanagedType.BStr)]
    public string pchHostNS;
  }

  [StructLayout (LayoutKind.Sequential)]
  public struct tagMSG
  {
    public IntPtr hwnd;
    public uint message;
    public uint wParam;
    public int lParam;
    public uint time;
    public tagPOINT pt;
  }

  [StructLayout (LayoutKind.Sequential, Pack = 4)]
  public struct tagRECT

  {
    public int left;
    public int top;
    public int right;
    public int bottom;
  }

  [StructLayout (LayoutKind.Sequential, Pack = 4)]
  public struct tagPOINT

  {
    public int X;
    public int Y;
  }

  /// <summary>
  /// DocHostUIHandler for modifying menus, toolbars, and context menus, as well as drag and drop animations/implementations.
  /// 
  /// http://msdn.microsoft.com/en-us/library/aa753260%28v=vs.85%29.aspx
  /// </summary>
  [ComVisible (true)]
  [ComImport]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  [Guid ("bd3f23c0-d43e-11cf-893b-00aa00bdce1a")]
  public interface IDocHostUIHandler
  {
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
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ShowContextMenu (
        [In] [MarshalAs (UnmanagedType.U4)] uint dwID,
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagPOINT pt,
        [In] [MarshalAs (UnmanagedType.IUnknown)] object pcmdtReserved,
        [In] [MarshalAs (UnmanagedType.IDispatch)] object pdispReserved);

    /// <summary>
    /// Gets the UI capabilities of the application that is hosting MSHTML.
    /// </summary>
    /// <param name="info">A pointer to a DOCHOSTUIINFO structure that receives the host's UI capabilities.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.></returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetHostInfo ([In] [Out] [MarshalAs (UnmanagedType.Struct)] ref DOCHOSTUIINFO info);

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
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ShowUI (
        [In] [MarshalAs (UnmanagedType.I4)] int dwID,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceActiveObject activeObject,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleCommandTarget commandTarget,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceFrame frame,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceUIWindow doc);


    /// <summary>
    /// Enables the host to remove its menus and toolbars.
    /// </summary>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int HideUI ();

    /// <summary>
    /// Notifies the host that the command state has changed.
    /// </summary>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int UpdateUI ();

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::EnableModeless. Also called when MSHTML displays a modal UI.
    /// </summary>
    /// <param name="fEnable"> A BOOL that indicates if the host's modeless dialog boxes are enabled or disabled.
    ///TRUE
    ///  Modeless dialog boxes are enabled.
    ///FALSE
    /// Modeless dialog boxes are disabled.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int EnableModeless (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fEnable);

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject.OnDocWindowActivate
    /// </summary>
    /// <param name="fActivate">
    /// TRUE
    /// The window is activated.
    ///FALSE
    /// The window is deactivated.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnDocWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::OnFrameWindowActivate.
    /// </summary>
    /// <param name="fActivate">
    /// TRUE
    /// The frame window is activated.
    ///FALSE
    /// The frame window is deactivated.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnFrameWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    /// <summary>
    /// Called by the MSHTML implementation of IOleInPlaceActiveObject::ResizeBorder.
    /// </summary>
    /// <param name="rect">A constant pointer to a RECT for the new outer rectangle of the border.</param>
    /// <param name="doc"> A pointer to an IOleInPlaceUIWindow interface for the frame or document window whose border is to be changed.</param>
    /// <param name="fFrameWindow">A BOOL that is TRUE if the frame window is calling IDocHostUIHandler.ResizeBorder, or FALSE otherwise. </param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ResizeBorder (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagRECT rect,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceUIWindow doc,
        [In] [MarshalAs (UnmanagedType.Bool)] bool fFrameWindow);


    /// <summary>
    /// Called by MSHTML when IOleInPlaceActiveObject.TranslateAccelerator or IOleControlSite.TranslateAccelerator is called.
    /// </summary>
    /// <param name="msg">A pointer to a MSG structure that specifies the message to be translated.</param>
    /// <param name="group">A pointer to a GUID for the command group identifier.</param>
    /// <param name="nCmdID">A DWORD that specifies a command identifier.</param>
    /// <returns>Returns one of the following values.
    ///S_OK 	The message was handled. Prevent the host default behavior.
    ///S_FALSE 	The message was not handled. Host default behavior is allowed. </returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateAccelerator (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagMSG msg,
        [In] ref Guid group,
        [In] [MarshalAs (UnmanagedType.U4)] uint nCmdID);

    /// <summary>
    /// Gets a registry subkey path that overrides the default Windows Internet Explorer registry settings.
    /// </summary>
    /// <param name="pbstrKey">A pointer to an LPOLESTR that receives the registry subkey string where the host stores its registry settings.</param>
    /// <param name="dw">Reserved. Must be set to NULL.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetOptionKeyPath (
        //out IntPtr pbstrKey,
        [Out] [MarshalAs (UnmanagedType.LPWStr)] out String pbstrKey,
        //[Out, MarshalAs(UnmanagedType.LPArray)] String[] pbstrKey,
        [In] [MarshalAs (UnmanagedType.U4)] uint dw);


    /// <summary>
    /// Enables the host to supply an alternative IDropTarget interface.
    /// </summary>
    /// <param name="pDropTarget">A pointer to an IDropTarget interface for the current drop target object supplied by MSHTML.</param>
    /// <param name="ppDropTarget">A pointer variable that receives an IDropTarget interface pointer for the alternative drop target object supplied by the host.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetDropTarget (
        [In] [MarshalAs (UnmanagedType.Interface)] IDropTarget pDropTarget,
        [Out] [MarshalAs (UnmanagedType.Interface)] out IDropTarget ppDropTarget);

    /// <summary>
    /// Gets the host's IDispatch interface.
    /// </summary>
    /// <param name="ppDispatch">A pointer to a variable that receives an IDispatch interface pointer for the host application.</param>
    /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetExternal (
        [Out] [MarshalAs (UnmanagedType.IDispatch)] out object ppDispatch);


    /// <summary>
    /// Enables the host to modify the URL to be loaded.
    /// </summary>
    /// <param name="dwTranslate">Reserved. Must be set to NULL.</param>
    /// <param name="strURLIn">A pointer to an OLECHAR that specifies the current URL for navigation.</param>
    /// <param name="pstrURLOut">A pointer variable that receives an OLECHAR pointer containing the new URL.</param>
    /// <returns>Returns S_OK if the URL was translated, or S_FALSE if the URL was not translated. </returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateUrl (
        [In] [MarshalAs (UnmanagedType.U4)] uint dwTranslate,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string strURLIn,
        [Out] [MarshalAs (UnmanagedType.LPWStr)] out string pstrURLOut);

    /// <summary>
    /// Enables the host to replace the MSHTML data object.
    /// </summary>
    /// <param name="pDO">A pointer to an IDataObject interface supplied by MSHTML.</param>
    /// <param name="ppDORet"> A pointer variable that receives an IDataObject interface pointer supplied by the host.</param>
    /// <returns>Returns S_OK if the data object is replaced, or S_FALSE if it is not replaced.</returns>
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int FilterDataObject (
        [In] [MarshalAs (UnmanagedType.Interface)] IDataObject pDO,
        [Out] [MarshalAs (UnmanagedType.Interface)] out IDataObject ppDORet);
  }
}