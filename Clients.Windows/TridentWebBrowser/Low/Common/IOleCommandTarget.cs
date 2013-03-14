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

namespace DesktopGap.Clients.Windows.TridentWebBrowser.Low.Common
{
   [StructLayout(LayoutKind.Sequential)]
    public struct OLECMD
    {
        //public UInt32 cmdID;
        //public UInt64 cmdf;
        public uint cmdID;
        public uint cmdf;    // NB: See above note (*)    
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OLECMDTEXT
    {
        public UInt32 cmdtextf;
        public UInt32 cwActual;
        public UInt32 cwBuf;
        public char rgwz;
    }

  [ComImport]
  [Guid ("b722bccb-4e68-101b-a2bc-00aa00404770")]
  [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleCommandTarget

  {
    //IMPORTANT: The order of the methods is critical here. You
    //perform early binding in most cases, so the order of the methods
    //here MUST match the order of their vtable layout (which is determined
    //by their layout in IDL). The interop calls key off the vtable 
    //ordering, not the symbolic names. Therefore, if you switched these 
    //method declarations and tried to call the Exec method on an 
    //IOleCommandTarget interface from your application, it would 
    //translate into a call to the QueryStatus method instead.
    void QueryStatus (
        ref Guid pguidCmdGroup,
        UInt32 cCmds,
        [MarshalAs (UnmanagedType.LPArray, SizeParamIndex = 1)] OLECMD[] prgCmds,
        ref OLECMDTEXT CmdText);

    void Exec (
        ref Guid pguidCmdGroup,
        uint nCmdId,
        uint nCmdExecOpt,
        ref object pvaIn,
        ref object pvaOut);
  }
}