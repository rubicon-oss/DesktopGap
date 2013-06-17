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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DesktopGap.Configuration;
using DesktopGap.UnitTests.Utilities;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class DesktopGapManifestConfigurationTest
  {
    private const string c_manifestHead = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  
    <configSections>
    <section name=""desktopGapConfiguration"" type=""DesktopGap.Configuration.DesktopGapConfiguration, DesktopGap.Core"" />
  </configSections>
  
  <desktopGapConfiguration>";

    private const string c_manifestTail = @"</desktopGapConfiguration></configuration>";


    private const string c_applicationTagName = "application";

    private const string c_securityTagName = "security";
    private const string c_thirdPartyUrlsTagName = "allowedNonApplicationUrls";
    private const string c_applicationUrlsTagName = "applicationUrls";
    private const string c_startUpTagName = "startupURLs";
    private const string c_addInsTagName = "addIns";

    private const string c_addTagName = "add";
    private const string c_removeTagName = "remove";

    private const string c_domainAttribute = "domain";
    private const string c_pathAttribute = "path";
    private const string c_nameAttribute = "name";
    private const string c_homeUrlAttribute = "homeUrl";
    private const string c_baseUrlAttribute = "baseUrl";
    private const string c_maxFrameNestingDepthAttribute = "maxFrameNestingDepth";
    private const string c_alwaysOpenHomeUrlAttribute = "alwaysOpenHomeUrl";
    private const string c_allowCloseHomeTabAttribute = "allowCloseHomeTab";
    private const string c_iconAttribute = "icon";
    private const string c_alwaysShowUrlAttribute = "alwaysShowUrl";

    private const string c_patternAttribute = "useRegex";
    private const string c_sslAttribute = "requireSSL";


    private string OpenTag (string name, IEnumerable<KeyValuePair<string, string>> attributes = null)
    {
      var result = string.Format ("<{0}", name);
      if (attributes == null)
        return result + ">";

      var attributeBuilder = new StringBuilder (result);
      foreach (var attributeString in attributes.Select (attribute => attribute.Key + "=\"" + attribute.Value + "\""))
        attributeBuilder.AppendFormat (" {0}", attributeString);

      attributeBuilder.Append (">");
      return attributeBuilder.ToString();
    }

    private string InlineTag (string name, IEnumerable<KeyValuePair<string, string>> attributes = null)
    {
      return OpenTag (name, attributes).Replace (">", "/>");
    }

    private string CloseTag (string name)
    {
      return string.Format ("</{0}>", name);
    }

    [Test]
    public void DesktopGapConfigurationProviderCreate_InvalidFile_ShouldThrowArgumentException ()
    {
      Assert.That (
          () => DesktopGapConfigurationProvider.Create ("", "hello").GetConfiguration(), Throws.InstanceOf<ArgumentException>());
    }


    [Test]
    public void Xml_UseInvalidTagNames_ShouldThrowConfigurationErrorsException ()
    {
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag ("invalid"));

        var attributes = new Dictionary<string, string> { { c_domainAttribute, ".*" }, { c_pathAttribute, ".*" } };

        stringBuilder.Append (InlineTag (c_addTagName, attributes));

        stringBuilder.Append (CloseTag ("invalid"));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());
        var configurationProvider = DesktopGapConfigurationProvider.Create ("", tempFile.FileName);
        Assert.That (() => configurationProvider.GetConfiguration(), Throws.InstanceOf<ConfigurationErrorsException>());
      }
    }


    [Test]
    public void Xml_UseMinimalConfiguration_ShouldSucceed ()
    {
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (InlineTag (c_applicationTagName, new Dictionary<string, string> { { c_baseUrlAttribute, "base.url.com" } }));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());
        var configurationProvider = DesktopGapConfigurationProvider.Create ("", tempFile.FileName);
        Assert.That (() => configurationProvider.GetConfiguration(), Throws.Nothing);
      }
    }

    [Test]
    public void Xml_ValidAllowedNonApplicationUrlsPatternTag_ShouldSucceed ()
    {
      var domain = ".*";
      var path = ".*";

      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);
        stringBuilder.Append (InlineTag (c_applicationTagName, new Dictionary<string, string> { { c_baseUrlAttribute, "base.url.com" } }));


        stringBuilder.Append (OpenTag (c_securityTagName));

        stringBuilder.Append (OpenTag (c_thirdPartyUrlsTagName));

        var attributes = new Dictionary<string, string>
                         {
                             { c_domainAttribute, domain },
                             { c_pathAttribute, path },
                             { c_patternAttribute, true.ToString() }
                         };

        stringBuilder.Append (InlineTag (c_addTagName, attributes));

        stringBuilder.Append (CloseTag (c_thirdPartyUrlsTagName));
        stringBuilder.Append (CloseTag (c_securityTagName));


        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        var rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Security.NonApplicationUrlRules.ToArray();
        var actualDomainExpression = rules.First().DomainExpression;
        var actualPathExpression = rules.First().PathExpression;

        Assert.That (
            (actualDomainExpression.ToString() == domain) &&
            (actualPathExpression.ToString() == path),
            Is.True);
      }
    }

    [Test]
    public void Xml_ValidAllowedNonApplicationUrlsNameTag_ShouldSucceed ()
    {
      var domain = "example.domain.com";
      var path = "/path";

      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);
        stringBuilder.Append (InlineTag (c_applicationTagName, new Dictionary<string, string> { { c_baseUrlAttribute, "base.url.com" } }));

        stringBuilder.Append (OpenTag (c_securityTagName));

        stringBuilder.Append (OpenTag (c_thirdPartyUrlsTagName));

        var attributes = new Dictionary<string, string>
                         {
                             { c_domainAttribute, domain },
                             { c_pathAttribute, path }
                         };

        stringBuilder.Append (InlineTag (c_addTagName, attributes));

        stringBuilder.Append (CloseTag (c_thirdPartyUrlsTagName));
        stringBuilder.Append (CloseTag (c_securityTagName));


        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        var rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Security.NonApplicationUrlRules.ToArray();
        var actualDomainExpression = rules.First().DomainExpression;
        var actualPathExpression = rules.First().PathExpression;

        var escapedDomain = Regex.Escape (domain);
        var escapedPath = Regex.Escape (path);

        Assert.That (
            (actualDomainExpression.ToString() == escapedDomain) &&
            (actualPathExpression.ToString() == escapedPath),
            Is.True);
      }
    }


    [Test]
    public void Xml_ValidAddInTagAllow_ShouldSucceed ()
    {
      var name = "valid.module.name";
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);
        stringBuilder.Append (InlineTag (c_applicationTagName, new Dictionary<string, string> { { c_baseUrlAttribute, "base.url.com" } }));

        stringBuilder.Append (OpenTag (c_securityTagName));
        stringBuilder.Append (OpenTag (c_addInsTagName));

        var attributes = new Dictionary<string, string> { { c_nameAttribute, name } };

        stringBuilder.Append (InlineTag (c_addTagName, attributes));

        stringBuilder.Append (CloseTag (c_addInsTagName));
        stringBuilder.Append (CloseTag (c_securityTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        var rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Security.AddInRules.ToArray();
        var actual = rules.First().Name;

        Assert.That (actual == name, Is.True);
      }
    }
  }
}