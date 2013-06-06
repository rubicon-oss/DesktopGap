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
using System.Net.Http;
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
    private const string c_httpHeaderSplitCharacters = "\r\n";
        private const int c_requiredMaxWorkerThreads = 2;

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
      int workers, ioThreads;
      ThreadPool.GetMaxThreads(out workers, out ioThreads);
      
      if(workers < c_requiredMaxWorkerThreads)
        ThreadPool.SetMaxThreads (c_requiredMaxWorkerThreads, ioThreads);
    }

    protected Stream ResponseStream;
    protected byte[] Buffer = new byte[0x8000];
    private HttpWebRequest _webRequest;
    private HttpWebResponse _httpResponse;
    private string _currentUrl;
    private uint _size;

    public void Start (string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
    {
      if (_size != 0)
        throw new InvalidOperationException ("nope");

      #region HTTPCLIENT 

      //_currentUrl = szURL;
      //Uri uri;
      //if (!(Uri.TryCreate (szURL, UriKind.RelativeOrAbsolute, out uri) && _urlFilter.IsAllowed (uri)))
      //{
      //  Sink.ReportResult (0, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
      //  return;
      //}
      //var httpClient = new HttpClient();
      //var httpRequestMessage = new HttpRequestMessage { RequestUri = uri };
      //var bindInfo = GetBindInfo (pOIBindInfo);
      //var negotiate = GetHttpNegotiate (Sink);


      ////"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
      //var postData = GetPostData (bindInfo);

      //string strRequestHeaders;
      //negotiate.BeginningTransaction (szURL, string.Empty, 0, out strRequestHeaders);
      //foreach (
      //    var header in
      //        Regex.Split (strRequestHeaders, c_httpHeaderSplitCharacters).Where (
      //            s => !string.IsNullOrEmpty (s)
      //                 && !s_restrictedHeaders.Contains (s.Split (':').First())))
      //{
      //  var splitHeader = header.Split (':');
      //  var key = splitHeader[0];
      //  var value = splitHeader[1];
      //  httpRequestMessage.Headers.Add (key, value);
      //}


      //switch (bindInfo.dwBindVerb)
      //{
      //  case BINDVERB.BINDVERB_GET:
      //    httpRequestMessage.Method = HttpMethod.Get;
      //    break;

      //  case BINDVERB.BINDVERB_POST:
      //    httpRequestMessage.Method = HttpMethod.Post;
      //    httpRequestMessage.Content = new ByteArrayContent (postData);
      //    break;

      //  case BINDVERB.BINDVERB_PUT:
      //    httpRequestMessage.Method = HttpMethod.Put;
      //    break;
      //    //case BINDVERB.BINDVERB_CUSTOM: // ???
      //    //  break;
      //  default:
      //    throw new ArgumentOutOfRangeException();
      //}


      //var response = httpClient.SendAsync (httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
      ////response.ContinueWith (
      ////    task =>
      ////    {

      //var task = response;
      //task.Wait();
      //      var status = (uint)HttpStatusCode.NotFound;
      //      var message = HttpStatusCode.NotFound.ToString();

      //      if (!task.IsFaulted)
      //      {
      //        var result = task.Result;
      //        string strNewResponseHeaders;
      //        negotiate.OnResponse (0, result.Headers.ToString(), result.Headers.ToString(), out strNewResponseHeaders);
      //        if (result.IsSuccessStatusCode)
      //        {
      //          result.Content.ReadAsStreamAsync().ContinueWith(t => ResponseStream = t.Result);
      //          Sink.ReportData (BSCF.BSCF_LASTDATANOTIFICATION, (uint)result.Content.Headers.ContentLength, (uint)result.Content.Headers.ContentLength);
      //        }
      //        status = (uint) result.StatusCode;
      //        message = result.ReasonPhrase;
      //      }

      //      Sink.ReportResult (0, status, message);
      //    //});

      //if (!_urlFilter.IsAllowed (_httpResponse.ResponseUri))
      //  throw new Exception ("not allowed!"); // TODO - do something useful here

      #endregion

      #region WebRequest

      Debug.WriteLine ("starting url: " + szURL);

      _currentUrl = szURL;
      // How to do more complex stuff: http://www.codeproject.com/Articles/6120/A-Simple-protocol-to-view-aspx-pages-without-IIS-i
      Uri uri;

      if (!(Uri.TryCreate (szURL, UriKind.RelativeOrAbsolute, out uri) && _urlFilter.IsAllowed (uri)))
      {
        Sink.ReportResult (0, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
        return;
      }
      var bindInfo = GetBindInfo (pOIBindInfo);
      var postData = GetPostData (bindInfo);

      string method;

      //_webRequest = (HttpWebRequest) WebRequest.Create (uri);
      switch (bindInfo.dwBindVerb)
      {
        case BINDVERB.BINDVERB_GET:
          method = HttpMethod.Get.Method;
          break;

        case BINDVERB.BINDVERB_POST:
          method = HttpMethod.Post.Method;
          break;

        case BINDVERB.BINDVERB_PUT:
          method = HttpMethod.Put.Method;
          break;
          //case BINDVERB.BINDVERB_CUSTOM: // ???
          //  break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      //_webRequest.KeepAlive = false;
      ThreadPool.QueueUserWorkItem (
          obj =>
          {
            if (postData.Length > 0)
            {
              using (var requestStream = _webRequest.GetRequestStream())
              {
                requestStream.Write (postData, 0, postData.Length);
              }
            }
            _webRequest = (HttpWebRequest) WebRequest.Create (uri);
            _webRequest.Method = method;
            _webRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            var negotiate = GetHttpNegotiate (Sink);

            string strRequestHeaders;
            negotiate.BeginningTransaction (szURL, string.Empty, 0, out strRequestHeaders);
            foreach (
                var header in
                    Regex.Split (strRequestHeaders, c_httpHeaderSplitCharacters).Where (
                        s => !string.IsNullOrEmpty (s)
                             && !s_restrictedHeaders.Contains (s.Split (':').First())))
            {
              _webRequest.Headers.Add (header);
            }
            //_webRequest.Headers.Add ("Accept-Encoding: gzip, deflate");

            try
            {
              _httpResponse = (HttpWebResponse) _webRequest.GetResponse();

              //_httpResponse.
              //if (!_urlFilter.IsAllowed (_httpResponse.ResponseUri))
              //  throw new Exception ("not allowed!"); // TODO - do something useful here

              string strNewResponseHeaders;
              negotiate.OnResponse (0, _httpResponse.Headers.ToString(), strRequestHeaders, out strNewResponseHeaders);
              ResponseStream = _httpResponse.GetResponseStream();
              Sink.ReportData (BSCF.BSCF_LASTDATANOTIFICATION, (uint) _httpResponse.ContentLength, (uint) _httpResponse.ContentLength);
              Sink.ReportResult (0, (uint) _httpResponse.StatusCode, _httpResponse.StatusDescription);
            }
            catch (Exception ex)
            {
              Debug.WriteLine ("exception while retrieving " + _currentUrl + " " + ex);
              Sink.ReportResult (0, (uint) HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
            }
          });

      #endregion
    }


    public void Continue (ref _tagPROTOCOLDATA pProtocolData)
    {
      Debug.WriteLine ("Continue");
    }

    public void Abort (int hrReason, uint dwOptions)
    {
      Debug.WriteLine ("Abort");
      _webRequest.Abort();
      Dispose();
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
      if (ResponseStream != null)
      {
        pcbRead = (uint) Math.Min (cb, Buffer.Length);
        pcbRead = (uint) ResponseStream.Read (Buffer, 0, (int) pcbRead);
        Marshal.Copy (Buffer, 0, pv, (int) pcbRead);
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
      if (BindInfo.dwBindVerb != BINDVERB.BINDVERB_POST) // TODO figure out PUT
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
      lock (this)
      {
        Debug.WriteLine ("Disposing instance " + _currentUrl);
        if (ResponseStream != null)
          ResponseStream.Dispose();
        if (_httpResponse != null)
          _httpResponse.Close();
        _httpResponse = null;
        _webRequest = null;
      }
    }
  }
}