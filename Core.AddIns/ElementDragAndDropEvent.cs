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
using System.Diagnostics;
using System.Windows.Forms;
using DesktopGap.AddIns.Events;

namespace DesktopGap.AddIns
{
  [PartCreationPolicy (CreationPolicy.NonShared)]
  public class ElementDragAndDropEvent : ExternalEventBase
  {
    private const string c_name = "ElementDragAndDrop";

    public override string Name
    {
      get { return c_name; }
    }

    public event ScriptEvent ItemDropped;

    public override void UnregisterEvents (IEventHost eventHost)
    {
      eventHost.UnregisterEvent (this, ref ItemDropped, "ItemDropped");
    }

    public override void OnBeforeLoad ()
    {
      //SystemEventHub.DragDrop += (s, e) =>
      //                           {
      //                             if (Boolean.Parse (((HtmlElement) e.Current).GetAttribute ("droptarget")))
      //                             {
      //                               e.Effect = DragDropEffects.Copy;
      //                               var filePaths = (string[]) (e.Data.GetData (DataFormats.FileDrop));

      //                               ItemDropped (
      //                                   this,
      //                                   "ItemDropped",
      //                                   new FileScriptArgs (filePaths[0]));
      //                             }
      //                             e.Effect = DragDropEffects.None;
      //                           };
    }


    public override bool CheckArgument (EventArgument argument)
    {
      try
      {
        return argument.Criteria.elementId == "realDropTarget";
      }catch
      {
        Debug.WriteLine ("something is wrong here");
        return false;
      }
    }

    public override void RegisterEvents (IEventHost eventHost)
    {
      eventHost.RegisterEvent (this, ref ItemDropped, "ItemDropped");
    }


    public override void OnBeforeUnload ()
    {
    }

    public override void Dispose ()

    {
    }
  }
}