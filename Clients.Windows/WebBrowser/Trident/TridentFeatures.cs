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
using System.IO;
using System.Reflection;
using DesktopGap.Utilities;
using Microsoft.Win32;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  public class TridentFeatures
  {
    private const string c_gpuRenderingKey = "FEATURE_GPU_RENDERING";
    private const string c_browserModeKey = "FEATURE_BROWSER_EMULATION";
    private const string c_localMachineLockdown = "FEATURE_LOCALMACHINE_LOCKDOWN";
    private const string c_localObjectBlocking = "FEATURE_BLOCK_LMZ_OBJECT";
    private const string c_resourceProtocolRestriction = "FEATURE_RESTRICT_RES_TO_LMZ";

    private const string c_featureControlLocation = @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl";
    private const string c_registrySeperator = "\\";

    private readonly string _applicationName;


    public TridentFeatures ()
    {
      _applicationName = Path.GetFileName (Assembly.GetExecutingAssembly().GetName().CodeBase);
    }


    public bool RestrictResourceProtocol
    {
      get { return GetKey (c_resourceProtocolRestriction) == "1"; }
      set { SetKey (c_resourceProtocolRestriction, value ? "1" : "0"); }
    }

    public bool LocalObjectBlocking
    {
      get { return GetKey (c_localObjectBlocking) == "1"; }
      set { SetKey (c_localObjectBlocking, value ? "1" : "0"); }
    }


    public bool LocalMachineLockdown
    {
      get { return GetKey (c_localMachineLockdown) == "1"; }
      set { SetKey (c_localMachineLockdown, value ? "1" : "0"); }
    }

    public bool GpuAcceleration
    {
      get { return GetKey (c_gpuRenderingKey) == "1"; }
      set { SetKey (c_gpuRenderingKey, value ? "1" : "0"); }
    }

    public TridentWebBrowserMode BrowserEmulationMode
    {
      get { return (TridentWebBrowserMode) Enum.ToObject (typeof (TridentWebBrowserMode), int.Parse (GetKey (c_browserModeKey))); }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        SetKey (c_browserModeKey, value.ToString ("D"));
      }
    }

    private void SetKey (string keyName, string value)
    {
      using (var key = Registry.CurrentUser.CreateSubKey (
          string.Join (c_registrySeperator, c_featureControlLocation, keyName),
          RegistryKeyPermissionCheck.ReadWriteSubTree))
      {
        if (key == null)
          throw new InvalidOperationException ("Could not open subkey for modification");

        key.SetValue (_applicationName, value, RegistryValueKind.DWord);
      }
    }

    private string GetKey (string keyName)
    {
      using (var key = Registry.CurrentUser.CreateSubKey (
          string.Join (c_registrySeperator, c_featureControlLocation, keyName),
          RegistryKeyPermissionCheck.ReadWriteSubTree))
      {
        if (key == null)
          throw new InvalidOperationException ("Could not open subkey for modification");
        var result = key.GetValue (_applicationName, null);
        return result != null ? result.ToString() : null;
      }
    }
  }
}