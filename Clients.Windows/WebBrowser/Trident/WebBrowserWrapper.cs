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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  [ComVisible (true)]
  public class WebBrowserWrapper : RealProxy, IWebBrowser2, IRemotingTypeInfo
  {
    private readonly IWebBrowser2 _inner;
    private readonly Type _targetType;

    private WebBrowserWrapper (IWebBrowser2 target, Type type)
        : base (type)
    {
      _inner = target;
      _targetType = GetType();
    }


    public static object CreateInstance (IWebBrowser2 target, Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("target", target);
      return new WebBrowserWrapper (target, type).GetTransparentProxy();
    }

    public override ObjRef CreateObjRef (Type requestedType)
    {
      throw new NotSupportedException ("Remoting of an interposed object is not supported.");
    }

    public override IMessage Invoke (IMessage message)
    {
      IMethodCallMessage methodMessage = new MethodCallMessageWrapper ((IMethodCallMessage) message);

      var methodInfo = _targetType.GetMethod (methodMessage.MethodBase.Name);
      var returnValue = methodInfo.Invoke (this, methodMessage.Args);

      // Finally, form a return message containing the return 
      // value along with any and all parameters that may have been 
      // modified by the method call (those that are out or ref). 
      return new ReturnMessage (
          returnValue,
          methodMessage.Args,
          methodMessage.ArgCount,
          methodMessage.LogicalCallContext,
          methodMessage);
    }


    public object Application
    {
      get { return _inner.Application; }
    }

    public object Parent
    {
      get { return _inner.Parent; }
    }

    public object Container
    {
      get { return _inner.Container; }
    }

    public object Document
    {
      get { return _inner.Document; }
    }

    public bool TopLevelContainer
    {
      get { return _inner.TopLevelContainer; }
    }

    public string Type
    {
      get { return _inner.Type; }
    }

    public int Left
    {
      get { return _inner.Left; }
      set { _inner.Left = value; }
    }

    public int Top
    {
      get { return _inner.Top; }
      set { _inner.Top = value; }
    }

    public int Width
    {
      get { return _inner.Width; }
      set { _inner.Width = value; }
    }

    public int Height
    {
      get { return _inner.Height; }
      set { _inner.Height = value; }
    }

    public string LocationName
    {
      get { return _inner.LocationName; }
    }

    public string LocationURL
    {
      get { return _inner.LocationURL; }
    }

    public bool Busy
    {
      get { return _inner.Busy; }
    }

    public string Name
    {
      get { return _inner.Name; }
    }

    public int HWND
    {
      get { return _inner.HWND; }
    }

    public string FullName
    {
      get { return _inner.FullName; }
    }

    public string Path
    {
      get { return _inner.Path; }
    }

    public bool Visible
    {
      get { return _inner.Visible; }
      set { _inner.Visible = value; }
    }

    public bool StatusBar
    {
      get { return _inner.StatusBar; }
      set { _inner.StatusBar = value; }
    }

    public string StatusText
    {
      get { return _inner.StatusText; }
      set { _inner.StatusText = value; }
    }

    public int ToolBar
    {
      get { return _inner.ToolBar; }
      set { _inner.ToolBar = value; }
    }

    public bool MenuBar
    {
      get { return _inner.MenuBar; }
      set { _inner.MenuBar = value; }
    }

    public bool FullScreen
    {
      get { return _inner.FullScreen; }
      set { _inner.FullScreen = value; }
    }

    public tagREADYSTATE ReadyState
    {
      get { return _inner.ReadyState; }
    }

    public bool Offline
    {
      get { return _inner.Offline; }
      set { _inner.Offline = value; }
    }

    public bool Silent
    {
      get { return _inner.Silent; }
      set { _inner.Silent = value; }
    }

    public bool RegisterAsBrowser
    {
      get { return _inner.RegisterAsBrowser; }
      set { _inner.RegisterAsBrowser = value; }
    }

    public bool RegisterAsDropTarget
    {
      get { return _inner.RegisterAsDropTarget; }
      set { _inner.RegisterAsDropTarget = value; }
    }

    public bool TheaterMode
    {
      get { return _inner.TheaterMode; }
      set { _inner.TheaterMode = value; }
    }

    public bool AddressBar
    {
      get { return _inner.AddressBar; }
      set { _inner.AddressBar = value; }
    }

    public bool Resizable
    {
      get { return _inner.Resizable; }
      set { _inner.Resizable = value; }
    }

    public void GoBack ()
    {
      // NOOP
    }

    public void GoForward ()
    {
      // NOOP
    }

    public void GoHome ()
    {
      // NOOP no more
    }

    public void GoSearch ()
    {
    }

    public void Navigate (string URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers)
    {
      _inner.Navigate (URL, ref Flags, ref TargetFrameName, ref PostData, ref Headers);
    }

    public void Refresh ()
    {
      // NOOP
    }

    public void Refresh2 (ref object Level)
    {
      // NOOP
    }

    public void Stop ()
    {
      // NOOP
    }


    public void Quit ()
    {
      _inner.Quit();
    }

    public void ClientToWindow (ref int pcx, ref int pcy)
    {
      _inner.ClientToWindow (ref pcx, ref pcy);
    }

    public void PutProperty (string Property, object vtValue)
    {
      _inner.PutProperty (Property, vtValue);
    }

    public object GetProperty (string Property)
    {
      return _inner.GetProperty (Property);
    }


    public void Navigate2 (ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers)
    {
      _inner.Navigate2 (ref URL, ref Flags, ref TargetFrameName, ref PostData, ref Headers);
    }

    public OLECMDF QueryStatusWB (OLECMDID cmdId)
    {
      switch (cmdId)
      {
        case OLECMDID.OLECMDID_SPELL:

        case OLECMDID.OLECMDID_CUT:
        case OLECMDID.OLECMDID_COPY:
        case OLECMDID.OLECMDID_PASTE:
        case OLECMDID.OLECMDID_PASTESPECIAL:
        case OLECMDID.OLECMDID_UNDO:
        case OLECMDID.OLECMDID_REDO:
        case OLECMDID.OLECMDID_SELECTALL:
        case OLECMDID.OLECMDID_FIND:
        case OLECMDID.OLECMDID_DELETE:
          return OLECMDF.OLECMDF_ENABLED;
        default:
          return OLECMDF.OLECMDF_INVISIBLE;
      }
    }

    public void ExecWB (OLECMDID cmdID, OLECMDEXECOPT cmdexecopt, ref object pvaIn, ref object pvaOut)
    {
      switch (cmdID)
      {
        case OLECMDID.OLECMDID_SPELL:

        case OLECMDID.OLECMDID_CUT:
        case OLECMDID.OLECMDID_COPY:
        case OLECMDID.OLECMDID_PASTE:
        case OLECMDID.OLECMDID_PASTESPECIAL:
        case OLECMDID.OLECMDID_UNDO:
        case OLECMDID.OLECMDID_REDO:
        case OLECMDID.OLECMDID_SELECTALL:
        case OLECMDID.OLECMDID_FIND:
        case OLECMDID.OLECMDID_DELETE:
          _inner.ExecWB (cmdID, cmdexecopt, ref pvaIn, pvaOut);
          break;
      }
    }


    public void ShowBrowserBar (ref object pvaClsid, ref object pvarShow, ref object pvarSize)
    {
      _inner.ShowBrowserBar (ref pvaClsid, ref pvarShow, ref pvarSize);
    }

    public bool CanCastTo (Type fromType, object o)
    {
      return true;
    }

    public string TypeName { get; set; }
  }
}