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
        public static string LoginId = "LoginId";

        public static void ClearSettings()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
            registryKey.DeleteSubKeyTree(TempDatas.SoftNameEN);
        }

        public static void SaveSetting(string name_, object value_)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey subKey = registryKey.CreateSubKey(TempDatas.SoftNameEN);
            subKey.SetValue(name_, value_);
        }

        public static object GetSetting(string name_, object defaulValue_)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey($@"Software\{TempDatas.SoftNameEN}", true);
            if (registryKey == null) return null;

            return registryKey.GetValue(name_, defaulValue_);
        }
    }
}
