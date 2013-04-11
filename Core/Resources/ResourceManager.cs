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
using System.Linq;
using DesktopGap.Utilities;

namespace DesktopGap.Resources
{
  public class ResourceManager : IResourceManager
  {
    private struct DirectoryBookmark
    {
      public ResourceHandle Handle;
      public DirectoryInfo Directory;
    }

    private const string c_dataFolderName = "DesktopGap";

    private readonly ConcurrentDictionary<ResourceHandle, FileSystemInfo> _resources = new ConcurrentDictionary<ResourceHandle, FileSystemInfo>();

    private readonly DirectoryBookmark _data;
    private readonly DirectoryBookmark _temp;

    public ResourceManager ()
    {
      var dataFolder = new DirectoryInfo (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), c_dataFolderName));
      if (!dataFolder.Exists)
        dataFolder.Create();

      _data = new DirectoryBookmark { Directory = dataFolder, Handle = new ResourceHandle (Guid.NewGuid()) };

      _resources[_data.Handle] = _data.Directory;

      var tempFolder = new DirectoryInfo (Path.Combine (Path.GetTempPath(), c_dataFolderName));
      if (tempFolder.Exists)
      {
        tempFolder.Delete (true);
        tempFolder.Create();
      }

      _temp = new DirectoryBookmark { Directory = tempFolder, Handle = new ResourceHandle (Guid.NewGuid()) };

      _resources[_temp.Handle] = _temp.Directory;
    }

    public FileSystemInfo GetResource (ResourceHandle handle)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);

      FileSystemInfo fileSystemInfo;
      if (!_resources.TryGetValue (handle, out fileSystemInfo))
        throw new InvalidOperationException (string.Format ("Resource {0} not found", handle));

      return fileSystemInfo;
    }

    public ResourceHandle GetTempDirectory ()
    {
      return _temp.Handle;
    }

    public ResourceHandle GetDataDirectory ()
    {
      return _data.Handle;
    }

    public ResourceHandle[] AddResources (string[] paths)
    {
      return paths.Select (AddResource).ToArray();
    }

    public ResourceHandle AddResource (string path)
    {
      var handle = new ResourceHandle (Guid.NewGuid());

      var attr = File.GetAttributes (path);

      var destination = Path.Combine (_temp.Directory.FullName, JoinExtension (handle, Path.GetExtension (path)));
      File.Copy (path, destination);
      
      FileSystemInfo resource;
      if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        resource = new DirectoryInfo (path);
      else
        resource = new FileInfo (path);

      _resources[handle] = resource;
      return handle;
    }

    private string JoinExtension (ResourceHandle handle, string extension)
    {
      if (string.IsNullOrEmpty (extension))
        return String.Format ("{0}.{1}", handle, extension);
      else
        return handle.ToString();
    }

    public void RemoveResource (ResourceHandle handle)
    {
      FileSystemInfo value;
      _resources.TryRemove (handle, out value);

      value.Delete();
    }

    public void Dispose ()
    {
      foreach (var resource in _resources)
        resource.Value.Delete();
      _resources.Clear();
    }
  }
}