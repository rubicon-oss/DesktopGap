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
using System.Collections.ObjectModel;
using Remotion.Dms.Shared.Utilities;

namespace DesktopGap.Clients.Windows.DebugConsole
{
  public class DebugViewModel
  {
    public class DebugMessage
    {
      public DateTime Date { get; set; }
      public String Message { get; set; }
    }

    public DebugViewModel ()
    {
      DebugMessages = new ObservableCollection<DebugMessage>();
    }

    public ObservableCollection<DebugMessage> DebugMessages { get; set; }

    public void AddMessage (string message)
    {
      ArgumentUtility.CheckNotNull ("message", message);
      
      lock (this)
      {
        DebugMessages.Add (new DebugMessage { Date = DateTime.Now, Message = message });
      }
    }
  }
}