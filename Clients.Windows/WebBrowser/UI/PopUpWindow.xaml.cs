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
using DesktopGap.Utilities;
using DesktopGap.WebBrowser;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for PopUpWindow.xaml
  /// </summary>
  public sealed partial class PopUpWindow : IWebBrowserView
  {
    private readonly WebBrowserHost _browserHost;

    public PopUpWindow (WebBrowserHost browserHost)
    {
      ArgumentUtility.CheckNotNull ("browserHost", browserHost);

      _browserHost = browserHost;
      InitializeComponent();
      Content = _browserHost;


      //_browserHost.WebBrowser.

      _browserHost.WebBrowser.WindowSetWidth += (s, w) => Width = w;
      _browserHost.WebBrowser.WindowSetHeight += (s, h) => Height = h;
      _browserHost.WebBrowser.WindowSetLeft += (s, l) => Left = l;
      _browserHost.WebBrowser.WindowSetTop += (s, t) => Top = t;
    }


    public IExtendedWebBrowser WebBrowser
    {
      get { return _browserHost.WebBrowser; }
    }



    public void OnBeforeNavigate (object parent, NavigationEventArgs args)
    {
      ArgumentUtility.CheckNotNull ("args", args);
      ArgumentUtility.CheckNotNull ("parent", parent);


      switch (args.StartMode)
      {
        case BrowserWindowStartMode.Modal:
          ShowDialog();
          break;
        
        default:
          Show();
          break;
      }
    }


    
  }
}