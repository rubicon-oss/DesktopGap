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
using System.Diagnostics;
using Microsoft.Win32;

namespace DesktopGap.Clients.Windows.WebBrowser.Trident
{
  public class TridentFeatures

  {
    private const string c_gpuRenderingKey = "FEATURE_GPU_RENDERING";
    private const string c_browserModeKey = "FEATURE_BROWSER_EMULATION";

    private readonly RegistryKey _featureControl;
    private readonly string _applicationName;

    public TridentFeatures ()
    {
      _featureControl = Registry.LocalMachine;
      _featureControl = Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Wow6432Node") ?? Registry.LocalMachine.OpenSubKey ("SOFTWARE");

      if (_featureControl == null)
        throw new Exception ("Registry key error"); // TODO use something proper

      _featureControl = _featureControl.OpenSubKey (@"Microsoft\Internet Explorer\MAIN\FeatureControl")
                        ?? _featureControl.CreateSubKey (@"Microsoft\Internet Explorer\MAIN\FeatureControl", RegistryKeyPermissionCheck.ReadSubTree);

      _applicationName = AppDomain.CurrentDomain.FriendlyName;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool GpuAcceleration
    {
      get { return GetKey (c_gpuRenderingKey) == "1"; }
      set { SetKey (c_gpuRenderingKey, value ? "1" : "0"); }
    }

    public WebBrowserMode BrowserEmulationMode
    {
      get { return (WebBrowserMode) Enum.ToObject (typeof (WebBrowserMode), int.Parse (GetKey (c_browserModeKey))); }
      set { SetKey (c_browserModeKey, value.ToString ("D")); }
    }

    private void SetKey (string keyName, string value)
    {
      var key = _featureControl.OpenSubKey (keyName, true) ?? _featureControl.CreateSubKey (keyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
      Debug.Assert (key != null);
      key.SetValue (_applicationName, value, RegistryValueKind.DWord);
      key.Close();
    }

    private string GetKey (string keyName)
    {
      var key = _featureControl.OpenSubKey (keyName);
      return key != null ? key.GetValue (_applicationName, null).ToString() : null;
    }
  }
}