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

namespace DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes
{
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  [Guid ("79EAC9E4-BAF9-11CE-8C82-00AA004BA90B")]
  public interface IInternetProtocol
  {
    uint Start (
        [MarshalAs (UnmanagedType.LPWStr)] string szURL,
        IInternetProtocolSink Sink,
        IInternetBindInfo pOIBindInfo,
        UInt32 grfPI,
        UInt32 dwReserved);

    void Continue (ref _tagPROTOCOLDATA pProtocolData);
    void Abort (Int32 hrReason, UInt32 dwOptions);
    void Terminate (UInt32 dwOptions);
    void Suspend ();
    void Resume ();


    [PreserveSig]
    UInt32 Read (IntPtr pv, UInt32 cb, out UInt32 pcbRead);

    void Seek (_LARGE_INTEGER dlibMove, UInt32 dwOrigin, out _ULARGE_INTEGER plibNewPosition);
    void LockRequest (UInt32 dwOptions);
    void UnlockRequest ();
  }
}