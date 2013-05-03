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
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DesktopGap.Security.AddIns;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Factories
{
  public class CompositionBasedAddInFactory<TAddIn> : IAddInFactory<TAddIn>
      where TAddIn : AddInBase
  {
    private readonly CompositionContainer _compositionContainer;
    private readonly IAddInFilter _addInFilter;

    private IEnumerable<TAddIn> _sharedAddIns;


    public CompositionBasedAddInFactory (CompositionContainer compositionContainer, IAddInFilter addInFilter)
    {
      ArgumentUtility.CheckNotNull ("compositionContainer", compositionContainer);
      ArgumentUtility.CheckNotNull ("addInFilter", addInFilter);

      _compositionContainer = compositionContainer;
      _addInFilter = addInFilter;
    }


    public IEnumerable<TAddIn> GetSharedAddIns ()
    {
      return _sharedAddIns ?? (_sharedAddIns = GetExports (CreationPolicy.Shared));
    }

    public IEnumerable<TAddIn> GetNonSharedAddIns ()
    {
      return GetExports (CreationPolicy.NonShared);
    }

    private IEnumerable<TAddIn> GetExports (CreationPolicy policy)
    {
      return
          _compositionContainer.GetExports (CreateImportManyDefiniton (policy))
              .Select (e => (TAddIn) e.Value)
              .Where (a => _addInFilter.IsAllowed (a.Name));
    }

    private ImportDefinition CreateImportManyDefiniton (CreationPolicy policy)
    {
      return new ContractBasedImportDefinition (
          AttributedModelServices.GetContractName (typeof (TAddIn)),
          AttributedModelServices.GetTypeIdentity (typeof (TAddIn)),
          null,
          ImportCardinality.ZeroOrMore,
          false,
          false,
          policy);
    }
  }
}