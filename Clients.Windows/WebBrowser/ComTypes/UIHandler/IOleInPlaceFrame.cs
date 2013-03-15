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
using System.Runtime.InteropServices;

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler
{
  [ComVisible (true)]
  [ComImport]
  [Guid ("00000116-0000-0000-C000-000000000046")]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleInPlaceFrame
  {
    //IOleWindow
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetWindow ([In] [Out] ref IntPtr phwnd);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ContextSensitiveHelp ([In] [MarshalAs (UnmanagedType.Bool)] bool fEnterMode);

    //IOleInPlaceUIWindow
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetBorder (
        [Out] [MarshalAs (UnmanagedType.LPStruct)] tagRECT lprectBorder);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int RequestBorderSpace ([In] [MarshalAs (UnmanagedType.Struct)] ref tagRECT pborderwidths);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int SetBorderSpace ([In] [MarshalAs (UnmanagedType.Struct)] ref tagRECT pborderwidths);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int SetActiveObject (
        [In] [MarshalAs (UnmanagedType.Interface)] ref IOleInPlaceActiveObject pActiveObject,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string pszObjName);

    //IOleInPlaceFrame 
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int InsertMenus (
        [In] IntPtr hmenuShared,
        [In] [Out] [MarshalAs (UnmanagedType.Struct)] ref object lpMenuWidths);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int SetMenu (
        [In] IntPtr hmenuShared,
        [In] IntPtr holemenu,
        [In] IntPtr hwndActiveObject);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int RemoveMenus ([In] IntPtr hmenuShared);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int SetStatusText ([In] [MarshalAs (UnmanagedType.LPWStr)] string pszStatusText);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int EnableModeless ([In] [MarshalAs (UnmanagedType.Bool)] bool fEnable);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateAccelerator (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagMSG lpmsg,
        [In] [MarshalAs (UnmanagedType.U2)] short wID);
  }
}