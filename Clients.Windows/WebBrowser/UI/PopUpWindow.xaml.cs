﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
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
using System.Windows;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for PopUpWindow.xaml
  /// </summary>
  public partial class PopUpWindow : IWebBrowserView
  {
    private readonly WebBrowserHost _browserHost;


    public PopUpWindow (TridentWebBrowser webBrowser, Guid identifier)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      InitializeComponent();

      _browserHost = new WebBrowserHost (webBrowser);
      Content = _browserHost;

      Width = _browserHost.WebBrowser.Width;
      _browserHost.WebBrowser.WindowSetWidth += (s, w) => Width = w;

      Height = _browserHost.WebBrowser.Height;
      _browserHost.WebBrowser.WindowSetHeight += (s, h) => Height = h;

      Left = _browserHost.WebBrowser.Left;
      _browserHost.WebBrowser.WindowSetLeft += (s, l) => Left = l;

      Top = _browserHost.WebBrowser.Top;
      _browserHost.WebBrowser.WindowSetTop += (s, t) => Top = t;

      ResizeMode = _browserHost.WebBrowser.IsResizable ? ResizeMode.CanResize : ResizeMode.NoResize;
      _browserHost.WebBrowser.WindowSetResizable += (s, r) => ResizeMode = r ? ResizeMode.CanResize : ResizeMode.NoResize;

      _browserHost.WebBrowser.DocumentTitleChanged += (s, e) => Title = _browserHost.WebBrowser.Title;

      Visibility = Visibility.Hidden;
      Identifier = identifier;
    }


    public void Dispose ()
    {
      _browserHost.Dispose();
    }


    public IExtendedWebBrowser WebBrowser
    {
      get { return _browserHost.WebBrowser; }
    }

    public Guid Identifier { get; private set; }


    public void Show (BrowserWindowStartMode startMode)
    {
      switch (startMode)
      {
        case BrowserWindowStartMode.Active:
          Show();
          break;
        case BrowserWindowStartMode.Modal:
          ShowDialog();
          break;
        default:
          throw new InvalidOperationException (string.Format ("Start mode '{0}' is not supported for PopUpWindow.", startMode));
      }
    }
  }
}