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
using System.Linq;
using System.Runtime.InteropServices;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Clients.Windows.Protocol.Wrapper.Factories;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper
{
  public class ProtocolWrapperManager : IDisposable
  {
    private static class NativeMethods
    {
      [DllImport ("urlmon.dll")]
      public static extern int CoInternetGetSession (UInt32 dwSessionMode, ref IInternetSession ppIInternetSession, UInt32 dwReserved);
    }

    public void Dispose ()
    {
      
    }
    

    public void RegisterProtocol (IProtocolFactory factory)
    {
      IInternetSession session = null;
      if (NativeMethods.CoInternetGetSession (0, ref session, 0) != 0)
        return;

      var id = GuidFromType (factory.ProtocolType);
      session.RegisterNameSpace (factory, ref id, factory.ProtocolName, 0, null, 0);
    }

    private Guid GuidFromType (Type t)
    {
      var guid = t.GetCustomAttributes (typeof (GuidAttribute), false).Select (_ => ((GuidAttribute) _).Value).First();
      return Guid.Parse (guid);
    }
  }
}