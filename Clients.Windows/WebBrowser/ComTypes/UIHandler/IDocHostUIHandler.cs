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

  // Added missing definitions of tagRECT/tagPOINT

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
  /// </summary>
  [ComVisible (true)]
  [ComImport]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  [Guid ("bd3f23c0-d43e-11cf-893b-00aa00bdce1a")]
  public interface IDocHostUIHandler
  {
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ShowContextMenu (
        [In] [MarshalAs (UnmanagedType.U4)] uint dwID,
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagPOINT pt,
        [In] [MarshalAs (UnmanagedType.IUnknown)] object pcmdtReserved,
        [In] [MarshalAs (UnmanagedType.IDispatch)] object pdispReserved);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetHostInfo ([In] [Out] [MarshalAs (UnmanagedType.Struct)] ref DOCHOSTUIINFO info);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ShowUI (
        [In] [MarshalAs (UnmanagedType.I4)] int dwID,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceActiveObject activeObject,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleCommandTarget commandTarget,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceFrame frame,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceUIWindow doc);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int HideUI ();

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int UpdateUI ();

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int EnableModeless (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fEnable);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnDocWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnFrameWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ResizeBorder (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagRECT rect,
        [In] [MarshalAs (UnmanagedType.Interface)] IOleInPlaceUIWindow doc,
        [In] [MarshalAs (UnmanagedType.Bool)] bool fFrameWindow);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateAccelerator (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagMSG msg,
        [In] ref Guid group,
        [In] [MarshalAs (UnmanagedType.U4)] uint nCmdID);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetOptionKeyPath (
        //out IntPtr pbstrKey,
        [Out] [MarshalAs (UnmanagedType.LPWStr)] out String pbstrKey,
        //[Out, MarshalAs(UnmanagedType.LPArray)] String[] pbstrKey,
        [In] [MarshalAs (UnmanagedType.U4)] uint dw);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetDropTarget (
        [In] [MarshalAs (UnmanagedType.Interface)] IDropTarget pDropTarget,
        [Out] [MarshalAs (UnmanagedType.Interface)] out IDropTarget ppDropTarget);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetExternal (
        [Out] [MarshalAs (UnmanagedType.IDispatch)] out object ppDispatch);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateUrl (
        [In] [MarshalAs (UnmanagedType.U4)] uint dwTranslate,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string strURLIn,
        [Out] [MarshalAs (UnmanagedType.LPWStr)] out string pstrURLOut);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int FilterDataObject (
        [In] [MarshalAs (UnmanagedType.Interface)] IDataObject pDO,
        [Out] [MarshalAs (UnmanagedType.Interface)] out IDataObject ppDORet);
  }
}