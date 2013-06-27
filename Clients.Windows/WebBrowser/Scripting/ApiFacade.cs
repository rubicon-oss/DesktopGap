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
using System.Runtime.InteropServices;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Scripting
{

  [ComVisible (true)]
  public class ApiFacade
  {
    public readonly IHtmlDocumentHandleRegistry _htmlDocumentHandleRegistry;

    public ApiFacade (IHtmlDocumentHandleRegistry htmlDocumentHandleRegistry)
    {
      ArgumentUtility.CheckNotNull ("htmlDocumentHandleRegistry", htmlDocumentHandleRegistry);

      _htmlDocumentHandleRegistry = htmlDocumentHandleRegistry;
    }
    

    //
    // Services
    //

    public object GetService (string guid, string serviceName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serviceName", serviceName);
      ArgumentUtility.CheckNotNullOrEmpty ("guid", guid);

      return _htmlDocumentHandleRegistry.GetServiceManager (new HtmlDocumentHandle (Guid.Parse (guid))).GetService (serviceName);
    }

    public bool HasService (string guid, string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("guid", guid);

      return _htmlDocumentHandleRegistry.GetServiceManager (new HtmlDocumentHandle (Guid.Parse (guid))).HasService (name);
    }

    //
    // Events
    //

    public void AddEventListener (string documentID, string eventName, string callbackName, string moduleName, dynamic argument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("documentID", documentID);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("callbackName", callbackName);

      Condition eventArgument = null;

      if (argument != null)
      {
        try // TODO find a better solution
        {
          eventArgument = new Condition (argument);
        }
        catch (Exception ex)
        {
          throw new Exception ("argument is the wrong class"); // TODO use proper exception class
        }
      }

      var eventDispatcher = _htmlDocumentHandleRegistry.GetEventDispatcher (new HtmlDocumentHandle (Guid.Parse (documentID)));
      eventDispatcher.Register (eventName, callbackName, moduleName, eventArgument);
    }

    public void RemoveEventListener (string documentID, string eventName, string callbackName, string moduleName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNullOrEmpty ("documentID", documentID);
      ArgumentUtility.CheckNotNullOrEmpty ("callbackName", callbackName);

      _htmlDocumentHandleRegistry.GetEventDispatcher (new HtmlDocumentHandle (Guid.Parse (documentID))).Unregister (
          eventName, callbackName, moduleName);
    }

    public bool HasEvent (string documentID, string moduleName, string eventName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("moduleName", moduleName);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);
      ArgumentUtility.CheckNotNullOrEmpty ("documentID", documentID);

      return _htmlDocumentHandleRegistry.GetEventDispatcher (new HtmlDocumentHandle (Guid.Parse (documentID))).HasEvent (moduleName, eventName);
    }
  }
}

