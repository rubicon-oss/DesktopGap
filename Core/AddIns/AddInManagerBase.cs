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
using System.Linq;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns
{
  public abstract class AddInManagerBase<TAddIn> : IDisposable
      where TAddIn : AddInBase
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

    private readonly IEnumerable<TAddIn> _sharedAddIns;
    private readonly IEnumerable<TAddIn> _nonSharedAddIns;

    private readonly HtmlDocumentHandle _documentHandle;

    protected event EventHandler<AddInLoadedEventArgs> NonSharedAddInLoaded;
    protected event EventHandler<AddInLoadedEventArgs> SharedAddInLoaded;
    protected event EventHandler<AddInLoadedEventArgs> NonSharedAddInUnloaded;
    protected event EventHandler<AddInLoadedEventArgs> SharedAddInUnloaded;

    protected AddInManagerBase ()
    {
      _sharedAddIns = new HashSet<TAddIn>();
      _nonSharedAddIns = new HashSet<TAddIn>();
    }

    protected AddInManagerBase (IEnumerable<TAddIn> sharedAddIns, IEnumerable<TAddIn> nonSharedAddIns, HtmlDocumentHandle documentHandle)
    {
      ArgumentUtility.CheckNotNull ("nonSharedAddIns", nonSharedAddIns);
      ArgumentUtility.CheckNotNull ("nonSharedAddIns", nonSharedAddIns);

      _sharedAddIns = sharedAddIns;
      _nonSharedAddIns = nonSharedAddIns;
      _documentHandle = documentHandle;
    }

    public void Dispose ()
    {
      Dispose (true);
      UnloadAddIns();
    }


    protected abstract void Dispose (bool disposing);

    public void LoadAddIns ()
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

    public IEnumerable<TSubscriber> GetSubscribers<TSubscriber> ()
    {
      return _sharedAddIns.OfType<TSubscriber>()
          .Concat (_nonSharedAddIns.OfType<TSubscriber>());
    }
  }
}