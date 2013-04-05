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
using DesktopGap.Clients.Windows.OleLibraryDependencies;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes;
using DesktopGap.Clients.Windows.WebBrowser.ComTypes.UIHandler;
using DesktopGap.Clients.Windows.WebBrowser.Trident;
using DesktopGap.OleLibraryDependencies;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using IDropTarget = DesktopGap.Clients.Windows.OleLibraryDependencies.IDropTarget;

namespace DesktopGap.Clients.Windows
{
  public class DesktopGapDocumentUIHandler : DocHostUIHandlerBase, IDropTarget
  {
    private readonly TridentWebBrowser _extendedTridentWebBrowser;

    public DesktopGapDocumentUIHandler (TridentWebBrowser browser)
        : base (browser)
    {
      _extendedTridentWebBrowser = browser;
    }

    private NativeDragDropEffects ToNative (DragDropEffects dragDropEffects)
    {
      NativeDragDropEffects nativeDragDropEffects;
      if (!NativeDragDropEffects.TryParse (dragDropEffects.ToString(), true, out nativeDragDropEffects))
        throw new Exception ("Invalid effect");
      return nativeDragDropEffects;
    }

    private DragDropEffects ToWinForms (NativeDragDropEffects nativeDragDropEffects)
    {
      var dragDropEffects = DragDropEffects.None;
      if (!DragDropEffects.TryParse (nativeDragDropEffects.ToString(), true, out nativeDragDropEffects))
        throw new Exception ("Invalid effect");

      return dragDropEffects;
    }

    public override int GetDropTarget (IDropTarget dropTarget, out IDropTarget target)
    {
      target = this;
      return HResult.S_OK;
    }

    public int DragEnter (IDataObject pDataObj, uint grfKeyState, tagPOINT pt, ref uint pdwEffect)
    {
      System.Windows.Forms.IDataObject dataObject = null;
      if (pDataObj != null)
      {
        dataObject = new DataObject (pDataObj);
      }

      var dragEnterHandler = new ExtendedDragEventHandlerArgs (
          dataObject,
          (int) grfKeyState,
          pt.X,
          pt.Y,
          DragDropEffects.All,
          ToWinForms ((NativeDragDropEffects) pdwEffect));
      _extendedTridentWebBrowser.OnDragEnter (dragEnterHandler);

      if (dragEnterHandler.Handled)
      {
        pdwEffect = (uint) ToNative (dragEnterHandler.Effect);
      }

      return HResult.S_OK;
    }

    public int DragOver (uint grfKeyState, tagPOINT pt, ref uint pdwEffect)
    {
      var dragEnterHandler = new ExtendedDragEventHandlerArgs (
          null,
          (int) grfKeyState,
          pt.X,
          pt.Y,
          DragDropEffects.All,
          ToWinForms ((NativeDragDropEffects) pdwEffect));

      _extendedTridentWebBrowser.OnDragOver (dragEnterHandler);

      if (dragEnterHandler.Handled)
      {
        pdwEffect = (uint) ToNative (dragEnterHandler.Effect);
      }

      return HResult.S_OK;
    }

    int IDropTarget.DragLeave ()
    {
      _extendedTridentWebBrowser.OnDragLeave (new EventArgs());
      return HResult.S_OK;
    }

    public int Drop (IDataObject pDataObj, uint grfKeyState, tagPOINT pt, ref uint pdwEffect)
    {
      System.Windows.Forms.IDataObject dataObject = null;
      if (pDataObj != null)
      {
        dataObject = new DataObject (pDataObj);
      }

      var dragEnterHandler = new ExtendedDragEventHandlerArgs (
          dataObject,
          (int) grfKeyState,
          pt.X,
          pt.Y,
          DragDropEffects.All,
          ToWinForms ((NativeDragDropEffects) pdwEffect));

      _extendedTridentWebBrowser.OnDragDrop (dragEnterHandler);

      if (dragEnterHandler.Handled)
      {
        pdwEffect = (uint) ToNative (dragEnterHandler.Effect);
      }

      return HResult.S_OK;
    }


    public override int TranslateAccelerator (ref tagMSG msg, ref Guid group, uint nCmdID)
    {
      var keyEventArgs = new KeyEventArgs ((Keys) msg.wParam);
      _extendedTridentWebBrowser.OnBrowserKeyDown (_extendedTridentWebBrowser, keyEventArgs);

      return keyEventArgs.Handled ? HResult.S_OK : HResult.S_FALSE;
    }
  }
}