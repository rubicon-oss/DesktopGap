﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using DesktopGap.Resources;

namespace DesktopGap.AddIns.Services
{
  [InheritedExport (typeof (ExternalServiceBase))]
  [ComVisible (true)]
  public abstract class ExternalServiceBase : IDisposable
  {
    [Import (typeof (IResourceManager), AllowDefault = true)]
    protected IResourceManager ResourceManager { get; private set; }

    public abstract String Name { get; }
    public abstract void Dispose ();

    public abstract void OnBeforeLoad ();
    public abstract void OnBeforeUnload ();
  }
}