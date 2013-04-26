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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.WebBrowser.Util
{
  public class FrameIterator
  {
    private readonly HtmlDocument _htmlDocument;

    public FrameIterator (HtmlDocument htmlDocument)
    {
      ArgumentUtility.CheckNotNull ("htmlDocument", htmlDocument);

      _htmlDocument = htmlDocument;
    }

    private IEnumerable<HtmlWindow> WalkFrame (HtmlWindow frame)
    {
      if (frame == null) //|| frame.Url == null || frame.Url.AbsoluteUri == c_blankSite)
        yield break;

      yield return frame;

      if (frame.Frames != null)
        foreach (var subframe in frame.Frames.Cast<HtmlWindow>().SelectMany (WalkFrame))
          yield return subframe;
    }

    public IEnumerable<HtmlWindow> GetFrames ()
    {
      return WalkFrame (_htmlDocument.Window);
    }
  }
}