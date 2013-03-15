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
using System.Runtime.InteropServices.ComTypes;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler;

namespace DesktopGap.Clients.Windows.OleLibraryDependencies
{
  public enum NativeDragDropEffects : uint
  {
    NONE = 0,
    COPY = 1,
    MOVE = 2,
    LINK = 4,
    SCROLL = 0x80000000
  }

  [ComVisible (true)]
  [ComImport]
  [Guid ("00000122-0000-0000-C000-000000000046")]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  public interface IDropTarget
  {
    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int DragEnter (
        [In] [MarshalAs (UnmanagedType.Interface)] IDataObject pDataObj,
        [In] [MarshalAs (UnmanagedType.U4)] uint grfKeyState,
        [In] [MarshalAs (UnmanagedType.Struct)] tagPOINT pt,
        [In] [Out] [MarshalAs (UnmanagedType.U4)] ref uint pdwEffect);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int DragOver (
        [In] [MarshalAs (UnmanagedType.U4)] uint grfKeyState,
        [In] [MarshalAs (UnmanagedType.Struct)] tagPOINT pt,
        [In] [Out] [MarshalAs (UnmanagedType.U4)] ref uint pdwEffect);

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int DragLeave ();

    [return: MarshalAs (UnmanagedType.I4)]
    [PreserveSig]
    int Drop (
        [In] [MarshalAs (UnmanagedType.Interface)] IDataObject pDataObj,
        [In] [MarshalAs (UnmanagedType.U4)] uint grfKeyState,
        [In] [MarshalAs (UnmanagedType.Struct)] tagPOINT pt,
        [In] [Out] [MarshalAs (UnmanagedType.U4)] ref uint pdwEffect);
  }
}