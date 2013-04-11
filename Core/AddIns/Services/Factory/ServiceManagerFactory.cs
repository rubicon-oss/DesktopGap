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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Services.Factory
{
  public class ServiceManagerFactory : IServiceManagerFactory
  {
    private readonly CompositionContainer _compositionContainer;
    private bool _factoryCalled;
    private readonly IList<IServiceAddIn> _preLoadedSharedServices;

    public ServiceManagerFactory (CompositionContainer compositionContainer)
    {
      ArgumentUtility.CheckNotNull ("compositionContainer", compositionContainer);
      _compositionContainer = compositionContainer;
      _preLoadedSharedServices = new List<IServiceAddIn>();
    }


    public void AddPreloadedService (IServiceAddIn addIn)
    {
      if (_factoryCalled)
        throw new InvalidOperationException ("An instance has already been created, you cannot add things anymore");

      _preLoadedSharedServices.Add (addIn);
    }

    public IServiceManager CreateServiceManager (HtmlDocumentHandle document)
    {
      try
      {
        var serviceManager = new ServiceManager (document, _preLoadedSharedServices);
        _compositionContainer.ComposeParts (serviceManager);
        return serviceManager;
      }
      finally
      {
        _factoryCalled = true;
      }
    }
  }
}