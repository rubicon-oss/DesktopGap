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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events.Factory;
using DesktopGap.AddIns.Events.System;
using DesktopGap.AddIns.Services.Factory;
using DesktopGap.Resources;
using DesktopGap.Utilities;
using DesktopGap.AddIns.Events;

namespace DesktopGap.WebBrowser.Factory
{
  public abstract class WebBrowserFactoryBase : IWebBrowserFactory
  {
    private readonly IResourceManager _resourceManager;
    private readonly CompositionContainer _compositionContainer;
    private readonly IEventAddIn[] _preLoadedAddIns;
    private readonly DragAndDropAddIn _dragAndDropAddIn;

    protected WebBrowserFactoryBase (ComposablePartCatalog catalog, IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("catalog", catalog);
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      _resourceManager = resourceManager;
      _compositionContainer = new CompositionContainer (catalog);

      _dragAndDropAddIn = new DragAndDropAddIn (_resourceManager);
      _preLoadedAddIns = new[]
                         {
                             (IEventAddIn) _dragAndDropAddIn
                         };
    }

    /// <summary>
    /// Method to construct the browser instance, should be overwritten; called by CreateBrowser().
    /// </summary>
    /// <param name="serviceManagerFactory"></param>
    /// <param name="eventDispatcherFactory"></param>
    /// <param name="addInManager"></param>
    /// <returns>A new web browser instance.</returns>
    protected abstract IExtendedWebBrowser CreateBrowser (
        IServiceManagerFactory serviceManagerFactory, IEventDispatcherFactory eventDispatcherFactory, IAddInManager addInManager);

    /// <summary>
    /// Factory method to create a web browser instance using the required parameters. For that, CreateBrowser(...) is called.
    /// </summary>
    /// <returns>A new webbrowser instance.</returns>
    public IExtendedWebBrowser CreateBrowser ()
    {
      var serviceManagerFactory = new ServiceManagerFactory (_compositionContainer);
      var eventDispatcherFactory = new EventDispatcherFactory (_compositionContainer);
      var addInManager = new AddInManager();


      var browser = CreateBrowser (serviceManagerFactory, eventDispatcherFactory, addInManager);

      _dragAndDropAddIn.AddDragDropSource (browser);

      foreach (var addIn in _preLoadedAddIns)
        eventDispatcherFactory.AddPreloadedEvent (addIn);

      return browser;
    }
  }
}