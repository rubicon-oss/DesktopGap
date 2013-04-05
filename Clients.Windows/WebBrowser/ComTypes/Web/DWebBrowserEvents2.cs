// This file is part of DesktopGap (desktopgap.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Runtime.InteropServices;
using System.Security;

namespace DesktopGap.Clients.Windows.WebBrowser.ComTypes.Web
{

  #region interface definition

  [ComImport]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType (ComInterfaceType.InterfaceIsIDispatch)]
  [Guid ("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
  public interface DWebBrowserEvents2
  {
    [DispId (0x66)]
    void StatusTextChange ([MarshalAs (UnmanagedType.BStr)] string Text);

    [DispId (0x6c)]
    void ProgressChange (int Progress, int ProgressMax);

    [DispId (0x69)]
    void CommandStateChange (int Command, [MarshalAs (UnmanagedType.VariantBool)] bool Enable);

    [DispId (0x6a)]
    void DownloadBegin ();

    [DispId (0x68)]
    void DownloadComplete ();

    [DispId (0x71)]
    void TitleChange ([MarshalAs (UnmanagedType.BStr)] string Text);

    [DispId (0x70)]
    void PropertyChange ([MarshalAs (UnmanagedType.BStr)] string szProperty);

    [DispId (250)]
    void BeforeNavigate2 (
        [MarshalAs (UnmanagedType.IDispatch)] object pDisp,
        [In] ref object URL,
        [In] ref object Flags,
        [In] ref object TargetFrameName,
        [In] ref object PostData,
        [In] ref object Headers,
        [In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel);

    [DispId (0xfb)]
    void NewWindow2 (
        [In] [Out] [MarshalAs (UnmanagedType.IDispatch)] ref object ppDisp, [In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel);

    [DispId (0xfc)]
    void NavigateComplete2 ([MarshalAs (UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

    [DispId (0x103)]
    void DocumentComplete ([MarshalAs (UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

    [DispId (0xfd)]
    void OnQuit ();

    [DispId (0xfe)]
    void OnVisible ([MarshalAs (UnmanagedType.VariantBool)] bool Visible);

    [DispId (0xff)]
    void OnToolBar ([MarshalAs (UnmanagedType.VariantBool)] bool ToolBar);

    [DispId (0x100)]
    void OnMenuBar ([MarshalAs (UnmanagedType.VariantBool)] bool MenuBar);

    [DispId (0x101)]
    void OnStatusBar ([MarshalAs (UnmanagedType.VariantBool)] bool StatusBar);

    [DispId (0x102)]
    void OnFullScreen ([MarshalAs (UnmanagedType.VariantBool)] bool FullScreen);

    [DispId (260)]
    void OnTheaterMode ([MarshalAs (UnmanagedType.VariantBool)] bool TheaterMode);

    [DispId (0x106)]
    void WindowSetResizable ([MarshalAs (UnmanagedType.VariantBool)] bool Resizable);

    [DispId (0x108)]
    void WindowSetLeft (int Left);

    [DispId (0x109)]
    void WindowSetTop (int Top);

    [DispId (0x10a)]
    void WindowSetWidth (int Width);

    [DispId (0x10b)]
    void WindowSetHeight (int Height);

    [DispId (0x107)]
    void WindowClosing (
        [MarshalAs (UnmanagedType.VariantBool)] bool IsChildWindow, [In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel);

    [DispId (0x10c)]
    void ClientToHostWindow ([In] [Out] ref int CX, [In] [Out] ref int CY);

    [DispId (0x10d)]
    void SetSecureLockIcon (int SecureLockIcon);

    [DispId (270)]
    void FileDownload ([In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel);

    [DispId (0x10f)]
    void NavigateError (
        [MarshalAs (UnmanagedType.IDispatch)] object pDisp,
        [In] ref object URL,
        [In] ref object Frame,
        [In] ref object StatusCode,
        [In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel);

    [DispId (0xe1)]
    void PrintTemplateInstantiation ([MarshalAs (UnmanagedType.IDispatch)] object pDisp);

    [DispId (0xe2)]
    void PrintTemplateTeardown ([MarshalAs (UnmanagedType.IDispatch)] object pDisp);

    [DispId (0xe3)]
    void UpdatePageStatus ([MarshalAs (UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);

    [DispId (0x110)]
    void PrivacyImpactedStateChange ([MarshalAs (UnmanagedType.VariantBool)] bool bImpacted);

    [DispId (0x111)]
    void NewWindow3 (
        [In] [Out] [MarshalAs (UnmanagedType.IDispatch)] ref object ppDisp,
        [In] [Out] [MarshalAs (UnmanagedType.VariantBool)] ref bool Cancel,
        uint dwFlags,
        [MarshalAs (UnmanagedType.BStr)] string bstrUrlContext,
        [MarshalAs (UnmanagedType.BStr)] string bstrUrl);
  }

  #endregion




}