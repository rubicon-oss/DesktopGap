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
using System.Dynamic;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class EventManagerTest
  {
    private HtmlDocumentHandle? _consistentDocumentHandle = null;
    private const string c_fakeEventID = "some event";

    [Test]
    public void Register_EventDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventName = "some event";
      var moduleName = "some module";


      Assert.That (
          () => eventManager.Register (eventName, "some callback", moduleName, new Condition (CreateCondition())),
          Throws.InvalidOperationException.
              With.Message.EqualTo (string.Format ("Event {0} in module {1} not found.", eventName, moduleName)));
    }

    [Test]
    public void Register_InvalidConditionObject_ShouldThrowInvalidOperation ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventName = "some event";
      var moduleName = "some module";

      Assert.That (
          () => eventManager.Register (eventName, "some callback", moduleName, new Condition (new object())),
          Throws.ArgumentException.
              With.Message.EqualTo ("The provided object does not have the required properties 'EventID', 'DocumentHandle', and 'Criteria'."));
    }

    [Test]
    public void Register_CallbackDoesNotExist_ShouldThrowInvalidOperation ()
    {
    }

    [Test]
    public void Unregister_EventDoesNotExist_ShouldThrowInvalidOperation ()
    {
    }

    [Test]
    public void Unregister_CallbackDoesNotExist_ShouldThrowNothing ()
    {
    }

    private HtmlDocumentHandle GetDocumentHandle ()
    {
      if (_consistentDocumentHandle == null)
      {
        var guid = Guid.NewGuid();
        _consistentDocumentHandle = new HtmlDocumentHandle (guid);
      }
      return (HtmlDocumentHandle) _consistentDocumentHandle;
    }

    private dynamic CreateCondition ()
    {
      dynamic fakeCondition = new ExpandoObject();


      fakeCondition.EventID = c_fakeEventID;
      fakeCondition.DocumentHandle = GetDocumentHandle().ToString();
      fakeCondition.Criteria = new object();
      return fakeCondition;
    }

    private EventManager CreateEventManager (IList<IEventAddIn> additionalEvents)
    {
      var handle = GetDocumentHandle();

      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add (new TypeCatalog());
      var compositionContainer = new CompositionContainer (catalog);

      var eventManager = new EventManager (handle, additionalEvents);
      compositionContainer.ComposeParts (eventManager);
      return eventManager;
    }
  }
}