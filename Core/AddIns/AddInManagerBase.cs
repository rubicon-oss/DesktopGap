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
using DesktopGap.Utilities;

namespace DesktopGap.AddIns
{
  public abstract class AddInManagerBase<TAddIn> : IDisposable
      where TAddIn : IAddIn
  {
    protected class AddInLoadedEventArgs : EventArgs
    {
      public TAddIn AddIn { get; private set; }

      public AddInLoadedEventArgs (TAddIn addIn)
      {
        ArgumentUtility.CheckNotNull ("addIn", addIn);
        AddIn = addIn;
      }
    }


    private const string c_duplicateRegistrationFormatString = "'{0}' is already registered.";
    private const string c_missingRegistrationFormatString = "Cannot find '{0}'.";

    private readonly IList<TAddIn> _sharedAddIns = new List<TAddIn>();
    private readonly IList<TAddIn> _nonSharedAddIns = new List<TAddIn>();

    private readonly HtmlDocumentHandle _documentHandle;

    protected event EventHandler<AddInLoadedEventArgs> NonSharedAddInLoaded;
    protected event EventHandler<AddInLoadedEventArgs> SharedAddInLoaded;
    protected event EventHandler<AddInLoadedEventArgs> NonSharedAddInUnloaded;
    protected event EventHandler<AddInLoadedEventArgs> SharedAddInUnloaded;

    protected AddInManagerBase ()
    {
      _sharedAddIns = new List<TAddIn>();
      _nonSharedAddIns = new List<TAddIn>();
    }

    protected AddInManagerBase (IList<TAddIn> sharedAddIns, IList<TAddIn> nonSharedAddIns, HtmlDocumentHandle documentHandle)
    {
      ArgumentUtility.CheckNotNull ("nonSharedAddIns", nonSharedAddIns);
      ArgumentUtility.CheckNotNull ("nonSharedAddIns", nonSharedAddIns);

      _sharedAddIns = sharedAddIns;
      _nonSharedAddIns = nonSharedAddIns;
      _documentHandle = documentHandle;
      LoadAddIns();
    }

    public void Dispose ()
    {
      Dispose (true);
      UnloadAddIns();
    }


    protected abstract void Dispose (bool disposing);

    protected void LoadAddIns ()
    {
      foreach (var addIn in _sharedAddIns)
      {
        addIn.OnBeforeLoad (_documentHandle);
        Notify (SharedAddInLoaded, new AddInLoadedEventArgs (addIn));
      }
      foreach (var addIn in _nonSharedAddIns)
      {
        addIn.OnBeforeLoad (_documentHandle);
        Notify (NonSharedAddInLoaded, new AddInLoadedEventArgs (addIn));
      }
    }

    protected void UnloadAddIns ()
    {
      foreach (var addIn in _sharedAddIns)
      {
        addIn.OnBeforeUnload (_documentHandle);
        Notify (SharedAddInUnloaded, new AddInLoadedEventArgs (addIn));
      }
      foreach (var addIn in _nonSharedAddIns)
      {
        addIn.OnBeforeUnload (_documentHandle);
        Notify (NonSharedAddInUnloaded, new AddInLoadedEventArgs (addIn));

        addIn.Dispose();
      }
    }

    protected Exception MissingRegistration (string name)
    {
      return new InvalidOperationException (string.Format (c_missingRegistrationFormatString, name));
    }

    protected Exception DuplicateRegistration (string name)
    {
      return new InvalidOperationException (string.Format (c_duplicateRegistrationFormatString, name));
    }

    private void Notify (EventHandler<AddInLoadedEventArgs> evnt, AddInLoadedEventArgs addInArgs)
    {
      if (evnt != null)
        evnt (this, addInArgs);
    }
  }
}