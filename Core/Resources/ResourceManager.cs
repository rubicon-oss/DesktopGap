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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesktopGap.Utilities;

namespace DesktopGap.Resources
{
  public class ResourceManager : IResourceManager
  {
    private readonly ConcurrentDictionary<ResourceHandle, FileSystemInfo> _resources = new ConcurrentDictionary<ResourceHandle, FileSystemInfo>();

    public FileSystemInfo GetResource (ResourceHandle handle)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);

      FileSystemInfo fileSystemInfo;

      if (!_resources.TryGetValue (handle, out fileSystemInfo))
        throw new InvalidOperationException (string.Format ("Resource {0} not found.", handle));

      return fileSystemInfo;
    }

    public int ResourceCount
    {
      get { return _resources.Count; }
    }


    public ResourceHandle[] AddResources (FileSystemInfo[] paths)
    {
      ArgumentUtility.CheckNotNull ("paths", paths);

      var handles = new List<ResourceHandle>();
      try
      {
        handles.AddRange (paths.Select (AddResource));
        return handles.ToArray();
      }
      catch (InvalidOperationException)
      {
        foreach (var resourceHandle in handles) // roll back
          RemoveResource (resourceHandle);

        throw;
      }
    }


    public ResourceHandle AddResource (FileSystemInfo path)
    {
      ArgumentUtility.CheckNotNull ("path", path);
      if (_resources.Values.Any (r => r.FullName.Equals (path.FullName)))
        throw new InvalidOperationException (string.Format ("Resource at '{0}' already registered.", path.FullName));

      var handle = new ResourceHandle (Guid.NewGuid());
      _resources[handle] = path;

      return handle;
    }


    public void RemoveResource (ResourceHandle handle)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);

      FileSystemInfo value;
      _resources.TryRemove (handle, out value);
    }
  }
}