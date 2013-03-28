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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web
{

  // ENUMS required for some function calls....
  public enum tagREADYSTATE
  {
    READYSTATE_UNINITIALIZED = 0,
    READYSTATE_LOADING = 1,
    READYSTATE_LOADED = 2,
    READYSTATE_INTERACTIVE = 3,
    READYSTATE_COMPLETE = 4
  };

  public enum OLECMDEXECOPT
  {
    OLECMDEXECOPT_DODEFAULT = 0,
    OLECMDEXECOPT_PROMPTUSER = 1,
    OLECMDEXECOPT_DONTPROMPTUSER = 2,
    OLECMDEXECOPT_SHOWHELP = 3
  };

  public enum OLECMDF
  {
    OLECMDF_SUPPORTED = 0x1,
    OLECMDF_ENABLED = 0x2,
    OLECMDF_LATCHED = 0x4,
    OLECMDF_NINCHED = 0x8,
    OLECMDF_INVISIBLE = 0x10,
    OLECMDF_DEFHIDEONCTXTMENU = 0x20
  };

  public enum OLECMDID
  {
    OLECMDID_OPEN = 1,
    OLECMDID_NEW = 2,
    OLECMDID_SAVE = 3,
    OLECMDID_SAVEAS = 4,
    OLECMDID_SAVECOPYAS = 5,
    OLECMDID_PRINT = 6,
    OLECMDID_PRINTPREVIEW = 7,
    OLECMDID_PAGESETUP = 8,
    OLECMDID_SPELL = 9,
    OLECMDID_PROPERTIES = 10,
    OLECMDID_CUT = 11,
    OLECMDID_COPY = 12,
    OLECMDID_PASTE = 13,
    OLECMDID_PASTESPECIAL = 14,
    OLECMDID_UNDO = 15,
    OLECMDID_REDO = 16,
    OLECMDID_SELECTALL = 17,
    OLECMDID_CLEARSELECTION = 18,
    OLECMDID_ZOOM = 19,
    OLECMDID_GETZOOMRANGE = 20,
    OLECMDID_UPDATECOMMANDS = 21,
    OLECMDID_REFRESH = 22,
    OLECMDID_STOP = 23,
    OLECMDID_HIDETOOLBARS = 24,
    OLECMDID_SETPROGRESSMAX = 25,
    OLECMDID_SETPROGRESSPOS = 26,
    OLECMDID_SETPROGRESSTEXT = 27,
    OLECMDID_SETTITLE = 28,
    OLECMDID_SETDOWNLOADSTATE = 29,
    OLECMDID_STOPDOWNLOAD = 30,
    OLECMDID_FIND = 32,
    OLECMDID_DELETE = 33,
    OLECMDID_HTTPEQUIV_DONE = 35,
    OLECMDID_ENABLE_INTERACTION = 36,
    OLECMDID_ONUNLOAD = 37,
    OLECMDID_PROPERTYBAG2 = 38,
    OLECMDID_PREREFRESH = 39,
    OLECMDID_SHOWSCRIPTERROR = 40,
    OLECMDID_SHOWMESSAGE = 41,
    OLECMDID_SHOWFIND = 42,
    OLECMDID_SHOWPAGESETUP = 43,
    OLECMDID_SHOWPRINT = 44,
    OLECMDID_CLOSE = 45,
    OLECMDID_ALLOWUILESSSAVEAS = 46,
    OLECMDID_DONTDOWNLOADCSS = 47,
    OLECMDID_UPDATEPAGESTATUS = 48,
    OLECMDID_PRINT2 = 49,
    OLECMDID_PRINTPREVIEW2 = 50,
    OLECMDID_SETPRINTTEMPLATE = 51,
    OLECMDID_GETPRINTTEMPLATE = 52,
    OLECMDID_PAGEACTIONBLOCKED = 55,
    OLECMDID_PAGEACTIONUIQUERY = 56,
    OLECMDID_FOCUSVIEWCONTROLS = 57,
    OLECMDID_FOCUSVIEWCONTROLSQUERY = 58,
    OLECMDID_SHOWPAGEACTIONMENU = 59,
    OLECMDID_ADDTRAVELENTRY = 60,
    OLECMDID_UPDATETRAVELENTRY = 61,
    OLECMDID_UPDATEBACKFORWARDSTATE = 62,
    OLECMDID_OPTICAL_ZOOM = 63,
    OLECMDID_OPTICAL_GETZOOMRANGE = 64,
    OLECMDID_WINDOWSTATECHANGED = 65,
    OLECMDID_ACTIVEXINSTALLSCOPE = 66,
    OLECMDID_UPDATETRAVELENTRY_DATARECOVERY = 67
  };


  /// <summary>
  /// IWebBrowser2 "exposes methods that are implemented by the WebBrowser control (Microsoft ActiveX control) or implemented by an instance of the InternetExplorer application (OLE Automation)" [msdn]. 
  /// Therefore, every method implemented by the WebBrowser Control can be overwritten.
  /// </summary>
       [SuppressUnmanagedCodeSecurity]
    [TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FOleAutomation)]
    [Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E")]
    [ComImport]
    public interface IWebBrowser2
    {
      [DispId(200)]
      object Application { get; }

      [DispId(201)]
      object Parent { get; }

      [DispId(202)]
      object Container { get; }

      [DispId(203)]
      object Document { get; }

      [DispId(204)]
      bool TopLevelContainer { get; }

      [DispId(205)]
      string Type { get; }

      [DispId(206)]
      int Left { get; set; }

      [DispId(207)]
      int Top { get; set; }

      [DispId(208)]
      int Width { get; set; }

      [DispId(209)]
      int Height { get; set; }

      [DispId(210)]
      string LocationName { get; }

      [DispId(211)]
      string LocationURL { get; }

      [DispId(212)]
      bool Busy { get; }

      [DispId(0)]
      string Name { get; }

      [DispId(-515)]
      int HWND { get; }

      [DispId(400)]
      string FullName { get; }

      [DispId(401)]
      string Path { get; }

      [DispId(402)]
      bool Visible { get; set; }

      [DispId(403)]
      bool StatusBar { get; set; }

      [DispId(404)]
      string StatusText { get; set; }

      [DispId(405)]
      int ToolBar { get; set; }

      [DispId(406)]
      bool MenuBar { get; set; }

      [DispId(407)]
      bool FullScreen { get; set; }

      [DispId(-525)]
      WebBrowserReadyState ReadyState { get; }

      [DispId(550)]
      bool Offline { get; set; }

      [DispId(551)]
      bool Silent { get; set; }

      [DispId(552)]
      bool RegisterAsBrowser { get; set; }

      [DispId(553)]
      bool RegisterAsDropTarget { get; set; }

      [DispId(554)]
      bool TheaterMode { get; set; }

      [DispId(555)]
      bool AddressBar { get; set; }

      [DispId(556)]
      bool Resizable { get; set; }

      [DispId(100)]
      void GoBack();

      [DispId(101)]
      void GoForward();

      [DispId(102)]
      void GoHome();

      [DispId(103)]
      void GoSearch();

      [DispId(104)]
      void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);

      [DispId(-550)]
      void Refresh();

      [DispId(105)]
      void Refresh2([In] ref object level);

      [DispId(106)]
      void Stop();

      [DispId(300)]
      void Quit();

      [DispId(301)]
      void ClientToWindow(out int pcx, out int pcy);

      [DispId(302)]
      void PutProperty([In] string property, [In] object vtValue);

      [DispId(303)]
      object GetProperty([In] string property);

      [DispId(500)]
      void Navigate2([In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);

      [DispId(501)]
      OLECMDF QueryStatusWB([In] OLECMDID cmdID);

      [DispId(502)]
      void ExecWB([In] OLECMDID cmdID, [In] OLECMDEXECOPT cmdexecopt, ref object pvaIn, IntPtr pvaOut);

      [DispId(503)]
      void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);
    }
/*  [ComImport]
  [Guid ("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E")]
  [TypeLibType ((short) 0x1050)]
  [DefaultMember ("Name")]
  [SuppressUnmanagedCodeSecurity]
  public interface IWebBrowser2
  {
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (100)]
    void GoBack ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x65)]
    void GoForward ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x66)]
    void GoHome ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x67)]
    void GoSearch ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x68)]
    void Navigate (
        [In] [MarshalAs (UnmanagedType.BStr)] string URL,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object Flags,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object TargetFrameName,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object PostData,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object Headers);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (-550)]
    void Refresh ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x69)]
    void Refresh2 ([In] [MarshalAs (UnmanagedType.Struct)] ref object Level);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x6a)]
    void Stop ();

    [DispId (200)]
    object Application { [return: MarshalAs (UnmanagedType.IDispatch)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (200)]
    get; }

    [DispId (0xc9)]
    object Parent { [return: MarshalAs (UnmanagedType.IDispatch)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xc9)]
    get; }

    [DispId (0xca)]
    object Container { [return: MarshalAs (UnmanagedType.IDispatch)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xca)]
    get; }

    [DispId (0xcb)]
    object Document { [return: MarshalAs (UnmanagedType.IDispatch)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xcb)]
    get; }

    [DispId (0xcc)]
    bool TopLevelContainer { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xcc)]
    get; }

    [DispId (0xcd)]
    string Type { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xcd)]
    get; }

    [DispId (0xce)]
    int Left { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xce)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xce)]
    set; }

    [DispId (0xcf)]
    int Top { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xcf)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xcf)]
    set; }

    [DispId (0xd0)]
    int Width { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd0)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd0)]
    set; }

    [DispId (0xd1)]
    int Height { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd1)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd1)]
    set; }

    [DispId (210)]
    string LocationName { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (210)]
    get; }

    [DispId (0xd3)]
    string LocationURL { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd3)]
    get; }

    [DispId (0xd4)]
    bool Busy { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0xd4)]
    get; }

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (300)]
    void Quit ();

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x12d)]
    void ClientToWindow ([In] [Out] ref int pcx, [In] [Out] ref int pcy);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x12e)]
    void PutProperty ([In] [MarshalAs (UnmanagedType.BStr)] string Property, [In] [MarshalAs (UnmanagedType.Struct)] object vtValue);

    [return: MarshalAs (UnmanagedType.Struct)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x12f)]
    object GetProperty ([In] [MarshalAs (UnmanagedType.BStr)] string Property);

    [DispId (0)]
    string Name { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0)]
    get; }

    [DispId (-515)]
    int HWND { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (-515)]
    get; }

    [DispId (400)]
    string FullName { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (400)]
    get; }

    [DispId (0x191)]
    string Path { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x191)]
    get; }

    [DispId (0x192)]
    bool Visible { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x192)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x192)]
    set; }

    [DispId (0x193)]
    bool StatusBar { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x193)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x193)]
    set; }

    [DispId (0x194)]
    string StatusText { [return: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x194)]
    get; [param: In]
    [param: MarshalAs (UnmanagedType.BStr)]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x194)]
    set; }

    [DispId (0x195)]
    int ToolBar { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x195)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x195)]
    set; }

    [DispId (0x196)]
    bool MenuBar { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x196)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x196)]
    set; }

    [DispId (0x197)]
    bool FullScreen { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x197)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x197)]
    set; }

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (500)]
    void Navigate2 (
        [In] [MarshalAs (UnmanagedType.Struct)] ref object URL,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object Flags,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object TargetFrameName,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object PostData,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object Headers);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x1f5)]
    OLECMDF QueryStatusWB ([In] OLECMDID cmdId);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x1f6)]
    void ExecWB (
        [In] OLECMDID cmdID,
        [In] OLECMDEXECOPT cmdexecopt,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object pvaIn,
        [In] [Out] [MarshalAs (UnmanagedType.Struct)] ref object pvaOut);

    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x1f7)]
    void ShowBrowserBar (
        [In] [MarshalAs (UnmanagedType.Struct)] ref object pvaClsid,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object pvarShow,
        [In] [MarshalAs (UnmanagedType.Struct)] ref object pvarSize);

    [DispId (-525)]
    tagREADYSTATE ReadyState { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (-525)]
    [TypeLibFunc ((short) 4)]
    get; }

    [DispId (550)]
    bool Offline { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (550)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (550)]
    set; }

    [DispId (0x227)]
    bool Silent { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x227)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x227)]
    set; }

    [DispId (0x228)]
    bool RegisterAsBrowser { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x228)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x228)]
    set; }

    [DispId (0x229)]
    bool RegisterAsDropTarget { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x229)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x229)]
    set; }

    [DispId (0x22a)]
    bool TheaterMode { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22a)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22a)]
    set; }

    [DispId (0x22b)]
    bool AddressBar { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22b)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22b)]
    set; }

    [DispId (0x22c)]
    bool Resizable { [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22c)]
    get; [param: In]
    [MethodImpl (MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [DispId (0x22c)]
    set; }
  } */
}