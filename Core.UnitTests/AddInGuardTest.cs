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
using DesktopGap.Security;
using NUnit.Framework;

namespace DesktopGap.UnitTests
{
  [TestFixture]
  public class AddInGuardTest
  {
    [Test]
    public void IsAllowed_AskForProhibitedAddIn_ShouldReturnFalse ()
    {
      var addInGuard = new AddInGuard();
      addInGuard.ChangeRule ("not.allowed", false);
      Assert.That (addInGuard.IsAllowed ("not.allowed"), Is.False);
    }

    [Test]
    public void IsAllowed_AskForPermittedAddIn_ShouldReturnTrue ()
    {
      var addInGuard = new AddInGuard();
      addInGuard.ChangeRule ("is.allowed", true);
      Assert.That (addInGuard.IsAllowed ("is.allowed"), Is.True);
    }

    [Test]
    public void IsAllowed_AskForInexistentAddIn_ShouldReturnFalse ()
    {
      var addInGuard = new AddInGuard();
      Assert.That (addInGuard.IsAllowed ("this/is/not/allowed.html"), Is.False);
    }

    [Test]
    public void ChangeRule_AddDuplicateRule_ShouldThrowInvalidOperationException ()
    {
      var addInGuard = new AddInGuard();
      addInGuard.ChangeRule ("duplicate.rule", true);
      Assert.That (() => addInGuard.ChangeRule ("duplicate.rule", true), Throws.InvalidOperationException);
    }

    [Test]
    public void ChangeRule_AddControverseRule_ShouldUseLatter ()
    {
      var addInGuard = new AddInGuard();
      addInGuard.ChangeRule ("duplicate.rule", true);
      Assert.That (addInGuard.IsAllowed ("duplicate.rule"), Is.True);
      addInGuard.ChangeRule ("duplicate.rule", false);
      Assert.That (addInGuard.IsAllowed ("duplicate.rule"), Is.False);
    }
  }
}