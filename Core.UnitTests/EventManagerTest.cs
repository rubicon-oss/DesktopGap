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
using System.Linq;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.UnitTests.Fakes;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class EventManagerTest
  {
    private const string c_eventNotFoundFormatString = "Event '{0}' in module '{1}' was not found.";
    private const string c_fakeEventID = "GUID identifying the event in JS";

    private HtmlDocumentHandle? _consistentDocumentHandle;


    [Test]
    public void Register_EventDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventName = "some event";
      var moduleName = "some module";

      Assert.That (
          () => eventManager.Register (eventName, "some callback", moduleName, new Condition (CreateCondition())),
          Throws.InvalidOperationException.
              With.Message.EqualTo (string.Format (c_eventNotFoundFormatString, eventName, moduleName)));
    }

    [Test]
    public void Register_InvalidConditionObject_ShouldThrowArgumentException ()
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
    public void Unregister_EventDoesNotExist_ShouldThrowInvalidOperation ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventName = "some event";
      var moduleName = "some module";

      Assert.That (
          () => eventManager.Unregister (eventName, "some callback", moduleName),
          Throws.InvalidOperationException.
              With.Message.EqualTo (string.Format (c_eventNotFoundFormatString, eventName, moduleName)));
    }


    [Test]
    public void RegisterEvent_EventRegistration_ShouldSucceed ()
    {
      ScriptEvent scriptEvent = null;
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventAddIn = new FakeEventAddIn();

      Assert.That (
          () => eventManager.RegisterEvent (eventAddIn, ref scriptEvent, eventAddIn.FakeEventName),
          Throws.Nothing);
      Assert.That (scriptEvent, Is.Not.Null);
      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.True);
    }

    [Test]
    public void RegisterEvent_EventDuplicateRegistration_ShouldThrowInvalidOperation ()
    {
      ScriptEvent scriptEvent = null;
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventAddIn = new FakeEventAddIn();

      Assert.That (
          () => eventManager.RegisterEvent (eventAddIn, ref scriptEvent, eventAddIn.FakeEventName),
          Throws.Nothing);
      Assert.That (
          () => eventManager.RegisterEvent (eventAddIn, ref scriptEvent, eventAddIn.FakeEventName),
          Throws.InvalidOperationException.With.Message.Contains ("already registered."));

      Assert.That (scriptEvent, Is.Not.Null);
      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.True);
    }


    [Test]
    public void UnegisterEvent_EventDeregistration_ShouldSucceed ()
    {
      ScriptEvent scriptEvent = null;
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventAddIn = new FakeEventAddIn();

      Assert.That (
          () => eventManager.RegisterEvent (eventAddIn, ref scriptEvent, eventAddIn.FakeEventName),
          Throws.Nothing);

      Assert.That (
          () => eventManager.UnregisterEvent (eventAddIn, ref scriptEvent, eventAddIn.FakeEventName),
          Throws.Nothing);
      Assert.That (scriptEvent, Is.Null);
      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.False);
      Assert.That (eventAddIn.IsLoaded && eventAddIn.EventsRegistered, Is.False);
    }

    [Test]
    public void HasEvent_EventDoesNotExist_ShouldReturnFalse ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());

      var eventName = "some event";
      var moduleName = "some module";

      Assert.That (eventManager.HasEvent (moduleName, eventName), Is.False);
    }

    [Test]
    public void HasEvent_EventDoesExist_ShouldReturnTrue ()
    {
      var eventManager = CreateEventManager (new List<IEventAddIn>());
      var eventAddIn = new FakeEventAddIn();
      eventAddIn.RegisterEvents (eventManager);

      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.True);
    }


    [Test]
    public void Constructor_AddSharedExternalEvents_ShouldSucceed ()
    {
      var eventAddIn = new FakeEventAddIn();
      var eventManager = CreateEventManager (new[] { eventAddIn });

      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.True);
      Assert.That (eventAddIn.IsLoaded && eventAddIn.EventsRegistered, Is.True);
    }

    [Test]
    public void Constructor_AddEventDuplicate_ShouldThrowInvalidOperation ()
    {
      var eventAddIn = new FakeEventAddIn();

      var compositionException = Assert.Throws<CompositionException> (
          () => CreateEventManager (new[] { eventAddIn, eventAddIn }));
      Assert.That (
          compositionException.RootCauses.First().InnerException, Is.InstanceOf<InvalidOperationException>());
    }

    [Test]
    public void Event_FireSharedEvent_ShouldSucceed ()
    {
      var eventAddIn = new FakeEventAddIn();
      var eventManager = CreateEventManager (new[] { eventAddIn });

      var wasCalled = false;
      eventManager.EventFired += (sender, args) =>
                                 {
                                   Assert.That (args.ScriptArgs, Is.TypeOf<FakeEventData>());
                                   Assert.That (args.ScriptArgs.EventID, Is.EqualTo (c_fakeEventID));
                                   Assert.That (((FakeEventData) args.ScriptArgs).ContainsData, Is.True);
                                   wasCalled = true;
                                 };
      eventManager.Register (eventAddIn.FakeEventName, "some callback", eventAddIn.Name, new Condition (CreateCondition()));
      Assert.That (() => eventAddIn.RaiseEvent(), Throws.Nothing);
      Assert.That (wasCalled, Is.True);
    }

    [Test]
    public void Dispose_SharedEventIsNotDisposed_ShouldSucceed ()
    {
      var eventAddIn = new FakeEventAddIn();
      var eventManager = CreateEventManager (new[] { eventAddIn });

      Assert.That (eventManager.HasEvent (eventAddIn.Name, eventAddIn.FakeEventName), Is.True);
      Assert.That (eventAddIn.IsLoaded && eventAddIn.EventsRegistered, Is.True);

      eventManager.Dispose();
      Assert.That (eventAddIn, Is.Not.Null);
      Assert.That (eventAddIn.IsLoaded && eventAddIn.EventsRegistered, Is.False);
    }

    [Test]
    public void Event_FireUnregisteredEventInRegisteredModule_ShouldThrow ()
    {
      var eventAddIn = new FakeEventAddIn();
      var eventManager = CreateEventManager (new[] { eventAddIn });

      eventManager.EventFired += (sender, args) => { }; // will never be called

      eventManager.Register (eventAddIn.FakeEventName, "some callback", eventAddIn.Name, new Condition (CreateCondition()));
      eventAddIn.FakeEventName = "some other event";
      Assert.That (
          () => eventAddIn.RaiseEvent(),
          Throws.InvalidOperationException
              .With.Message.EqualTo (string.Format ("Event '{0}' in module '{1}' never registered.", eventAddIn.FakeEventName, eventAddIn.Name)));
    }


    //
    // UTILITY FUNCTIONS
    //

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