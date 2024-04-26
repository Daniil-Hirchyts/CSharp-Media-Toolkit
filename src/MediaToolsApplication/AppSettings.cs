using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MediaToolsApplication
{
    class AppSettings
    {
        public static void AddSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (configuration.AppSettings.Settings[key] == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            else throw new Exception($"Setting '{key}' already exists in the App.config file.");
        }

        public static void UpdateAppSettings(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string? ReadSetting(string key)
        {
            var settingValue = ConfigurationManager.AppSettings[key];
            return settingValue ?? throw new Exception($"Setting '{key}' does not exist in the App.config file.");
        }
    }
}
