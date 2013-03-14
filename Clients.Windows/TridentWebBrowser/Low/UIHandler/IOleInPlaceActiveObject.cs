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

namespace DesktopGap.Clients.Windows.TridentWebBrowser.Low.UIHandler
{
  [ComVisible (true)]
  [ComImport]
  [Guid ("00000117-0000-0000-C000-000000000046")]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleInPlaceActiveObject
  {
    //IOleWindow
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int GetWindow ([In] [Out] ref IntPtr phwnd);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ContextSensitiveHelp (
        [In] [MarshalAs (UnmanagedType.Bool)] bool
            fEnterMode);

    //IOleInPlaceActiveObject
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int TranslateAccelerator (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagMSG lpmsg);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnFrameWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int OnDocWindowActivate (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fActivate);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int ResizeBorder (
        [In] [MarshalAs (UnmanagedType.Struct)] ref tagRECT prcBorder,
        [In] [MarshalAs (UnmanagedType.Interface)] ref IOleInPlaceUIWindow pUIWindow,
        [In] [MarshalAs (UnmanagedType.Bool)] bool fFrameWindow);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int EnableModeless (
        [In] [MarshalAs (UnmanagedType.Bool)] bool fEnable);
  }
}