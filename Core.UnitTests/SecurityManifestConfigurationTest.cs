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
using DesktopGap.Security.AddIns;
using DesktopGap.Security.Urls;
using DesktopGap.UnitTests.Utilities;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class SecurityManifestConfigurationTest
  {
    private const string c_manifestHead = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <configSections>
    <section name=""SecurityManifest"" type=""DesktopGap.Configuration.Security.SecurityManifestConfiguration, DesktopGap.Core"" />
  </configSections>
  <SecurityManifest>";

    private const string c_manifestTail = @"</SecurityManifest></configuration>";

    private const string c_urlsTagName = "Urls";
    private const string c_addInsTagName = "AddIns";
    private const string c_startUpTagName = "Startup";

    private const string c_urlTagName = "Url";
    private const string c_addInTagName = "AddIn";

    private const string c_allowTagName = "Allow";
    private const string c_denyTagName = "Deny";

    private const string c_domainAttribute = "domain";
    private const string c_pathAttribute = "path";
    private const string c_nameAttribute = "name";

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
    public void DesktopGapSecurityProviderCreate_InvalidFile_ShouldThrowArgumentExceptionn ()
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

        stringBuilder.Append (OpenTag (c_allowTagName));

        var attributes = new Dictionary<string, string> { { c_domainAttribute, ".*" }, { c_pathAttribute, ".*" } };

        stringBuilder.Append (InlineTag (c_urlTagName, attributes));

        stringBuilder.Append (CloseTag (c_allowTagName));

        stringBuilder.Append (CloseTag ("invalid"));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());
        Assert.That (
            () => DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration(), Throws.InstanceOf<ConfigurationErrorsException>());
      }
    }

    [Test]
    public void Xml_ValidUrlsTagAllowOnly_ShouldSucceed ()
    {
      var domainExpression = new Regex (".*");
      var pathExpression = new Regex (".*");


      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_urlsTagName));

        stringBuilder.Append (OpenTag (c_allowTagName));

        var attributes = new Dictionary<string, string>
                         {
                             { c_domainAttribute, domainExpression.ToString() },
                             { c_pathAttribute, pathExpression.ToString() }
                         };

        stringBuilder.Append (InlineTag (c_urlTagName, attributes));

        stringBuilder.Append (CloseTag (c_allowTagName));

        stringBuilder.Append (CloseTag (c_urlsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        IUrlRules rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Urls;
        var actualDomainExpression = rules.Allowed.First().DomainExpression;
        var actualPathExpression = rules.Allowed.First().PathExpression;

        Assert.That (
            (actualDomainExpression.ToString() == domainExpression.ToString()) &&
            (actualPathExpression.ToString() == pathExpression.ToString()),
            Is.True);
      }
    }

    [Test]
    public void Xml_ValidUrlsTagDenyOnly_ShouldSucceed ()
    {
      var domainExpression = new Regex (".*");
      var pathExpression = new Regex (".*");


      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_urlsTagName));

        stringBuilder.Append (OpenTag (c_denyTagName));

        var attributes = new Dictionary<string, string>
                         {
                             { c_domainAttribute, domainExpression.ToString() },
                             { c_pathAttribute, pathExpression.ToString() }
                         };

        stringBuilder.Append (InlineTag (c_urlTagName, attributes));

        stringBuilder.Append (CloseTag (c_denyTagName));

        stringBuilder.Append (CloseTag (c_urlsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        IUrlRules rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Urls;
        var actualDomainExpression = rules.Denied.First().DomainExpression;
        var actualPathExpression = rules.Denied.First().PathExpression;

        Assert.That (
            (actualDomainExpression.ToString() == domainExpression.ToString()) &&
            (actualPathExpression.ToString() == pathExpression.ToString()),
            Is.True);
      }
    }

    [Test]
    public void Xml_ValidUrlsTagDenyDomainOnly_ShouldSucceed ()
    {
      var domainExpression = new Regex (".*");

      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_urlsTagName));

        stringBuilder.Append (OpenTag (c_denyTagName));

        var attributes = new Dictionary<string, string> { { c_domainAttribute, domainExpression.ToString() } };

        stringBuilder.Append (InlineTag (c_urlTagName, attributes));

        stringBuilder.Append (CloseTag (c_denyTagName));

        stringBuilder.Append (CloseTag (c_urlsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        IUrlRules rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Urls;
        var actualDomainExpression = rules.Denied.First().DomainExpression;

        Assert.That (
            actualDomainExpression.ToString() == domainExpression.ToString(), Is.True);
      }
    }

    [Test]
    public void Xml_InvalidAttributes_ShouldThrowConfigurationErrorsException ()
    {
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_urlsTagName));

        stringBuilder.Append (OpenTag (c_denyTagName));

        var attributes = new Dictionary<string, string> { { c_domainAttribute, ")" } };

        stringBuilder.Append (InlineTag (c_urlTagName, attributes));

        stringBuilder.Append (CloseTag (c_denyTagName));

        stringBuilder.Append (CloseTag (c_urlsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());
        var rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().Urls;
        Assert.That (
            () => rules.Denied.First().DomainExpression,
            Throws.ArgumentException);
      }
    }

    [Test]
    public void Xml_ValidAddInTagDeny_ShouldSucceed ()
    {
      var name = "valid.module.name";
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_addInsTagName));

        stringBuilder.Append (OpenTag (c_denyTagName));

        var attributes = new Dictionary<string, string> { { c_nameAttribute, name } };

        stringBuilder.Append (InlineTag (c_addInTagName, attributes));

        stringBuilder.Append (CloseTag (c_denyTagName));

        stringBuilder.Append (CloseTag (c_addInsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        IAddInRules rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().AddIns;
        var actual = rules.Denied.First().Name;

        Assert.That (actual == name, Is.True);
      }
    }

    [Test]
    public void Xml_ValidAddInTagAllow_ShouldSucceed ()
    {
      var name = "valid.module.name";
      using (var tempFile = new TempFile())
      {
        var stringBuilder = new StringBuilder (c_manifestHead);

        stringBuilder.Append (OpenTag (c_addInsTagName));

        stringBuilder.Append (OpenTag (c_allowTagName));

        var attributes = new Dictionary<string, string> { { c_nameAttribute, name } };

        stringBuilder.Append (InlineTag (c_addInTagName, attributes));

        stringBuilder.Append (CloseTag (c_allowTagName));

        stringBuilder.Append (CloseTag (c_addInsTagName));

        stringBuilder.Append (c_manifestTail);

        tempFile.WriteAllText (stringBuilder.ToString());

        IAddInRules rules = DesktopGapConfigurationProvider.Create ("", tempFile.FileName).GetConfiguration().AddIns;
        var actual = rules.Allowed.First().Name;

        Assert.That (actual == name, Is.True);
      }
    }
  }
}