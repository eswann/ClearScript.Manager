using System.Configuration;

namespace ClearScript.Manager
{
    public interface IManagerSettings
    {
        int StackAllocationMB { get; }
        int HeapAllocationMB { get; }
        int ScriptTimeoutMilliSeconds { get; }
        int RuntimeMaxCount { get; }
        int ScriptCacheMaxCount { get; }
    }

    public class ManagerSettings : IManagerSettings
    {
        public const int DefaultStackAllocationMB = 8 * 1024 * 1024;
        public const int DefaultHeapAllocationMB = 16 * 1024 * 1024;
        public const int DefaultScriptTimeoutMilliSeconds = 60000;
        public const int DefaultRuntimeMaxCount = 8;
        public const int DefaultScriptCacheMaxCount = 1000;

        public int StackAllocationMB
        {
            get { return SettingToInt("StackAllocationBytes", DefaultStackAllocationMB); }
        }

        public int HeapAllocationMB
        {
            get { return SettingToInt("HeapAllocationBytes", DefaultHeapAllocationMB); }
        }

        public int ScriptTimeoutMilliSeconds
        {
            get { return SettingToInt("ScriptTimeoutMilliSeconds", DefaultScriptTimeoutMilliSeconds); }
        }

        public int RuntimeMaxCount
        {
            get { return SettingToInt("RuntimeMaxCount", DefaultRuntimeMaxCount); }
        }

        public int ScriptCacheMaxCount
        {
            get { return SettingToInt("ScriptCacheMaxCount", DefaultScriptCacheMaxCount); }
        }


        public static int SettingToInt(string settingName, int defaultValue)
        {
            int? setting = null;

            string stringSetting = ConfigurationManager.AppSettings[settingName];

            int result;
            if (int.TryParse(stringSetting, out result))
            {
                setting = result;
            }

            return setting.GetValueOrDefault(defaultValue);
        }
    }

    /// <summary>
    /// This settings class is mainly for testing scenarios
    /// </summary>
    public class ManualManagerSettings : IManagerSettings
    {
        public ManualManagerSettings()
        {
            StackAllocationMB = ManagerSettings.DefaultStackAllocationMB;
            HeapAllocationMB = ManagerSettings.DefaultHeapAllocationMB;
            ScriptTimeoutMilliSeconds = ManagerSettings.DefaultScriptTimeoutMilliSeconds;
            RuntimeMaxCount = ManagerSettings.DefaultRuntimeMaxCount;
            ScriptCacheMaxCount = ManagerSettings.DefaultScriptCacheMaxCount;
        }

        public int StackAllocationMB { get; set; }

        public int HeapAllocationMB { get; set; }

        public int ScriptTimeoutMilliSeconds { get; set; }

        public int RuntimeMaxCount { get; set; }

        public int ScriptCacheMaxCount { get; set; }

    }
}