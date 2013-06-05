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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes;
using DesktopGap.Security.Urls;
using DesktopGap.Utilities;
using IServiceProvider = DesktopGap.Clients.Windows.Protocol.Wrapper.ComTypes.IServiceProvider;

namespace DesktopGap.Clients.Windows.Protocol.Wrapper
{
  [ComVisible (true)]
  [Guid ("E8253D6B-1AEE-4B5A-B7F9-F37D9C76C5FB")]
  public class ExtendedHttpProtocol : IInternetProtocol, IInternetProtocolRoot, IDisposable
  {
    private readonly IUrlFilter _urlFilter;
    private readonly object s_lockObject = new object();
    private const string c_httpHeaderSplitCharacters = "\r\n";

    private static int counter = 0;

    private static readonly ISet<string> s_restrictedHeaders = new HashSet<string> (
        new[]
        {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent",
            "Proxy-Connection"
        });


    public ExtendedHttpProtocol (Control dispatcher, IUrlFilter urlFilter)
    {
      ArgumentUtility.CheckNotNull ("dispatcher", dispatcher);
      ArgumentUtility.CheckNotNull ("urlFilter", urlFilter);

      _urlFilter = urlFilter;
    }

    protected Stream Stream;
    protected byte[] StreamBuffer = new byte[0x8000];
    private HttpWebRequest _webRequest;
    private HttpWebResponse _httpResponse;
    private string _currentUrl;
    private uint _size;

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      if(_size != 0)
        throw new InvalidOperationException("nope");

      new Thread (
          () =>
          {

            Debug.WriteLine ("starting url: " + szURL);

            counter++;
            Debug.WriteLine ("current instances: " + counter);

            _currentUrl = szURL;
            // How to do more complex stuff: http://www.codeproject.com/Articles/6120/A-Simple-protocol-to-view-aspx-pages-without-IIS-i
            Uri uri;

            if (!(Uri.TryCreate (szURL, UriKind.RelativeOrAbsolute, out uri) && _urlFilter.IsAllowed (uri)))
            {
              Sink.ReportResult (0, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
              return;
            }

            BINDINFO bindinfo = GetBindInfo (pOIBindInfo);
            IHttpNegotiate Negotiate = GetHttpNegotiate (Sink);

            var data = GetPostData (bindinfo);

            _webRequest = (HttpWebRequest) WebRequest.Create (uri);
            _webRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            _webRequest.Headers = new WebHeaderCollection();
            _webRequest.Method = GetMethod (bindinfo);
            

            //_webRequest.KeepAlive = false;
            _webRequest.AllowAutoRedirect = true;

            string strRequestHeaders;
            _webRequest.ContentLength = data.Length;
            Negotiate.BeginningTransaction (szURL, string.Empty, 0, out strRequestHeaders);
            foreach (
                var header in
                    Regex.Split (strRequestHeaders, c_httpHeaderSplitCharacters).Where (
                        s => !string.IsNullOrEmpty (s)
                             && !s_restrictedHeaders.Contains (s.Split (':').First())))
            {
              _webRequest.Headers.Add (header);
            }
            _webRequest.Headers.Add ("Accept-Encoding: gzip, deflate");
            if (data.Length > 0)
            {
              using (var requestStream = _webRequest.GetRequestStream())
              {
                requestStream.Write (data, 0, data.Length);
              }
            }
            try
            {
              _httpResponse = (HttpWebResponse) _webRequest.GetResponse();
              //if (!_urlFilter.IsAllowed (_httpResponse.ResponseUri))
              //  throw new Exception ("not allowed!"); // TODO - do something useful here

              string strNewResponseHeaders;
              Negotiate.OnResponse (0, _httpResponse.Headers.ToString(), strRequestHeaders, out strNewResponseHeaders);
              Stream = _httpResponse.GetResponseStream();
              Sink.ReportData (BSCF.BSCF_LASTDATANOTIFICATION, (uint) _httpResponse.ContentLength, (uint) _httpResponse.ContentLength);
              Sink.ReportResult (0, (uint) _httpResponse.StatusCode, _httpResponse.StatusDescription);
            }
            catch (Exception ex)
            {
              Debug.WriteLine (ex);
              Sink.ReportResult (0, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
            }
          }).Start();
    }

    public void Continue (ref _tagPROTOCOLDATA pProtocolData)
    {
      Debug.WriteLine ("Continue");
    }

    public void Abort (int hrReason, uint dwOptions)
    {
      Debug.WriteLine ("Abort");
      _webRequest.Abort();
      if (_httpResponse != null)
        _httpResponse.Close();
    }

    public void Terminate (uint dwOptions)
    {
      Dispose();
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
      pcbRead = 0;
      if (Stream != null)
      {
        pcbRead = (uint) Math.Min (cb, StreamBuffer.Length);
        pcbRead = (uint) Stream.Read (StreamBuffer, 0, (int) pcbRead);
        Marshal.Copy (StreamBuffer, 0, pv, (int) pcbRead);
        _size += pcbRead;
      }
      var response = (pcbRead == 0) ? HResult.S_FALSE : (UInt32) HResult.S_OK;
      return response;
    }

    public void Seek (_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
    {
      _ULARGE_INTEGER _plibNewPosition = default(_ULARGE_INTEGER);
      Debug.WriteLine ("Seek");
      plibNewPosition = _plibNewPosition;
    }

    public void LockRequest (uint dwOptions)
    {
      Debug.WriteLine ("LockRequest");
    }

    public void UnlockRequest ()
    {
      Debug.WriteLine ("UnlockRequest");
    }

    private IHttpNegotiate GetHttpNegotiate (IInternetProtocolSink Sink)
    {
      if ((Sink is IServiceProvider) == false)
        throw new Exception ("Error ProtocolSink does not support IServiceProvider.");

      Debug.WriteLine ("ServiceProvider");

      IServiceProvider Provider = (IServiceProvider) Sink;
      object obj_Negotiate = new object();
      Provider.QueryService (ref Guids.IID_IHttpNegotiate, ref Guids.IID_IHttpNegotiate, out obj_Negotiate);
      return (IHttpNegotiate) obj_Negotiate;
    }

    private string GetMethod (BINDINFO bindInfo)
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

    private byte[] GetPostData (BINDINFO BindInfo)
    {
      if (BindInfo.dwBindVerb != BINDVERB.BINDVERB_POST)
        return new byte[0];
      byte[] result = new byte[0];
      if (BindInfo.stgmedData.enumType == TYMED.TYMED_HGLOBAL)
      {
        UInt32 length = BindInfo.cbStgmedData;
        result = new byte[length];

        Marshal.Copy (BindInfo.stgmedData.u, result, 0, (int) length);
        if (BindInfo.stgmedData.pUnkForRelease == null)
          Marshal.FreeHGlobal (BindInfo.stgmedData.u);
      }
      return result;
    }

    private BINDINFO GetBindInfo (IInternetBindInfo pOIBindInfo)
    {
      BINDINFO BindInfo = new BINDINFO();
      BindInfo.cbSize = (UInt32) Marshal.SizeOf (typeof (BINDINFO));
      UInt32 AsyncFlag;
      pOIBindInfo.GetBindInfo (out AsyncFlag, ref BindInfo);
      return BindInfo;
    }

    public void Dispose ()
    {
      lock (s_lockObject)
      {
        counter--;
        Debug.WriteLine ("Disposing instance, " + _currentUrl + ", " + counter + " instances left");
        if (Stream != null)
          Stream.Dispose();
        if (_httpResponse != null)
          _httpResponse.Close();
        _httpResponse = null;
        _webRequest = null;
      }
    }
  }
}