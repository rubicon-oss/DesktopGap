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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Scripting
{
  [ComVisible (true)]
  public class ApiFacade : IDisposable
  {
    public IServiceManager ServiceManager { get; private set; }
    private readonly Func<IEventDispatcher> _eventDispatcherFactory;

    private readonly IDictionary<Guid, KeyValuePair<IEventDispatcher, HtmlDocument>> _targets = new Dictionary<Guid, KeyValuePair<IEventDispatcher, HtmlDocument>>();
    private const string c_addDocumentIdentification = "dg_assignID";


    public ApiFacade (IServiceManager serviceManager, Func<IEventDispatcher> eventDispatcherFactory)
    {
      ArgumentUtility.CheckNotNull ("serviceManager", serviceManager);
      ArgumentUtility.CheckNotNull ("eventDispatcherFactory", eventDispatcherFactory);
      
      _eventDispatcherFactory = eventDispatcherFactory;
      ServiceManager = serviceManager;
    }

    public void Dispose ()
    {
      ServiceManager.Dispose();
      foreach (var keyValuePair in _targets)
      {
        keyValuePair.Value.Key.Dispose();
      }
      _targets.Clear();
    }

    //
    // Services
    //

    public object GetService (string serviceName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serviceName", serviceName);

      return ServiceManager.GetService (serviceName);
    }

    public bool HasService (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return ServiceManager.HasService (name);
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
      

      EventArgument eventArgument = null;

      if (argument != null)
      {
        try // TODO find a better solution
        {
          eventArgument = new EventArgument (argument);
        }
        catch (Exception ex)
        {
          throw new Exception ("argument is the wrong class"); // TODO use proper exception class
        }
      }

      _targets[Guid.Parse (documentID)].Key.Register (eventName, callbackName, moduleName, eventArgument);
    }

    public void RemoveEventListener (string documentId, string eventName, string callbackName, string moduleName)
    {
      _targets[Guid.Parse (documentId)].Key.Unregister (eventName, callbackName, moduleName);
    }

    public bool HasEvent (string documentId, string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("documentId", documentId);

      return _targets[Guid.Parse (documentId)].Key.HasEvent (name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlDocument"></param>
    internal void AddDocument (HtmlDocument htmlDocument)
    {
      var guid = Guid.NewGuid();
      htmlDocument.InvokeScript (c_addDocumentIdentification, new object[] { guid.ToString() });

      var eventDispatcher = _eventDispatcherFactory();
      eventDispatcher.EventFired += (s, a) => htmlDocument.InvokeScript (a.Function, new object[] { a.Serialize() });

      _targets.Add (guid, new KeyValuePair<IEventDispatcher, HtmlDocument> (eventDispatcher, htmlDocument));
    }
  }
}