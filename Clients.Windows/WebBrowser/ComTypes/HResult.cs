﻿// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes
{
  public sealed class HResult
  {
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOTIMPL = unchecked((int) 0x80004001);


    /*
    public const int NOERROR = 0;
            public const int S_OK = 0;
        public const int S_FALSE = 1;
    public const int E_PENDING = unchecked((int) 0x8000000A);
    public const int E_HANDLE = unchecked((int) 0x80070006);
    public const int E_NOTIMPL = unchecked((int) 0x80004001);
    public const int E_NOINTERFACE = unchecked((int) 0x80004002);
    //ArgumentNullException. NullReferenceException uses COR_E_NULLREFERENCE
    public const int E_POINTER = unchecked((int) 0x80004003);
    public const int E_ABORT = unchecked((int) 0x80004004);
    public const int E_FAIL = unchecked((int) 0x80004005);
    public const int E_OUTOFMEMORY = unchecked((int) 0x8007000E);
    public const int E_ACCESSDENIED = unchecked((int) 0x80070005);
    public const int E_UNEXPECTED = unchecked((int) 0x8000FFFF);
    public const int E_FLAGS = unchecked((int) 0x1000);
    public const int E_INVALIDARG = unchecked((int) 0x80070057);

    //Wininet
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_FILE_NOT_FOUND = 2;
    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_INSUFFICIENT_BUFFER = 122;
    public const int ERROR_NO_MORE_ITEMS = 259;

    //Ole Errors
    public const int OLE_E_FIRST = unchecked((int) 0x80040000);
    public const int OLE_E_LAST = unchecked((int) 0x800400FF);
    public const int OLE_S_FIRST = unchecked((int) 0x00040000);
    public const int OLE_S_LAST = unchecked((int) 0x000400FF);
    //OLECMDERR_E_FIRST = 0x80040100
    public const int OLECMDERR_E_FIRST = unchecked((int) (OLE_E_LAST + 1));
    public const int OLECMDERR_E_NOTSUPPORTED = unchecked((int) (OLECMDERR_E_FIRST));
    public const int OLECMDERR_E_DISABLED = unchecked((int) (OLECMDERR_E_FIRST + 1));
    public const int OLECMDERR_E_NOHELP = unchecked((int) (OLECMDERR_E_FIRST + 2));
    public const int OLECMDERR_E_CANCELED = unchecked((int) (OLECMDERR_E_FIRST + 3));
    public const int OLECMDERR_E_UNKNOWNGROUP = unchecked((int) (OLECMDERR_E_FIRST + 4));

    public const int OLEOBJ_E_NOVERBS = unchecked((int) 0x80040180);
    public const int OLEOBJ_S_INVALIDVERB = unchecked((int) 0x00040180);
    public const int OLEOBJ_S_CANNOT_DOVERB_NOW = unchecked((int) 0x00040181);
    public const int OLEOBJ_S_INVALIDHWND = unchecked((int) 0x00040182);

    public const int DV_E_LINDEX = unchecked((int) 0x80040068);
    public const int OLE_E_OLEVERB = unchecked((int) 0x80040000);
    public const int OLE_E_ADVF = unchecked((int) 0x80040001);
    public const int OLE_E_ENUM_NOMORE = unchecked((int) 0x80040002);
    public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int) 0x80040003);
    public const int OLE_E_NOCONNECTION = unchecked((int) 0x80040004);
    public const int OLE_E_NOTRUNNING = unchecked((int) 0x80040005);
    public const int OLE_E_NOCACHE = unchecked((int) 0x80040006);
    public const int OLE_E_BLANK = unchecked((int) 0x80040007);
    public const int OLE_E_CLASSDIFF = unchecked((int) 0x80040008);
    public const int OLE_E_CANT_GETMONIKER = unchecked((int) 0x80040009);
    public const int OLE_E_CANT_BINDTOSOURCE = unchecked((int) 0x8004000A);
    public const int OLE_E_STATIC = unchecked((int) 0x8004000B);
    public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int) 0x8004000C);
    public const int OLE_E_INVALIDRECT = unchecked((int) 0x8004000D);
    public const int OLE_E_WRONGCOMPOBJ = unchecked((int) 0x8004000E);
    public const int OLE_E_INVALIDHWND = unchecked((int) 0x8004000F);
    public const int OLE_E_NOT_INPLACEACTIVE = unchecked((int) 0x80040010);
    public const int OLE_E_CANTCONVERT = unchecked((int) 0x80040011);
    public const int OLE_E_NOSTORAGE = unchecked((int) 0x80040012);
    public const int RPC_E_RETRY = unchecked((int) 0x80010109); */
  }
}