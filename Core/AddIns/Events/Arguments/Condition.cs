// This file is part of DesktopGap (http://desktopgap.codeplex.com)
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
using Microsoft.CSharp.RuntimeBinder;

namespace DesktopGap.AddIns.Events.Arguments
{
  public class Condition
  {
    public string EventID { get; private set; }

    public HtmlDocumentHandle Document { get; private set; }
    public dynamic Criteria { get; private set; }

    public Condition (dynamic condition)
    {
      try
      {
        EventID = condition.EventID;
        Document = new HtmlDocumentHandle (Guid.Parse (condition.DocumentHandle));
        Criteria = condition.Criteria;
      }
      catch (RuntimeBinderException binderException)
      {
        throw new ArgumentException ("The provided object does not have the required properties 'EventID', 'DocumentHandle', and 'Criteria'.");
      }
    }
  }
}