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
using System.Windows;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Components
{
  /// <summary>
  /// Interaction logic for CloseableTabHeader.xaml
  /// </summary>
  public partial class CloseableTabHeader
  {
    private bool _isCloseable;
    private const string c_defaultCloseButtonContent = "X";

    public event EventHandler TabClose;

    public String Text
    {
      get { return (string) _label.Content; }
      set { _label.Content = value ?? String.Empty; }
    }

    public bool IsCloseable
    {
      get { return _isCloseable; }
      set
      {
        _isCloseable = value;
        if (_closeButton != null)
          _closeButton.Visibility = _isCloseable ? Visibility.Visible : Visibility.Hidden;
      }
    }

    public CloseableTabHeader (string headerText, bool isCloseable = false, object closeButtonContent = null)
    {
      ArgumentUtility.CheckNotNull ("headerText", headerText);
      InitializeComponent();
      IsCloseable = isCloseable;

      if (closeButtonContent == null)
        closeButtonContent = c_defaultCloseButtonContent;

      _closeButton.Content = closeButtonContent;
      _closeButton.Click += (s, e) =>
                            {
                              if (IsCloseable && TabClose != null)
                                TabClose (s, e);
                            };

      _label.Content = headerText;
    }
  }
}