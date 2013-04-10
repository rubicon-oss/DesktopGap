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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Events.Factory;
using DesktopGap.AddIns.Services;
using DesktopGap.AddIns.Services.Factory;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.Clients.Windows.WebBrowser.Scripting
{
  [ComVisible (true)]
  public class ApiFacade : IDisposable
  {
    private readonly IServiceManagerFactory _serviceManagerFactory;
    private readonly IEventDispatcherFactory _eventDispatcherFactory;
    private readonly IAddInManager _addInManager;
    private const string c_getDocumentIdentification = "dg_retrieveID";

    private const string c_addDocumentIdentification = "dg_assignID";


    public ApiFacade (IServiceManagerFactory serviceManagerFactory, IEventDispatcherFactory eventDispatcherFactory, IAddInManager addInManager)
    {
      ArgumentUtility.CheckNotNull ("serviceManagerFactory", serviceManagerFactory);
      ArgumentUtility.CheckNotNull ("eventDispatcherFactory", eventDispatcherFactory);
      ArgumentUtility.CheckNotNull ("addInManager", addInManager);

      _eventDispatcherFactory = eventDispatcherFactory;
      _addInManager = addInManager;
      _serviceManagerFactory = serviceManagerFactory;
    }

    public void Dispose ()
    {
      _addInManager.Dispose();
    }

    //
    // Services
    //

    public object GetService (string guid, string serviceName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serviceName", serviceName);
      ArgumentUtility.CheckNotNullOrEmpty ("guid", guid);


      return _addInManager.GetServiceManager (new HtmlDocumentHandle(Guid.Parse (guid))).GetService (serviceName);
    }

    public bool HasService (string guid, string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("guid", guid);

      return _addInManager.GetServiceManager (new HtmlDocumentHandle(Guid.Parse (guid))).HasService (name);
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

      _addInManager.GetEventDispatcher (new HtmlDocumentHandle(Guid.Parse (documentID))).Register (eventName, callbackName, moduleName, eventArgument);
    }

    public void RemoveEventListener (string documentID, string eventName, string callbackName, string moduleName)
    {
      _addInManager.GetEventDispatcher (new HtmlDocumentHandle(Guid.Parse (documentID))).Unregister (eventName, callbackName, moduleName);
    }

    public bool HasEvent (string documentID, string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("documentID", documentID);

      return _addInManager.GetEventDispatcher (new HtmlDocumentHandle(Guid.Parse (documentID))).HasEvent (name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlDocument"></param>
    internal void AddDocument (HtmlDocument htmlDocument)
    {
      var docID = new HtmlDocumentHandle(Guid.NewGuid());
      htmlDocument.InvokeScript (c_addDocumentIdentification, new object[] { docID.ToString() });

            var serviceManager = _serviceManagerFactory.CreateServiceManager(docID);

      var eventDispatcher = _eventDispatcherFactory.CreateEventDispatcher(docID);
      
      // dndevent = new DnDEvent();
      // browser.DragEnter += dndevent.DragEnter;
      // eventDispatcher.AddEvent(dndevent);
      _addInManager.AddEventDispatcher (docID, eventDispatcher);

      eventDispatcher.EventFired += (s, a) => htmlDocument.InvokeScript (a.Function, new object[] { a.Serialize() });

      _addInManager.AddServiceManager (docID, serviceManager);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlDocument"></param>
    internal void RemoveDocument (HtmlDocument htmlDocument)
    {
      var guidObj = htmlDocument.InvokeScript (c_getDocumentIdentification);
     
      if(guidObj == null)
        return;

      var guid = guidObj.ToString();
      var docID = new HtmlDocumentHandle(Guid.Parse(guid));

      _addInManager.RemoveEventDispatcher (docID);
      _addInManager.RemoveEventDispatcher(docID);
    }
  }
}