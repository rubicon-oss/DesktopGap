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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace DesktopGap.Clients.Windows
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class Browser : Window
  {
    private sealed class CloseableTabHeader : StackPanel
    {
      private readonly Label _lbl = new Label();
      private readonly Button _btn = new Button();

      public EventHandler Clicked;

      public String Text
      {
        get { return (string) _lbl.Content; }
        set { _lbl.Content = value ?? String.Empty; }
      }

      public CloseableTabHeader (string headerText)
          : base()
      {
        Orientation = Orientation.Horizontal;

        _btn.Content = "X";
        _btn.Click += (s, e) => Clicked (s, e);
        _lbl.Content = headerText;

        Children.Add (_lbl);
        Children.Add (_btn);
      }
    }

    private sealed class BrowserTabItem : TabItem
    {
      public BrowserTabItem (TabControl parent)
          : base()
      {
        var host =
            new WindowsFormsHost();
        var extendedWebBrowser = new ExtendedTridentWebBrowser();
        extendedWebBrowser.DocumentCompleted += (s, e) =>
                                                {
                                                  var x = new CloseableTabHeader (extendedWebBrowser.Title);
                                                  x.Clicked += (sx, ex) => parent.Items.Remove (this);
                                                  Header = x;
                                                };
        host.Child = extendedWebBrowser;
        this.AddChild (host);
      }
    }

    public Browser ()
    {
      InitializeComponent();
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      _tabControl.Items.Add (new BrowserTabItem (_tabControl));
    }
  }
}