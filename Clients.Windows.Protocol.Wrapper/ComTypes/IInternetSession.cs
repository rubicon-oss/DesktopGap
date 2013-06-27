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

namespace DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes
{
  [ComVisible (true)]
  [Guid ("79eac9e7-baf9-11ce-8c82-00aa004ba90b")]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  public interface IInternetSession
  {
    [PreserveSig]
    int RegisterNameSpace (
        [In] IClassFactory classFactory,
        [In] ref Guid rclsid,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string pwzProtocol,
        [In] int cPatterns,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string ppwzPatterns,
        [In] int dwReserved);

    [PreserveSig]
    int UnregisterNameSpace (
        [In] IClassFactory classFactory,
        [In] [MarshalAs (UnmanagedType.LPWStr)] string pszProtocol);

    [PreserveSig]
    int RegisterMimeFilter (
        [MarshalAs (UnmanagedType.Interface)] IClassFactory pCF, ref Guid rclsid, [MarshalAs (UnmanagedType.LPWStr)] string pwzType);

    [PreserveSig]
    void UnregisterMimeFilter (IClassFactory pCF, [MarshalAs (UnmanagedType.LPWStr)] string pwzType);

    int Bogus3 ();

    int Bogus4 ();

    int Bogus5 ();
  }
}