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
using JetBrains.Annotations;

namespace DesktopGap.Utilities
{
  public static class ArgumentUtility
  {
    [AssertionMethod]
    public static T CheckNotNull<T> ([InvokerParameterName] string argumentName, [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] [NoEnumeration] T actualValue)
    {
// ReSharper disable CompareNonConstrainedGenericWithNull
      if (actualValue == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    [AssertionMethod]
    public static string CheckNotNullOrEmpty ([InvokerParameterName] string argumentName, [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }
  }
}