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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DesktopGap.Clients.Windows.WebBrowser.Util;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Components
{
  public class TabHeaderViewModel : INotifyPropertyChanged
  {
    private Visibility _closeButtonVisibility;
    private string _text;
    private Brush _backgroundBrush;
    private BitmapImage _icon;

    public TabHeaderViewModel (Visibility closeButtonVisibility, BitmapImage icon, string text, Brush backgroundBrush)
    {
      ArgumentUtility.CheckNotNull ("icon", icon);
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("backgroundBrush", backgroundBrush);

      CloseButtonVisibility = closeButtonVisibility;
      Icon = icon;
      Text = text;
      BackgroundBrush = backgroundBrush;
      CloseCommand = new RelayCommand (o => true, OnClose);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler CloseClicked;

    public ICommand CloseCommand { get; private set; }

    public Visibility CloseButtonVisibility
    {
      get { return _closeButtonVisibility; }
      set
      {
        _closeButtonVisibility = value;
        OnPropertyChanged ("CloseButtonVisibility");
      }
    }

    public Brush BackgroundBrush
    {
      get { return _backgroundBrush; }
      set
      {
        _backgroundBrush = value;
        OnPropertyChanged ("BackgroundBrush");
      }
    }

    public String Text
    {
      get { return _text; }
      set
      {
        _text = value ?? String.Empty;
        OnPropertyChanged ("Text");
      }
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

    public bool IsCloseable
    {
      get { return CloseButtonVisibility == Visibility.Visible; }
      set { CloseButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }

    protected virtual void OnPropertyChanged (string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler (this, new PropertyChangedEventArgs (propertyName));
    }

    private void OnClose (object parameter)
    {
      if (CloseClicked != null)
        CloseClicked (this, EventArgs.Empty);
    }
  }
}