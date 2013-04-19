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
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Factory;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns
{
  public class EventManagerFactory<TFactoryBase>: IEventDispatcherFactory where TFactoryBase : AddInManagerFactoryBase<IEventAddIn>
  {
    private readonly TFactoryBase _factoryBase;
    private const string c_invalidGenericTypeMessage = "Factory has to return an instance of type IEventDispatcher.";

    public EventManagerFactory (TFactoryBase factoryBase)
    {
      ArgumentUtility.CheckNotNull ("factoryBase", factoryBase);
      
      _factoryBase = factoryBase;
    }

    public IEventDispatcher CreateEventDispatcher (HtmlDocumentHandle document)
    {
      var dispatcher = _factoryBase.CreateManager();
      
      if (dispatcher is IEventDispatcher)
        return dispatcher as IEventDispatcher;
      
      throw new ArgumentException (c_invalidGenericTypeMessage);
    }
  }
}