using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    class RegistryHelper
    {
        public const string LoginId = "LoginId";
        public const string DefaulLoginId = "VNW0014732";

        public static void ClearSettings()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
            registryKey.DeleteSubKeyTree(TPConfigs.SoftNameEN);
        }

        public static void SaveSetting(string name_, object value_)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey subKey = registryKey.CreateSubKey(TPConfigs.SoftNameEN);
            subKey.SetValue(name_, value_);
        }

        public static object GetSetting(string name_, object defaulValue_)
        {
            RegistryKey registryFolder = Registry.CurrentUser.OpenSubKey($@"Software", true);
            if (!registryFolder.GetSubKeyNames().Contains(TPConfigs.SoftNameEN))
            {
                SaveSetting(name_, defaulValue_);
            }

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey($@"Software\{TPConfigs.SoftNameEN}", true);
            if (!registryKey.GetValueNames().Contains(name_)) return null;

            return registryKey.GetValue(name_, defaulValue_);
        }
    }
}
