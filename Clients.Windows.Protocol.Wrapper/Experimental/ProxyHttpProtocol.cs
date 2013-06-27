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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper.Experimental
{
  [ComVisible (true)]
  [Guid ("E8253D6B-1AEE-4B5A-B7F9-F37D9C76C5FB")]
  public class ProxyHttpProtocol : IInternetProtocol, IInternetProtocolRoot, IInternetProtocolInfo
  {
    private IInternetProtocol _wrapped;

    private readonly Control _dispatcher;
    private readonly IUrlFilter _urlFilter;

    public ProxyHttpProtocol (Control dispatcher, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("dispatcher", dispatcher);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);


      _dispatcher = dispatcher;
      _urlFilter = urlFilter;

      _dispatcher.Dispatcher.Invoke (
          () =>
          {
            var originalHttpHandler = new HttpProtocol();
            _wrapped = (IInternetProtocol) originalHttpHandler;
          });
    }

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      // How to do more complex stuff: http://www.codeproject.com/Articles/6120/A-Simple-protocol-to-view-aspx-pages-without-IIS-i
      var isAllowed = _urlFilter.IsAllowed (szURL);
      if (!isAllowed)
      {
        Debug.WriteLine ("is not allowed", szURL);
        Sink.ReportResult (0, 403, "Forbidden");
        LockRequest (0);
        Terminate (0);
        UnlockRequest();
      }

     //Sink.ReportProgress(tagBINDSTATUS.BINDSTATUS_MIMETYPEAVAILABLE, "text/html");
     // Sink.ReportResult (HResult.INET_E_REDIRECT_FAILED, 0,   );
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

    public uint ParseUrl (
        string pwzUrl, PARSEACTION ParseAction, uint dwParseFlags, IntPtr pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
    {
      var temp = new StringBuilder (pwzUrl);

      temp.Insert (0, "http://localhost:1234/");

      Marshal.Copy(temp.ToString().ToCharArray(), 0, pwzResult, temp.Length);
      Marshal.WriteInt32(pwzResult, temp.Length * 2, 0);
      pcchResult = (UInt32)temp.Length+1;
      return HResult.S_OK;
    }

    public uint CombineUrl (
        string pwzBaseUrl, string pwzRelativeUrl, uint dwCombineFlags, IntPtr pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
    {
      pcchResult = HResult.INET_E_DEFAULT_ACTION;
      return HResult.INET_E_DEFAULT_ACTION;
    }

    public uint CompareUrl (string pwzUrl1, string pwzUrl2, uint dwCompareFlags)
    {
      return HResult.INET_E_DEFAULT_ACTION;

    }

    public uint QueryInfo (string pwzUrl, QUERYOPTION OueryOption, uint dwQueryFlags, IntPtr pBuffer, uint cbBuffer, ref uint pcbBuf, uint dwReserved)
    {
      return HResult.INET_E_DEFAULT_ACTION;
    }
  }
}