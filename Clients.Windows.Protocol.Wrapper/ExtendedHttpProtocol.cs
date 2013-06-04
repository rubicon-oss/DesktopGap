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
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper
{
  [ComVisible (true)]
  [Guid ("E8253D6B-1AEE-4B5A-B7F9-F37D9C76C5FB")]
  public class ExtendedHttpProtocol : IInternetProtocol, IInternetProtocolRoot
  {
    private IInternetProtocol _wrapped;

    private readonly Control _dispatcher;
    private readonly IUrlFilter _urlFilter;
    private static readonly object lockObject = new object();


    public ExtendedHttpProtocol (Control dispatcher, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("dispatcher", dispatcher);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);

      _urlFilter = urlFilter;
    }

    protected MemoryStream Stream = new MemoryStream (0x8000);
    protected byte[] StreamBuffer = new byte[0x8000];

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      // How to do more complex stuff: http://www.codeproject.com/Articles/6120/A-Simple-protocol-to-view-aspx-pages-without-IIS-i
      Uri uri;


      if (!(Uri.TryCreate (szURL, UriKind.RelativeOrAbsolute, out uri) && _urlFilter.IsAllowed (uri)))
      {
        Debug.WriteLine ("is not allowed", szURL);
        Sink.ReportResult (0, 403, "Forbidden");
        return;
      }

      BINDINFO bindinfo = GetBindInfo (pOIBindInfo);

      var wc = WebRequest.Create (uri);
      wc.Headers = new WebHeaderCollection();
      wc.Method = GetMethod (bindinfo);
      wc.
      Negotiate.BeginningTransaction(szURL, string.Empty, 0, out strRequestHeaders);


      wc.DownloadData (uri)
    }

    public string GetMethod (BINDINFO bindInfo)
    {
      switch (bindInfo.dwBindVerb)
      {
        case BINDVERB.BINDVERB_GET:
          return "GET";
        case BINDVERB.BINDVERB_POST:
          return "POST";
        case BINDVERB.BINDVERB_PUT:
          return "PUT";
          //case BINDVERB.BINDVERB_CUSTOM: // ???
          //  break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    public byte[] GetPostData(BINDINFO BindInfo)
    {
      if (BindInfo.dwBindVerb != BINDVERB.BINDVERB_POST)
        return new byte[0];
      byte[] result = new byte[0];
      if (BindInfo.stgmedData.enumType == TYMED.TYMED_HGLOBAL)
      {
        UInt32 length = BindInfo.cbStgmedData;
        result = new byte[length];

        Marshal.Copy(BindInfo.stgmedData.u, result, 0, (int)length);
        if (BindInfo.stgmedData.pUnkForRelease == null)
          Marshal.FreeHGlobal(BindInfo.stgmedData.u);
      }
      return result;
    }
    public BINDINFO GetBindInfo (IInternetBindInfo pOIBindInfo)
    {
      BINDINFO BindInfo = new BINDINFO();
      BindInfo.cbSize = (UInt32) Marshal.SizeOf (typeof (BINDINFO));
      UInt32 AsyncFlag;
      pOIBindInfo.GetBindInfo (out AsyncFlag, ref BindInfo);
      return BindInfo;
    }

    public void Continue (ref _tagPROTOCOLDATA pProtocolData)
    {
      var _pProtocolData = pProtocolData;
      _dispatcher.Dispatcher.Invoke (() => _wrapped.Continue (ref _pProtocolData));
      pProtocolData = _pProtocolData;
    }

    public void Abort (int hrReason, uint dwOptions)
    {
      Debug.WriteLine ("Abort");
    }

    public void Terminate (uint dwOptions)
    {
      Debug.WriteLine ("Terminate");
    }

    public void Suspend ()
    {
      Debug.WriteLine ("Suspend");
    }

    public void Resume ()
    {
      Debug.WriteLine ("Resume");
    }

    public uint Read (IntPtr pv, uint cb, out uint pcbRead)
    {
      lock (lockObject)
      {
        pcbRead = (uint) Math.Min (cb, StreamBuffer.Length);
        pcbRead = (uint) Stream.Read (StreamBuffer, 0, (int) pcbRead);
        Marshal.Copy (StreamBuffer, 0, pv, (int) pcbRead);

        var response = (pcbRead == 0) ? HResult.S_FALSE : (UInt32) HResult.S_OK;
        return response;
      }
    }

    public void Seek (_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
    {
      _ULARGE_INTEGER _plibNewPosition = default(_ULARGE_INTEGER);
      Debug.WriteLine ("Seek");
    }

    public void LockRequest (uint dwOptions)
    {
      Debug.WriteLine ("LockRequest");
    }

    public void UnlockRequest ()
    {
      Debug.WriteLine ("UnlockRequest");
    }
  }
}