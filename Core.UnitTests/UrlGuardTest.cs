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
using System.Text.RegularExpressions;
using DesktopGap.Security;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class UrlGuardTest
  {
    [Test]
    public void IsAllowed_AskForProhibitedUrl_ShouldReturnFalse ()
    {
      var urlGuard = new UrlGuard();
      urlGuard.ChangeRule (@"[\s]+", @"[\s]+", false);
      Assert.That (urlGuard.IsAllowed ("http://not/allowed.html"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForPermittedUrl_ShouldReturnTrue ()
    {
      var urlGuard = new UrlGuard();
      urlGuard.ChangeRule (@"[^\s]+", @"[^\s]+", true);
      Assert.That (urlGuard.IsAllowed ("http://this/is/allowed.html"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForInexistentRule_ShouldReturnFalse ()
    {
      var urlGuard = new UrlGuard();
      Assert.That (urlGuard.IsAllowed ("http://this/is/not/allowed.html"), Is.False);
    }

    [Test]
    public void ChangeRule_AddDuplicateRule_ShouldThrowInvalidOperationException ()
    {
      var urlGuard = new UrlGuard();
      urlGuard.ChangeRule (@"[^\s]+", @"[^\s]+", true);
      Assert.That (() => urlGuard.ChangeRule (@"[^\s]+", @"[^\s]+", true), Throws.InvalidOperationException);
    }

    [Test]
    public void ChangeRule_AddControverseRule_ShouldUseLatter ()
    {
      var urlGuard = new UrlGuard();
      urlGuard.ChangeRule (@"[^\s]+", @"[^\s]+", true);
      Assert.That (urlGuard.IsAllowed ("this/is/allowed.html"), Is.True);
      urlGuard.ChangeRule (@"[^\s]+", @"[^\s]+", false);
      Assert.That (urlGuard.IsAllowed ("this/is/allowed.html"), Is.False);
    }

    [Test]
    public void ChangeRule_AddInvalidRegex_ShouldThrowArgumentException ()
    {
      var urlGuard = new UrlGuard();
      Assert.That (() => urlGuard.ChangeRule (@"[", @"]", false), Throws.ArgumentException);
    }
  }
}