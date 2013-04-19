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

namespace DesktopGap.AddIns.Services
{
  public class ServiceManager : AddInManagerBase<IServiceAddIn>, IServiceManager
  {
    private readonly IDictionary<string, IServiceAddIn> _services =
        new Dictionary<string, IServiceAddIn>();


    public ServiceManager (
        IList<IServiceAddIn> sharedAddIns, IList<IServiceAddIn> nonSharedAddIns, HtmlDocumentHandle documentHandle, HtmlDocumentHandle document)
        : base (sharedAddIns, nonSharedAddIns, documentHandle)
    {
      NonSharedAddInLoaded += (s, a) => RegisterService (a.AddIn);
      SharedAddInLoaded += (s, a) => RegisterService (a.AddIn);
    }

    protected override void Dispose (bool disposing)
    {
      // pass
    }


    public IServiceAddIn GetService (string serviceName)
    {
      IServiceAddIn service;
      if (!_services.TryGetValue (serviceName, out service))
        throw MissingRegistration (serviceName);

      return service;
    }

    public bool HasService (string name)
    {
      IServiceAddIn s;
      return _services.TryGetValue (name, out s);
    }


    private void RegisterService (IServiceAddIn service)
    {
      if (HasService (service.Name))
        throw DuplicateRegistration (service.Name);

      _services[service.Name] = service;
    }
  }
}