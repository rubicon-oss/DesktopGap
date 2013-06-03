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
using DesktopGap.Configuration;
using DesktopGap.Security.Urls;
using DesktopGap.UnitTests.Utilities;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class UrlFilterTest
  {
    private const string c_manifest = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  
    <configSections>
    <section name=""DesktopGapConfiguration"" type=""DesktopGap.Configuration.DesktopGapConfiguration, DesktopGap.Core"" />
  </configSections>
  
  <DesktopGapConfiguration>
    <Application name=""TestApp"" frameNestingDepth=""5"" baseUrl=""http://localhost:3936"">
      <Favicon location=""C:\Development\DesktopGap.Sandbox\WebHostWebApplication\rainbow-dash.png""/>
    </Application>
    
    <Security>
      <StartupUrls>			
      </StartupUrls>
      <ThirdPartyUrls>
        <Url domain=""this/is/"" path=""allowed.html"" />
      </ThirdPartyUrls>
      <ApplicationUrls>
      </ApplicationUrls>
      <AddIns>			
      </AddIns>
    </Security>	

  </DesktopGapConfiguration>
</configuration>";


    private IEnumerable<PositiveUrlRule> _urlRules;

    [SetUp]
    public void SetUp ()
    {
      using (var tmp = new TempFile())
      {
        tmp.WriteAllText (c_manifest);
        var securityConfiguration = DesktopGapConfigurationProvider.Create ("", tmp.FileName).GetConfiguration();
        _urlRules = securityConfiguration.Security.NonApplicationUrlRules;
      }
    }

    [Test]
    public void IsAllowed_AskForProhibitedUrl_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://not/allowed.html"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForProhibitedUrl2_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://some.stuff.test.com/badapp"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForProhibitedUrl3_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("ftp://another.test.de/badapp"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrl_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/allowed.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrl2_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("ftp://this.domain.com/is/allowed.html"), Is.True);
    }


    [Test]
    public void IsAllowed_AskForPermittedUrl3_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://my.test.at/app?key=value"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrlWithAdditionalContent_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://my.test.at/asdasdasdsadasdasdsdasd/app?key=value"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForBaseUrlDescendent_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://base.domain.com/a/short/path/and/stuff/index.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForInexistentRule_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (new Uri ("http://base.domain.com/a/short/path"), _urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/not/allowed.html"), Is.False);
    }
  }
}