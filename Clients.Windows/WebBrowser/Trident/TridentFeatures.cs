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
  public class TridentFeatures : IDisposable //TODO rewrite 
  {
    private const string c_gpuRenderingKey = "FEATURE_GPU_RENDERING";
    private const string c_browserModeKey = "FEATURE_BROWSER_EMULATION";

    private readonly RegistryKey _featureControl;
    private readonly string _applicationName;

    public TridentFeatures ()
    {
      // WoW64Node is not required
      _featureControl = Registry.CurrentUser.OpenSubKey ("SOFTWARE", true);

      if (_featureControl == null)
        throw new InvalidOperationException ("Could not open registry for modification");

      _featureControl = _featureControl.OpenSubKey (@"Microsoft\Internet Explorer\MAIN\FeatureControl", true)
                        ??
                        _featureControl.CreateSubKey (@"Microsoft\Internet Explorer\MAIN\FeatureControl", RegistryKeyPermissionCheck.ReadWriteSubTree);

      if (_featureControl == null)
        throw new InvalidOperationException ("Could not open registry for modification");

      // Important: The exact(!) filename of the running executable is written into the registry, 
      // as opposed to process name (DesktopGap.vshost.exe vs DesktopGap.exe <- works)
      _applicationName = Path.GetFileName (Assembly.GetExecutingAssembly().GetName().CodeBase);
    }

    public void Dispose ()
    {
      if (_featureControl != null)
        _featureControl.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
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
      using (
          var key = _featureControl.OpenSubKey (keyName, true) ?? _featureControl.CreateSubKey (keyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
      {
        if (key == null)
          throw new InvalidOperationException ("Could not open subkey for modification");

        key.SetValue (_applicationName, value, RegistryValueKind.DWord);
      }
    }

    private string GetKey (string keyName)
    {
      var key = _featureControl.OpenSubKey (keyName);
      return key != null ? key.GetValue (_applicationName, null).ToString() : null;
    }
  }
}