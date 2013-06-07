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
using System.Windows.Controls;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper.Factories
{
  [ComVisible (true)]
  public class FilteredHttpProtocolFactory : IProtocolFactory, IDisposable
  {
    private readonly IUrlFilter _urlFilter;
    private readonly Control _ctrl;

    private int _lockCount;

    public FilteredHttpProtocolFactory (IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);

      _urlFilter = urlFilter;
      _ctrl = new Control();
    }

    
    public void Dispose ()
    {
      
    }

    public Type ProtocolType
    {
      get { return typeof (FilteredHttpProtocol); }
    }

    public string ProtocolName
    {
      get { return Uri.UriSchemeHttp; }
    }

    public void CreateInstance (object pUnkOuter, Guid riid, out object ppvObject)
    {
      ppvObject = new FilteredHttpProtocol (_ctrl, _urlFilter);
      //ppvObject = new ExtendedHttpProtocol (_ctrl, _urlFilter);
      //ppvObject = new ProxyHttpProtocol (_ctrl, _urlFilter);
    }

    public void LockServer (bool fLock)
    {
      if(fLock)
        _lockCount++;
      else
        _lockCount--;

      if(_lockCount == 0)
        Dispose();
    }
  }
}