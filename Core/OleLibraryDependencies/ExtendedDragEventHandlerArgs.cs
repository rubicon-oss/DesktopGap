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
using System.Windows.Forms;
using DesktopGap.WebBrowser;

namespace DesktopGap.OleLibraryDependencies
{
  /// <summary>
  /// Extends the DragEventArgs to add the "Handled" flag
  /// </summary>
  public class ExtendedDragEventHandlerArgs : DragEventArgs
  {
    public HtmlDocumentHandle Document { get; set; }

    /// <summary>
    /// Wether the Drag Drop Event was handled or not
    /// If true the underlying control is prohibited from overiding the defined DragDropEffects
    /// If false, the underlying control has full control over the DragDropEffects (modifications won't be applied)
    /// </summary>
    public bool Handled { get; set; }

    public HtmlElementData Current { get; set; }

    public bool Droppable { get; set; }


    public ExtendedDragEventHandlerArgs (
        IDataObject data,
        int keyState,
        int x,
        int y,
        DragDropEffects allowedEffect,
        DragDropEffects effect,
        HtmlDocumentHandle document = default (HtmlDocumentHandle))
        : base (data, keyState, x, y, allowedEffect, effect)
    {
      Document = document;
    }
  }
}