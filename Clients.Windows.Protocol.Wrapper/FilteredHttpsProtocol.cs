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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper
{
  [ComVisible (true)]
  [Guid ("A997B3FD-FDEE-4E51-9F56-95EC2CBFCB5F")]
  public class FilteredHttpsProtocol : IInternetProtocol, IInternetProtocolRoot
  {
    private IInternetProtocol _wrapped;

    private readonly Control _dispatcher;
    private readonly IUrlFilter _urlFilter;

    public FilteredHttpsProtocol (Control dispatcher, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("dispatcher", dispatcher);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);


      _dispatcher = dispatcher;
      _urlFilter = urlFilter;

      _dispatcher.Dispatcher.Invoke (
          () =>
          {
            var originalHttpHandler = new HttpsProtocol();
            _wrapped = (IInternetProtocol) originalHttpHandler;
          });
    }

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      Debug.WriteLine ("HTTPS filter applied to " + szURL);
      var isAllowed = _urlFilter.IsAllowed (szURL);
      if (!isAllowed)
      {
        Sink.ReportResult (-1, 403, "Forbidden");
        return;
      }

      _dispatcher.Dispatcher.Invoke (() => _wrapped.Start (szURL, Sink, pOIBindInfo, grfPI, dwReserved));
    }

    public void Continue (ref _tagPROTOCOLDATA pProtocolData)
    {
      var _pProtocolData = pProtocolData;
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Continue (ref _pProtocolData));
      pProtocolData = _pProtocolData;
    }

    public void Abort (int hrReason, uint dwOptions)
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Abort (hrReason, dwOptions));
    }

    public void Terminate (uint dwOptions)
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Terminate (dwOptions));
    }

    public void Suspend ()
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Suspend());
    }


    public void Resume ()
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Resume());
    }

    public uint Read (IntPtr pv, uint cb, out uint pcbRead)
    {
      uint result = 0;
      uint _pcbRead = 0;

      _dispatcher.Dispatcher.Invoke (() => result = _wrapped.Read (pv, cb, out _pcbRead));
      pcbRead = _pcbRead;

      return result;
    }

    public void Seek (_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
    {
      _ULARGE_INTEGER _plibNewPosition = default(_ULARGE_INTEGER);
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Seek (dlibMove, dwOrigin, out _plibNewPosition));
      plibNewPosition = _plibNewPosition;
    }

    public void LockRequest (uint dwOptions)
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.LockRequest (dwOptions));
    }

    public void UnlockRequest ()
    {
      _dispatcher.Dispatcher.Invoke (() => _wrapped.UnlockRequest());
    }
  }
}