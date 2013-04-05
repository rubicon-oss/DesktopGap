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

namespace JetBrains.Annotations
{
  /// <summary>
  /// Specifies assertion type. If the assertion method argument satisifes the condition, then the execution continues. 
  /// Otherwise, execution is assumed to be halted
  /// </summary>
  internal enum AssertionConditionType
  {
    /// <summary>
    /// Indicates that the marked parameter should be evaluated to true
    /// </summary>
    IS_TRUE = 0,

    /// <summary>
    /// Indicates that the marked parameter should be evaluated to false
    /// </summary>
    IS_FALSE = 1,

    /// <summary>
    /// Indicates that the marked parameter should be evaluated to null value
    /// </summary>
    IS_NULL = 2,

    /// <summary>
    /// Indicates that the marked parameter should be evaluated to not null value
    /// </summary>
    IS_NOT_NULL = 3,
  }
}