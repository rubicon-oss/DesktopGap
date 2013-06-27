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
using System.IO;
using System.Linq;
using DesktopGap.Resources;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class ResourceManagerTest
  {
    [Test]
    public void GetResource_GetByInvalidHandle_ShouldThrowInvalidOperation ()
    {
      var resourceManager = new ResourceManager();
      var resourceHandle = new ResourceHandle (Guid.NewGuid());
      Assert.That (
          () => resourceManager.GetResource (resourceHandle),
          Throws.InvalidOperationException.With.Message.EqualTo (string.Format ("Resource {0} not found.", resourceHandle)));
    }

    [Test]
    public void GetResource_GetByValidHandle_ShouldSucceed ()
    {
      var resourceManager = new ResourceManager();
      var resourceHandle = resourceManager.AddResource (new FileInfo ("c:/this/is/a/test"));
      Assert.That (() => resourceManager.GetResource (resourceHandle), Throws.Nothing);
    }

    [Test]
    public void AddResource_AddAResource_ShouldSucceed ()
    {
      var resourceManager = new ResourceManager();
      Assert.That (() => resourceManager.AddResource (new FileInfo ("c:/this/is/a/test")), Is.InstanceOf<ResourceHandle>());
      Assert.That (() => resourceManager.AddResource (new DirectoryInfo ("c:/this/is/a/test/dir")), Is.InstanceOf<ResourceHandle>());
    }

    [Test]
    public void AddResources_AddManyResources_ShouldSucceed ()
    {
      var resources = new[]
                      {
                          new FileInfo ("c:/this/is/a/test"),
                          (FileSystemInfo) new DirectoryInfo ("c:/this/is/a/test/dir"),
                          new FileInfo ("c:/this/is/another/test"),
                          new FileInfo ("c:/this/is/yet/another/test"),
                      };
      var resourceManager = new ResourceManager();
      Assert.That (() => resourceManager.AddResources (resources), Is.InstanceOf<ResourceHandle[]>());
      Assert.That (resourceManager.ResourceCount, Is.EqualTo (resources.Length));
    }

    [Test]
    public void AddResource_AddADuplicateResource_ShouldReturnHandle ()
    {
      var resourceManager = new ResourceManager();
      var fileInfo = new FileInfo ("c:/this/is/a/test");
      
      var handle = resourceManager.AddResource (fileInfo);
      var count = resourceManager.ResourceCount;

      Assert.That (resourceManager.AddResource (fileInfo), Is.EqualTo (handle));
      Assert.That (count, Is.EqualTo (resourceManager.ResourceCount));
    }

    [Test]
    public void AddResource_AddADuplicateResourcePath_ShouldReturnHandle ()
    {
      var resourceManager = new ResourceManager();
      var fileInfo = new FileInfo ("c:/this/is/a/test");
      var fileInfoDuplicate = new FileInfo ("c:/this/is/a/test");

      var handle = resourceManager.AddResource (fileInfo);
      var count = resourceManager.ResourceCount;

      Assert.That (resourceManager.AddResource (fileInfoDuplicate), Is.EqualTo (handle));
      Assert.That (count, Is.EqualTo (resourceManager.ResourceCount));
    }

    [Test]
    public void AddResources_AddDuplicateResources_ShouldThrowInvalidOperation ()
    {
      var resourceManager = new ResourceManager();
      var resources = new[]
                      {
                          new FileInfo ("c:/this/is/a/test"),
                          (FileSystemInfo) new DirectoryInfo ("c:/this/is/a/test/dir"),
                          new FileInfo ("c:/this/is/another/test"),
                          new FileInfo ("c:/this/is/yet/another/test"),
                      };

      var handles = resourceManager.AddResources (resources);
      var count = resourceManager.ResourceCount;

      Assert.That (
          resourceManager.AddResources (resources), Is.EquivalentTo (handles));

      Assert.That (count, Is.EqualTo (resourceManager.ResourceCount));
    }

    [Test]
    public void RemoveResource_RemoveByInvalidHandle_ShouldThrowNothing ()
    {
      Assert.That (() => new ResourceManager().RemoveResource (new ResourceHandle (Guid.NewGuid())), Throws.Nothing);
    }

    [Test]
    public void RemoveResource_RemoveByValidHandle_ShouldSucceed ()
    {
      var resourceManager = new ResourceManager();
      var count = resourceManager.ResourceCount;
      var resourceHandle = resourceManager.AddResource (new FileInfo ("c:/this/is/a/test"));

      Assert.That (() => resourceManager.RemoveResource (resourceHandle), Throws.Nothing);
      Assert.That (count, Is.EqualTo (resourceManager.ResourceCount));
    }
  }
}