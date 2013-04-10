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
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Services;

namespace DesktopGap.WebBrowser.Session
{
  public class Session : ISession
  {
    public IEnumerable<IExtendedWebBrowser> WebBrowsers { get; private set; }
    public IEventDispatcher EventManager { get; private set; }
    public IServiceManager ServiceManager { get; private set; }

    public Session ()
    {
    }

    public Session (IEnumerable<IExtendedWebBrowser> webBrowsers, IEventDispatcher eventManager, IServiceManager serviceManager)
    {
      if (webBrowsers == null)
        throw new ArgumentNullException ("webBrowsers");
      if (eventManager == null)
        throw new ArgumentNullException ("eventManager");
      if (serviceManager == null)
        throw new ArgumentNullException ("serviceManager");

      WebBrowsers = webBrowsers;
      EventManager = eventManager;
      ServiceManager = serviceManager;
    }
  }
}