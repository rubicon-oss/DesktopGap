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
using System.Windows.Forms;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.OleLibraryDependencies;
using DesktopGap.Resources;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;

namespace DesktopGap.AddIns.Events.System
{
  internal class DragAndDropAddIn : IEventAddIn
  {
    private class DragDropEventData : JsonData
    {
      public ResourceHandle[] ResourceHandle { get; set; }
      public string Test { get; set; }
    }

    private class RingBuffer<TElement>
    {
      private readonly TElement[] _buffer;
      private int _current;

      public RingBuffer (int size)
      {
        _buffer = new TElement[size];
      }

      public void Push (TElement item)
      {
        Next();
        _buffer[_current] = item;
      }

      public TElement Head ()
      {
        return _buffer[_current];
      }

      private void Next ()
      {
        _current = (_current + 1) % _buffer.Length;
      }
    }

    private const string c_dragEnterEventName = "DragEnter";
    private const string c_dragDropEventName = "DragDrop";
    private const string c_dragLeaveEventName = "DragLeave";
    private const string c_dragOverEventName = "DragOver";
    private const string c_doppableAttributeName = "droptarget";

    private const string c_name = "DragAndDrop";

    private const int c_eventBufferSize = 10;

    public event ScriptEvent DragDrop;
    public event ScriptEvent DragEnter;
    public event ScriptEvent DragLeave;
    public event ScriptEvent DragOver;

    private readonly RingBuffer<KeyValuePair<HtmlDocumentHandle, HtmlElement>> _elementBuffer =
        new RingBuffer<KeyValuePair<HtmlDocumentHandle, HtmlElement>> (c_eventBufferSize);


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
      var lastElement = _elementBuffer.Head();

      return argument.Criteria.elementID == lastElement.Value.Id && argument.Document.Equals (lastElement.Key);
    }

    public void RegisterEvents (IEventHost eventHost)
    {
      eventHost.RegisterEvent (this, ref DragDrop, c_dragDropEventName);
      eventHost.RegisterEvent (this, ref DragEnter, c_dragEnterEventName);
      eventHost.RegisterEvent (this, ref DragLeave, c_dragLeaveEventName);
      eventHost.RegisterEvent (this, ref DragOver, c_dragOverEventName);
    }

    public void UnregisterEvents (IEventHost eventHost)
    {
      eventHost.UnregisterEvent (this, ref DragDrop, c_dragDropEventName);
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

    private void OnDragOver (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = (HtmlElement) e.Current;
      _elementBuffer.Push (new KeyValuePair<HtmlDocumentHandle, HtmlElement> (e.Document, (HtmlElement) e.Current));

      e.Droppable = IsDroppable (current);
      //if (e.Droppable)
      //  Debug.WriteLine ("yep");
      if (DragOver != null)
        DragOver (this, c_dragOverEventName, new DragDropEventData());
    }

    private void OnDragLeave (object sender, EventArgs e)
    {
      if (DragLeave != null)
        DragLeave (this, c_dragLeaveEventName, new DragDropEventData { ResourceHandle = new ResourceHandle[0] });
    }

    private void OnDragEnter (object sender, ExtendedDragEventHandlerArgs e)
    {
      //_elementBuffer.Push (new KeyValuePair<HtmlDocumentHandle, HtmlElement> (e.Document, (HtmlElement) e.Current));

      if (DragEnter != null)
        DragEnter (this, c_dragEnterEventName, new DragDropEventData { ResourceHandle = new ResourceHandle[] { } });
    }

    private void OnDragDrop (object sender, ExtendedDragEventHandlerArgs e)
    {
      var current = (HtmlElement) e.Current;

      e.Droppable = IsDroppable (current);

      _elementBuffer.Push (new KeyValuePair<HtmlDocumentHandle, HtmlElement> (e.Document, current));


      if (DragDrop != null && e.Droppable)
      {
        //var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
        //var resources = ResourceManager.AddResources (filePaths);
        var resources = new[] { new ResourceHandle (Guid.NewGuid()) };
        var args = new DragDropEventData { ResourceHandle = resources };
        DragDrop (this, c_dragDropEventName, args);

      }
    }

    private bool IsDroppable (HtmlElement element)
    {
      bool result;
      Boolean.TryParse (element.GetAttribute (c_doppableAttributeName), out result);
      return result;
    }
  }
}