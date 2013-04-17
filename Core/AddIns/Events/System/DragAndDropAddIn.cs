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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Resources;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns.Events.System
{
  public class DragAndDropAddIn : IEventAddIn
  {
    private const string c_name = "DragAndDrop";
    private const string c_dragEnterEventName = "DragEnter";
    private const string c_dragDropEventName = "DragDrop";
    private const string c_dragLeaveEventName = "DragLeave";
    private const string c_dragOverEventName = "DragOver";
    private const string c_droppableAttributeName = "dg_droptarget";
    private const string c_dropconditionAttributeName = "dg_dropcondition";

    private const int c_dragdropEffectCount = 4;

    public event ScriptEvent DragDrop;
    public event ScriptEvent DragEnter;
    public event ScriptEvent DragLeave;
    public event ScriptEvent DragOver;

    private KeyValuePair<HtmlDocumentHandle, HtmlElementData> _elementUnderCursor;
    private KeyValuePair<HtmlDocumentHandle, HtmlElementData> _enterElement;

    private List<ResourceHandle> _currentResources = new List<ResourceHandle>();
    private List<string> _currentFilePaths = new List<string>();


    public DragAndDropAddIn (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      ResourceManager = resourceManager;
    }

    public void Dispose ()
    {
      DragEnter = null;
      DragDrop = null;
      DragLeave = null;
      DragOver = null;
    }

    public IResourceManager ResourceManager { get; private set; }

    public string Name
    {
      get { return c_name; }
    }

    public void OnBeforeLoad (HtmlDocumentHandle document)
    {
    }

    public void OnBeforeUnload (HtmlDocumentHandle document)
    {
    }

    public bool CheckRaiseCondition (Condition argument)
    {
      return argument.Criteria.elementID == _elementUnderCursor.Value.ID && argument.Document.Equals (_elementUnderCursor.Key);
    }

    public void RegisterEvents (IEventHost eventHost)
    {
      ArgumentUtility.CheckNotNull ("eventHost", eventHost);

      eventHost.RegisterEvent (this, ref DragDrop, c_dragDropEventName);
      eventHost.RegisterEvent (this, ref DragEnter, c_dragEnterEventName);
      eventHost.RegisterEvent (this, ref DragLeave, c_dragLeaveEventName);
      eventHost.RegisterEvent (this, ref DragOver, c_dragOverEventName);
    }

    public void UnregisterEvents (IEventHost eventHost)
    {
      ArgumentUtility.CheckNotNull ("eventHost", eventHost);

      eventHost.UnregisterEvent (this, ref DragDrop, c_dragDropEventName);
      eventHost.UnregisterEvent (this, ref DragEnter, c_dragEnterEventName);
      eventHost.UnregisterEvent (this, ref DragLeave, c_dragLeaveEventName);
      eventHost.UnregisterEvent (this, ref DragOver, c_dragOverEventName);
    }

    public void AddDragDropSource (IExtendedWebBrowser webBrowser)
    {
      webBrowser.DragDrop += OnDragDrop;
      webBrowser.DragEnter += OnDragEnter;
      webBrowser.DragLeave += OnDragLeave;
      webBrowser.DragOver += OnDragOver;
    }

    public void RemoveDragDropSource (IExtendedWebBrowser webBrowser)
    {
      webBrowser.DragDrop -= OnDragDrop;
      webBrowser.DragEnter -= OnDragEnter;
      webBrowser.DragLeave -= OnDragLeave;
      webBrowser.DragOver -= OnDragOver;
    }

    // when hovering things
    private void OnDragOver (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = e.Current;
      if (!_elementUnderCursor.Value.Equals (current))
        _elementUnderCursor = new KeyValuePair<HtmlDocumentHandle, HtmlElementData> (e.Document, e.Current);

      e.Droppable = IsDroppable (current);

      var args = new DragDropEventData();
      args.Names = _currentFilePaths.ToArray();

      if (DragOver != null)
        DragOver (this, c_dragOverEventName, args);
      var effect = GetMouseEffect (current);
      if (e.Droppable && effect > 0)
        e.Effect = (DragDropEffects) effect;
    }

    // when leaving the window
    private void OnDragLeave (object sender, EventArgs e)
    {
      if (DragLeave != null)
        DragLeave (
            this,
            c_dragLeaveEventName,
            new DragDropEventData
            {
                ResourceHandles = _currentResources.Select (r => r.ToString()).ToArray(),
                Names = _currentFilePaths.ToArray()
            });
      _currentFilePaths.Clear();
      _currentResources.Clear();
    }

    // when entering the window
    private void OnDragEnter (object sender, ExtendedDragEventHandlerArgs e)
    {
      _enterElement = new KeyValuePair<HtmlDocumentHandle, HtmlElementData> (e.Document, e.Current);
      _elementUnderCursor = _enterElement;

      var args = new DragDropEventData();
      if (e.Data != null)
      {
        _currentFilePaths = ((string[]) e.Data.GetData (DataFormats.FileDrop)).ToList();

        _currentResources = ResourceManager.AddResources (
            _currentFilePaths.Select (
                p => (File.GetAttributes (p) & FileAttributes.Directory) == FileAttributes.Directory
                         ? new DirectoryInfo (p) as FileSystemInfo
                         : new FileInfo (p) as FileSystemInfo
                ).ToArray()).ToList();
      }
      args.Names = _currentFilePaths.ToArray();
      if (DragEnter != null)
        DragEnter (this, c_dragEnterEventName, args);
    }

    private void OnDragDrop (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = e.Current;
      e.Droppable = IsDroppable (current);

      if (DragDrop != null && e.Droppable)
      {
        var args = new DragDropEventData
                   {
                       ResourceHandles = _currentResources.Select (r => r.ToString()).ToArray(),
                       Names = _currentFilePaths.ToArray()
                   };
        DragDrop (this, c_dragDropEventName, args);
      }
    }

    private bool IsDroppable (HtmlElementData element)
    {
      var result = false;
      string value;

      if (element != null && element.Attributes.TryGetValue (c_droppableAttributeName, out value))
        Boolean.TryParse (value, out result);

      return result && GetMouseEffect (element) != 0;
    }

    private int GetMouseEffect (HtmlElementData element)
    {
      string value;
      var result = 0;
      if (element != null && element.Attributes.TryGetValue (c_dropconditionAttributeName, out value))
        Int32.TryParse (value, out result);
      return result % c_dragdropEffectCount;
    }
  }
}