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
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Arguments;
using DesktopGap.AddIns.Services;
using DesktopGap.Utilities;

namespace DesktopGap.AddIns.Contrib
{
  internal class MessageData : JsonData
  {
    public string Message { get; set; }
  }

  internal class MessageEventArgs : EventArgs
  {
    public MessageEventArgs (string message)
    {
      ArgumentUtility.CheckNotNull ("message", message);

      Message = message;
    }

    public string Message { get; private set; }
  }

  internal static class MessageHandler
  {
    public static event EventHandler<MessageEventArgs> MessageReceived;

    public static void Publish (string message)
    {
      MessageReceived (null, new MessageEventArgs (message));
    }
  }

  [ComVisible (true)]
  [PartCreationPolicy (CreationPolicy.Shared)]
  public class MessageEvent : ExternalEventBase
  {
    private event ScriptEvent MessageReceived;

    public MessageEvent ()
    {
      MessageHandler.MessageReceived += (s, e) =>
                                        {
                                          if (MessageReceived == null)
                                            return;

                                          MessageReceived (this, "MessageReceived", new MessageData { Message = e.Message });
                                        };
    }

    public override string Name
    {
      get { return "com.example.MessageEvent"; }
    }

    public override void Dispose ()
    {
    }

    public override bool CheckRaiseCondition (Condition argument)
    {
      return true;
    }

    public override void RegisterEvents (IEventHost eventHost)
    {
      eventHost.RegisterEvent (this, ref MessageReceived, "MessageReceived");
    }

    public override void UnregisterEvents (IEventHost eventHost)
    {
      eventHost.UnregisterEvent (this, ref MessageReceived, "MessageReceived");
    }
  }

  [ComVisible (true)]
  [PartCreationPolicy (CreationPolicy.Shared)]
  public class MessageService : ExternalServiceBase
  {
    public MessageService ()
    {
    }

    public override string Name
    {
      get { return "com.example.MessageService"; }
    }

    public override void Dispose ()
    {
    }

    public void Publish (string message)
    {
      ArgumentUtility.CheckNotNull ("message", message);
      
      MessageHandler.Publish (message);
    }
  }
}