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
using System.IO;
using DesktopGap.Utilities;

namespace DesktopGap.UnitTests.Utilities
{
  public class TempFile : IDisposable
  {
    private string _fileName;
    private bool Disposed { get; set; }

    public TempFile ()
    {
      _fileName = Path.GetTempFileName();
    }


    public string FileName
    {
      get
      {
        //TODO: Use AssertNotDisposed once new DisposableBase is commited
        if (Disposed)
          throw new InvalidOperationException ("Object disposed.");
        return _fileName;
      }
    }

    public void WriteStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);

      using (var streamReader = new StreamReader (stream))
      {
        using (var streamWriter = new StreamWriter (_fileName))
        {
          while (!streamReader.EndOfStream)
          {
            var buffer = new char[100];
            streamWriter.Write (buffer, 0, streamReader.Read (buffer, 0, buffer.Length));
          }
        }
      }
    }

    public void WriteAllBytes (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      File.WriteAllBytes (_fileName, bytes);
    }

    public void WriteAllText (string text)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      File.WriteAllText (_fileName, text);
    }

    public long Length
    {
      get
      {
        var fileInfo = new FileInfo (_fileName);
        return fileInfo.Length;
      }
    }

    public void Dispose ()
    {
      if (_fileName == null || !File.Exists (_fileName))
        return;

      File.Delete (_fileName);
      _fileName = null;
      Disposed = true;
    }
  }
}