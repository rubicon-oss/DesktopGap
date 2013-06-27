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
using System.Collections.Generic;
using DesktopGap.Configuration;
using DesktopGap.Security.AddIns;
using DesktopGap.UnitTests.Utilities;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class AddInFilterTest
  {
    private IEnumerable<AddInRule> _addInRules;
  private const string c_manifest = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  
  <configSections>
    <section name=""desktopGapConfiguration"" type=""DesktopGap.Configuration.DesktopGapConfiguration, DesktopGap.Core"" />
  </configSections>

  <desktopGapConfiguration>
    <application 
        name=""Test App"" 
        maxFrameNestingDepth=""5"" 
        baseUrl=""http://test.rubicon.eu/""
        homeUrl=""http://myapp.rubicon.eu/folder1/home.asp""
        alwaysOpenHomeUrl=""true""
        allowCloseHomeTab=""false""
        icon=""C:\Development\DesktopGap.Sandbox\WebHostWebApplication\rainbow-dash.png""
        alwaysShowUrl=""true"" 
      />
   
    <security>   
      <addIns>
        <add name=""Test.AddIn.Allowed"" />
      </addIns>
    </security>	

  </desktopGapConfiguration>
</configuration>";


    [SetUp]
    public void SetUp ()
    {
      using (var tmp = new TempFile())
      {
        tmp.WriteAllText (c_manifest);
        var securityConfiguration = DesktopGapConfigurationProvider.Create ("", tmp.FileName).GetConfiguration();
        _addInRules = securityConfiguration.Security.AddInRules;
      }
    }

      [Test]
    public void IsAllowed_AskForProhibitedAddIn_ShouldReturnFalse ()
    {
      var addInGuard = new AddInFilter (_addInRules);
      Assert.That (addInGuard.IsAllowed ("Test.AddIn.Denied"), Is.False);
    }


    [Test]
    public void IsAllowed_AskForPermittedAddIn_ShouldReturnTrue ()
    {
      var addInGuard = new AddInFilter (_addInRules);
      Assert.That (addInGuard.IsAllowed ("Test.AddIn.Allowed"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForInexistentAddIn_ShouldReturnFalse ()
    {
      var addInGuard = new AddInFilter (_addInRules);
      Assert.That (addInGuard.IsAllowed ("this/is/not/allowed.html"), Is.False);
    }
  }
}