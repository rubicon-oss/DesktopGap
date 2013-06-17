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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Events.Subscriptions;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Resources;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using Microsoft.CSharp.RuntimeBinder;

namespace DesktopGap.AddIns
{
  [PartCreationPolicy (CreationPolicy.Shared)]
  public class DragAndDropEvent : ExternalEventBase, IBrowserEventSubscriber
  {
    private const string c_name = "DragAndDrop";
    private const string c_dragEnterEventName = "DragEnter";
    private const string c_dragDropEventName = "DragDrop";
    private const string c_dragLeaveEventName = "DragLeave";
    private const string c_dragOverEventName = "DragOver";

    public const string DroppableAttributeName = "dg_droptarget";
    public const string DropConditionAttributeName = "dg_dropcondition";

    private const int c_dragdropEffectCount = 4;

    public event ScriptEvent DragDrop;
    public event ScriptEvent DragEnter;
    public event ScriptEvent DragLeave;
    public event ScriptEvent DragOver;

    private KeyValuePair<HtmlDocumentHandle, HtmlElementData> _elementUnderCursor = new KeyValuePair<HtmlDocumentHandle, HtmlElementData>();
    private KeyValuePair<HtmlDocumentHandle, HtmlElementData> _enterElement = new KeyValuePair<HtmlDocumentHandle, HtmlElementData>();

    public DragAndDropEvent ()
    {
    }

    public DragAndDropEvent (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      ResourceManager = resourceManager;
    }

    public override void Dispose ()
    {
      DragEnter = null;
      DragDrop = null;
      DragLeave = null;
      DragOver = null;
    }

    public override string Name
    {
      get { return c_name; }
    }

    public override bool CheckRaiseCondition (Condition argument)
    {
      ArgumentUtility.CheckNotNull ("argument", argument);

      try
      {
        var elementID = argument.Criteria.elementID;
        return _elementUnderCursor.Value != null && elementID == _elementUnderCursor.Value.ID && argument.Document.Equals (_elementUnderCursor.Key);
      }
      catch (RuntimeBinderException)
      {
        throw new ArgumentException ("The provided 'Criteria' object does not have the required property 'elementID'.");
      }
    }

    public override void RegisterEvents (IEventHost eventHost)
    {
      ArgumentUtility.CheckNotNull ("eventHost", eventHost);

      eventHost.RegisterEvent (this, ref DragDrop, c_dragDropEventName);
      eventHost.RegisterEvent (this, ref DragEnter, c_dragEnterEventName);
      eventHost.RegisterEvent (this, ref DragLeave, c_dragLeaveEventName);
      eventHost.RegisterEvent (this, ref DragOver, c_dragOverEventName);
    }

    public override void UnregisterEvents (IEventHost eventHost)
    {
      ArgumentUtility.CheckNotNull ("eventHost", eventHost);

      eventHost.UnregisterEvent (this, ref DragDrop, c_dragDropEventName);
      eventHost.UnregisterEvent (this, ref DragEnter, c_dragEnterEventName);
      eventHost.UnregisterEvent (this, ref DragLeave, c_dragLeaveEventName);
      eventHost.UnregisterEvent (this, ref DragOver, c_dragOverEventName);
    }

    public void AddDragDropSource (IExtendedWebBrowser webBrowser)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      webBrowser.DragDrop += OnDragDrop;
      webBrowser.DragEnter += OnDragEnter;
      webBrowser.DragLeave += OnDragLeave;
      webBrowser.DragOver += OnDragOver;
    }

    public void RemoveDragDropSource (IExtendedWebBrowser webBrowser)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      webBrowser.DragDrop -= OnDragDrop;
      webBrowser.DragEnter -= OnDragEnter;
      webBrowser.DragLeave -= OnDragLeave;
      webBrowser.DragOver -= OnDragOver;
    }

    // when hovering things
    public void OnDragOver (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = e.Current;
      if (_elementUnderCursor.Value == null || (current != null && !_elementUnderCursor.Value.Equals (current)))
        _elementUnderCursor = new KeyValuePair<HtmlDocumentHandle, HtmlElementData> (e.Document, current);

      e.Droppable = IsDroppable (current);

      var args = new DragEventData();
      args.Names = GetFileNames (e.Data);

      if (DragOver != null)
        DragOver (this, c_dragOverEventName, args);

      var effect = GetMouseEffect (current);
      if (e.Droppable && effect > 0)
        e.Effect = (DragDropEffects) effect;
    }

    // when leaving the window
    public void OnDragLeave (object sender, EventArgs e)
    {
      if (DragLeave != null)
        DragLeave (this, c_dragLeaveEventName, new DragEventData());
    }

    // when entering the window
    public void OnDragEnter (object sender, ExtendedDragEventHandlerArgs e)
    {
      if (e.Current != null)
        _enterElement = new KeyValuePair<HtmlDocumentHandle, HtmlElementData> (e.Document, e.Current);
      _elementUnderCursor = _enterElement;

      var args = new DragDropEventData();
      args.Names = GetFileNames (e.Data);

      if (DragEnter != null)
        DragEnter (this, c_dragEnterEventName, args);
    }

    public void OnDragDrop (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = e.Current;
      e.Droppable = IsDroppable (current);

      if (DragDrop != null && e.Droppable)
      {
        var paths = GetFileNames (e.Data);
        var resources = paths.Select (
            p => (File.GetAttributes (p) & FileAttributes.Directory) == FileAttributes.Directory
                     ? new DirectoryInfo (p) as FileSystemInfo
                     : new FileInfo (p) as FileSystemInfo
            ).ToArray();

        var handles = ResourceManager.AddResources (resources);

        var args = new DragDropEventData
                   {
                       ResourceHandles = handles.Select (r => r.ToString()).ToArray(),
                       Names = paths
                   };

        DragDrop (this, c_dragDropEventName, args);
      }
    }

    private bool IsDroppable (HtmlElementData element)
    {
      var result = false;
      string value;

      if (element != null && element.Attributes.TryGetValue (DroppableAttributeName, out value))
        Boolean.TryParse (value, out result);

      return result && GetMouseEffect (element) != 0;
    }

    private int GetMouseEffect (HtmlElementData element)
    {
      string value;
      var result = 0;

      if (element != null && element.Attributes.TryGetValue (DropConditionAttributeName, out value))
        Int32.TryParse (value, out result);

      return result % c_dragdropEffectCount;
    }

    private string[] GetFileNames (IDataObject data) // TODO-MK Replace with proper implementation
    {
      var paths = new string[0];

      if (data != null) // TODO known issue: dragging images within the web page crashes DG
        paths = ((string[]) data.GetData (DataFormats.FileDrop));

      return paths;
    }
  }
}