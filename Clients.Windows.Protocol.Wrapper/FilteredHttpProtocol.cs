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
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper
{
  [ComVisible (true)]
  [Guid ("E8253D6B-1AEE-4B5A-B7F9-F37D9C76C5FB")]
  public class FilteredHttpProtocol : IInternetProtocol, IInternetProtocolRoot, IDisposable
  {
    private IInternetProtocol _wrapped;

    private readonly Dispatcher _dispatcher;
    private readonly IUrlFilter _urlFilter;
    private string _currentUrl;
    private bool _isAllowed;

    public FilteredHttpProtocol (Dispatcher dispatcher, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("dispatcher", dispatcher);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);

      _urlFilter = urlFilter;
      _dispatcher = dispatcher;
      _dispatcher.Invoke (
          () =>
          {
            var originalHttpHandler = new HttpProtocol();
            _wrapped = (IInternetProtocol) originalHttpHandler;
          });
    }

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      // How to do more complex stuff: http://www.codeproject.com/Articles/6120/A-Simple-protocol-to-view-aspx-pages-without-IIS-i
      _currentUrl = szURL;
      _isAllowed = _urlFilter.IsAllowed (szURL);
      if (!_isAllowed)
      {
        _dispatcher.Invoke (
            () => Sink.ReportResult (HResult.INET_E_RESOURCE_NOT_FOUND, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString()));
        return;
      }
      _dispatcher.Invoke (
          () => _wrapped.Start (szURL, Sink, pOIBindInfo, grfPI, dwReserved),
          new TimeSpan (0, 0, 0, 30));
    }

    public void Continue (ref _tagPROTOCOLDATA pProtocolData)
    {
      var _pProtocolData = pProtocolData;
      _dispatcher.Invoke (() => _wrapped.Continue (ref _pProtocolData));
      pProtocolData = _pProtocolData;
    }

    public void Abort (int hrReason, uint dwOptions)
    {
      _dispatcher.Invoke (() => _wrapped.Abort (hrReason, dwOptions));
    }

    public void Terminate (uint dwOptions)
    {
      _dispatcher.Invoke (() => _wrapped.Terminate (dwOptions));
    }

    public void Suspend ()
    {
      _dispatcher.Invoke (() => _wrapped.Suspend());
    }

    public void Resume ()
    {
      _dispatcher.Invoke (() => _wrapped.Resume());
    }

    public uint Read (IntPtr pv, uint cb, out uint pcbRead)
    {
      uint result = 0;
      uint _pcbRead = 0;
      if (_isAllowed)
        _dispatcher.Invoke (() => result = _wrapped.Read (pv, cb, out _pcbRead));

      pcbRead = _pcbRead;

      return result;
    }

    public void Seek (_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
    {
      _ULARGE_INTEGER _plibNewPosition = default(_ULARGE_INTEGER);
      _dispatcher.Invoke (() => _wrapped.Seek (dlibMove, dwOrigin, out _plibNewPosition));
      plibNewPosition = _plibNewPosition;
    }

    public void LockRequest (uint dwOptions)
    {
      _dispatcher.Invoke (() => _wrapped.LockRequest (dwOptions));
    }

    public void UnlockRequest ()
    {
      _dispatcher.Invoke (() => _wrapped.UnlockRequest());
    }

    public void Dispose ()
    {
    }
  }
}