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
using System.Collections.Concurrent;
using System.IO;
using DesktopGap.Utilities;

namespace DesktopGap.Resources
{
  public class ResourceManager : IResourceManager
  {
    private struct DirectoryBookmark
    {
      public Guid Guid;
      public DirectoryInfo Directory;
    }

    private const string c_dataFolderName = "DesktopGap";
    private readonly ConcurrentDictionary<Guid, FileSystemInfo> _resources = new ConcurrentDictionary<Guid, FileSystemInfo>();

    private readonly DirectoryBookmark _data;
    private readonly DirectoryBookmark _temp;

    public ResourceManager ()
    {
      var dataFolder = new DirectoryInfo (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), c_dataFolderName));
      if (!dataFolder.Exists)
        dataFolder.Create();

      _data = new DirectoryBookmark() { Directory = dataFolder, Guid = Guid.NewGuid() };

      _resources[_data.Guid] = _data.Directory;

      var tempFolder = new DirectoryInfo (Path.Combine (Path.GetTempPath(), c_dataFolderName));
      if (tempFolder.Exists)
      {
        tempFolder.Delete (true);
        tempFolder.Create();
      }

      _temp = new DirectoryBookmark() { Directory = tempFolder, Guid = Guid.NewGuid() };
      
      _resources[_temp.Guid] = _temp.Directory;

    }

    public string GetFullPath (Guid guid)
    {
      ArgumentUtility.CheckNotNull ("guid", guid);

      var resource = GetResource (guid);
      return resource.FullName;
    }

    public FileSystemInfo GetResource (Guid guid)
    {
      ArgumentUtility.CheckNotNull ("guid", guid);

      FileSystemInfo fileSystemInfo;
      if (!_resources.TryGetValue (guid, out fileSystemInfo))
        throw new InvalidOperationException (string.Format ("Resource {0} not found", guid));

      return fileSystemInfo;
    }

    public Guid GetTempDirectory ()
    {
      return _temp.Guid;
    }

    public Guid GetDataDirectory()
    {
      return _data.Guid;
    }
  }

}