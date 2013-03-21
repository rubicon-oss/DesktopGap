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
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;

namespace DesktopGap.AddIns
{

  [PartCreationPolicy (CreationPolicy.NonShared)]

  public class ElementDragAndDropEvent : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent2 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent3 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent4 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent5 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent6 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent7 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent8 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent9 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }


  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent10 : IExternalEvent
  {
    private const string c_name = "ElementDragAndDrop";

    public string Name
    {
      get { return c_name; }
    }

    private event ScriptEvent ItemDropped;

    public void OnBeforeLoad ()
    {
      SystemEventHub.DragDrop += (e) =>
                                 {
                                   var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));
                                   ItemDropped (
                                       this.GetType().GetEvent ("ItemDropped"),
                                       new ScriptEventArgs() { ScriptArgs = new FileScriptArgs (filePaths[0]) });
                                 };
    }


    public void RegisterEvents (IEventManager eventManager)
    {
      eventManager.RegisterEvent (this, ref ItemDropped);
    }


    public void OnBeforeUnload ()
    {
    }
  }
}