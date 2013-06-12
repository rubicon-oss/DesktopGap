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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Components
{
  /// <summary>
  /// Interaction logic for CloseableTabHeader.xaml
  /// </summary>
  public partial class CloseableTabHeader : INotifyPropertyChanged
  {
    private string _text;
    private Visibility _visibility = Visibility.Visible;
    private BitmapImage _icon;

    public CloseableTabHeader (string headerText, BitmapImage icon, bool isCloseable = false)
    {
      ArgumentUtility.CheckNotNull ("headerText", headerText);
      ArgumentUtility.CheckNotNull ("icon", icon);

      InitializeComponent();
      IsCloseable = isCloseable;
      Text = headerText;
      DataContext = this;
      _icon = icon;
    }

    public event EventHandler TabClose;
    public event PropertyChangedEventHandler PropertyChanged;


    public String Text
    {
      get { return _text; }
      set
      {
        OnPropertyChanged ("Text");
        _text = value ?? String.Empty;
      }
    }

    public void ShowCloseButton ()
    {
      IsCloseable = true;
    }


    public void HideCloseButton ()
    {
      IsCloseable = false;
    }

    public bool IsCloseable
    {
      get { return CloseButtonVisibility == Visibility.Visible; }
      private set { CloseButtonVisibility = value ? Visibility.Visible : Visibility.Hidden; }
    }

    public BitmapImage Icon
    {
      get { return _icon; }
      set
      {
        _icon = value;
        OnPropertyChanged ("Icon");
      }
    }

    public Visibility CloseButtonVisibility
    {
      get { return _visibility; }
      set
      {
        _visibility = value;
        OnPropertyChanged ("CloseButtonVisibility");
      }
    }


    private void OnPropertyChanged (string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler (this, new PropertyChangedEventArgs (propertyName));
    }

    private void CloseButton_Click (object sender, RoutedEventArgs eventArgs)
    {
      if (IsCloseable && TabClose != null)
        TabClose (sender, eventArgs);
    }
  }
}