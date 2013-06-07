﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
  public struct Guids
  {
    public static Guid IID_IHttpNegotiate = new Guid ("79eac9d2-baf9-11ce-8c82-00aa004ba90b");
  }

  public struct _LARGE_INTEGER
  {
    public Int64 QuadPart;
  }

  public struct _ULARGE_INTEGER
  {
    public UInt64 QuadPart;
  }

  public struct _tagPROTOCOLDATA
  {
    public uint grfFlags;
    public uint dwState;
    public IntPtr pData;
    public uint cbData;
  }

  public struct BINDINFO
  {
    public UInt32 cbSize;

    [MarshalAs (UnmanagedType.LPWStr)]
    public string szExtraInfo;

    public STGMEDIUM stgmedData;
    public UInt32 grfBindInfoF;

    [MarshalAs (UnmanagedType.U4)]
    public BINDVERB dwBindVerb;

    [MarshalAs (UnmanagedType.LPWStr)]
    public string szCustomVerb;

    public UInt32 cbStgmedData;
    public UInt32 dwOptions;
    public UInt32 dwOptionsFlags;
    public UInt32 dwCodePage;
    public SECURITY_ATTRIBUTES securityAttributes;
    public Guid iid;

    [MarshalAs (UnmanagedType.IUnknown)]
    public object pUnk;

    public UInt32 dwReserved;
  }

  public struct STGMEDIUM
  {
    [MarshalAs (UnmanagedType.U4)]
    public TYMED enumType;

    public IntPtr u;

    [MarshalAs (UnmanagedType.IUnknown)]
    public object pUnkForRelease;
  }

  public struct SECURITY_ATTRIBUTES
  {
    public UInt32 nLength;
    public IntPtr lpSecurityDescriptor;
    public bool bInheritHandle;
  }

  public enum TYMED : uint //Should be UInt32 but you can only use C# keywords here.
  {
    TYMED_HGLOBAL = 1,
    TYMED_FILE = 2,
    TYMED_ISTREAM = 4,
    TYMED_ISTORAGE = 8,
    TYMED_GDI = 16,
    TYMED_MFPICT = 32,
    TYMED_ENHMF = 64,
    TYMED_NULL = 0
  }

  public enum BINDVERB : uint
  {
    BINDVERB_GET = 0,
    BINDVERB_POST = 1,
    BINDVERB_PUT = 2,
    BINDVERB_CUSTOM = 3,
  }


  public enum PARSEACTION
  {
    PARSE_CANONICALIZE = 1,
    PARSE_FRIENDLY = PARSE_CANONICALIZE + 1,
    PARSE_SECURITY_URL = PARSE_FRIENDLY + 1,
    PARSE_ROOTDOCUMENT = PARSE_SECURITY_URL + 1,
    PARSE_DOCUMENT = PARSE_ROOTDOCUMENT + 1,
    PARSE_ANCHOR = PARSE_DOCUMENT + 1,
    PARSE_ENCODE = PARSE_ANCHOR + 1,
    PARSE_DECODE = PARSE_ENCODE + 1,
    PARSE_PATH_FROM_URL = PARSE_DECODE + 1,
    PARSE_URL_FROM_PATH = PARSE_PATH_FROM_URL + 1,
    PARSE_MIME = PARSE_URL_FROM_PATH + 1,
    PARSE_SERVER = PARSE_MIME + 1,
    PARSE_SCHEMA = PARSE_SERVER + 1,
    PARSE_SITE = PARSE_SCHEMA + 1,
    PARSE_DOMAIN = PARSE_SITE + 1,
    PARSE_LOCATION = PARSE_DOMAIN + 1,
    PARSE_SECURITY_DOMAIN = PARSE_LOCATION + 1,
    PARSE_ESCAPE = PARSE_SECURITY_DOMAIN + 1,
    PARSE_UNESCAPE = PARSE_ESCAPE + 1,
  }

  public enum QUERYOPTION
  {
    QUERY_EXPIRATION_DATE = 1,
    QUERY_TIME_OF_LAST_CHANGE = QUERY_EXPIRATION_DATE + 1,
    QUERY_CONTENT_ENCODING = QUERY_TIME_OF_LAST_CHANGE + 1,
    QUERY_CONTENT_TYPE = QUERY_CONTENT_ENCODING + 1,
    QUERY_REFRESH = QUERY_CONTENT_TYPE + 1,
    QUERY_RECOMBINE = QUERY_REFRESH + 1,
    QUERY_CAN_NAVIGATE = QUERY_RECOMBINE + 1,
    QUERY_USES_NETWORK = QUERY_CAN_NAVIGATE + 1,
    QUERY_IS_CACHED = QUERY_USES_NETWORK + 1,
    QUERY_IS_INSTALLEDENTRY = QUERY_IS_CACHED + 1,
    QUERY_IS_CACHED_OR_MAPPED = QUERY_IS_INSTALLEDENTRY + 1,
    QUERY_USES_CACHE = QUERY_IS_CACHED_OR_MAPPED + 1,
    QUERY_IS_SECURE = QUERY_USES_CACHE + 1,
    QUERY_IS_SAFE = QUERY_IS_SECURE + 1,
  }

  public enum BSCF : uint
  {
    BSCF_FIRSTDATANOTIFICATION = 0,
    BSCF_INTERMEDIATEDATANOTIFICATION = 1,
    BSCF_LASTDATANOTIFICATION = 2,
    BSCF_DATAFULLYAVAILABLE = 3,
    BSCF_AVAILABLEDATASIZEUNKNOWN = 4,
  }


  public enum tagBINDSTATUS: uint
  {
    BINDSTATUS_FINDINGRESOURCE = 1,
    BINDSTATUS_CONNECTING,
    BINDSTATUS_REDIRECTING,
    BINDSTATUS_BEGINDOWNLOADDATA,
    BINDSTATUS_DOWNLOADINGDATA,
    BINDSTATUS_ENDDOWNLOADDATA,
    BINDSTATUS_BEGINDOWNLOADCOMPONENTS,
    BINDSTATUS_INSTALLINGCOMPONENTS,
    BINDSTATUS_ENDDOWNLOADCOMPONENTS,
    BINDSTATUS_USINGCACHEDCOPY,
    BINDSTATUS_SENDINGREQUEST,
    BINDSTATUS_CLASSIDAVAILABLE,
    BINDSTATUS_MIMETYPEAVAILABLE,
    BINDSTATUS_CACHEFILENAMEAVAILABLE,
    BINDSTATUS_BEGINSYNCOPERATION,
    BINDSTATUS_ENDSYNCOPERATION,
    BINDSTATUS_BEGINUPLOADDATA,
    BINDSTATUS_UPLOADINGDATA,
    BINDSTATUS_ENDUPLOADINGDATA,
    BINDSTATUS_PROTOCOLCLASSID,
    BINDSTATUS_ENCODING,
    BINDSTATUS_VERIFIEDMIMETYPEAVAILABLE,
    BINDSTATUS_CLASSINSTALLLOCATION,
    BINDSTATUS_DECODING,
    BINDSTATUS_LOADINGMIMEHANDLER,
    BINDSTATUS_CONTENTDISPOSITIONATTACH,
    BINDSTATUS_FILTERREPORTMIMETYPE,
    BINDSTATUS_CLSIDCANINSTANTIATE,
    BINDSTATUS_IUNKNOWNAVAILABLE,
    BINDSTATUS_DIRECTBIND,
    BINDSTATUS_RAWMIMETYPE,
    BINDSTATUS_PROXYDETECTING,
    BINDSTATUS_ACCEPTRANGES,
    BINDSTATUS_COOKIE_SENT,
    BINDSTATUS_COMPACT_POLICY_RECEIVED,
    BINDSTATUS_COOKIE_SUPPRESSED,
    BINDSTATUS_COOKIE_STATE_UNKNOWN,
    BINDSTATUS_COOKIE_STATE_ACCEPT,
    BINDSTATUS_COOKIE_STATE_REJECT,
    BINDSTATUS_COOKIE_STATE_PROMPT,
    BINDSTATUS_COOKIE_STATE_LEASH,
    BINDSTATUS_COOKIE_STATE_DOWNGRADE,
    BINDSTATUS_POLICY_HREF,
    BINDSTATUS_P3P_HEADER,
    BINDSTATUS_SESSION_COOKIE_RECEIVED,
    BINDSTATUS_PERSISTENT_COOKIE_RECEIVED,
    BINDSTATUS_SESSION_COOKIES_ALLOWED,
    BINDSTATUS_CACHECONTROL,
    BINDSTATUS_CONTENTDISPOSITIONFILENAME,
    BINDSTATUS_MIMETEXTPLAINMISMATCH,
    BINDSTATUS_PUBLISHERAVAILABLE,
    BINDSTATUS_DISPLAYNAMEAVAILABLE,
    BINDSTATUS_SSLUX_NAVBLOCKED,
    BINDSTATUS_SERVER_MIMETYPEAVAILABLE,
    BINDSTATUS_SNIFFED_CLASSIDAVAILABLE,
    BINDSTATUS_64BIT_PROGRESS,
    BINDSTATUS_LAST = BINDSTATUS_64BIT_PROGRESS,
    BINDSTATUS_RESERVED_0,
    BINDSTATUS_RESERVED_1,
    BINDSTATUS_RESERVED_2,
    BINDSTATUS_RESERVED_3,
    BINDSTATUS_RESERVED_4,
    BINDSTATUS_RESERVED_5,
    BINDSTATUS_RESERVED_6,
    BINDSTATUS_RESERVED_7,
    BINDSTATUS_RESERVED_8,
    BINDSTATUS_RESERVED_9,
    BINDSTATUS_LAST_PRIVATE = BINDSTATUS_RESERVED_9
  }
}