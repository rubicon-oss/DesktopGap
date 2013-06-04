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
      <allowedNonApplicationUrls>
        <add domain=""this"" path=""/is/allowed.html"" />
        <add domain=""?his"" path=""/is/allowed*"" />
        <add domain=""*this"" path=""/is/allow?"" />
        <add domain=""this"" path=""/is/allowed.html"" />
        <add domain=""this"" path=""/is/secure.html"" requireSSL=""true"" />
        <add domain=""this"" path=""/is/(also/)?allowed.html"" useRegex=""true"" />        
      </allowedNonApplicationUrls>
    </security>	

  </desktopGapConfiguration>
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
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://not/allowed.html"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForProhibitedUrl2_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://some.stuff.test.com/badapp"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForProhibitedUrl3_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("ftp://another.test.de/badapp"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrl_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/allowed.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrl2_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("ftp://this/is/allowed.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForWildcardedUrl_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://fhis/is/allowed-and-valid"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForWildcardedUrl2_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://www.allow.this/is/allowd"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForWildcardedUrl3_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/also/allowed.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForSSLOnlyUrlWithoutSSL3_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/secure.html"), Is.False);
    }


    [Test]
    public void IsAllowed_AskForSSLOnlyUrlWithSSL3_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("https://this/is/secure.html"), Is.True);
    }


    [Test]
    public void IsAllowed_AskForPermittedUrl3_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/allowed.html?key=value"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrlWithAdditionalContent_ShouldReturnTrue ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/stuff/is/allowed.html?key=value"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForInexistentRule_ShouldReturnFalse ()
    {
      var urlGuard = new UrlFilter (_urlRules);
      Assert.That (urlGuard.IsAllowed ("http://this/is/not/allowed.html"), Is.False);
    }
  }
}