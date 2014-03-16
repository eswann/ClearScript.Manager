using System.Configuration;

namespace ClearScript.Manager
{
    public interface IManagerSettings
    {
        /// <summary>
        /// V8 Max Executable Size in bytes.
        /// </summary>
        int MaxExecutableBytes { get; }

        /// <summary>
        /// V8 Max Young Space in bytes.
        /// </summary>
        int MaxYoungSpaceBytes { get; }

        /// <summary>
        /// V8 Max Old Space in bytes.
        /// </summary>
        int MaxOldSpaceBytes { get; }

        /// <summary>
        /// Default script timeout in ms.
        /// </summary>
        int ScriptTimeoutMilliSeconds { get; }

        /// <summary>
        /// Max number of simultaneous V8 Runtimes.
        /// </summary>
        int RuntimeMaxCount { get; }

        /// <summary>
        /// Per Runtime, the maximum number of cached scripts.
        /// </summary>
        int ScriptCacheMaxCount { get; }

        /// <summary>
        /// The default script cache expiration in seconds.
        /// </summary>
        int ScriptCacheExpirationSeconds { get; }
    }

    public class ManagerSettings : IManagerSettings
    {
        public const int DefaultMaxExecutableBytes = 8 * 1024 * 1024;
        public const int DefaultMaxYoungSpaceBytes = 16 * 1024 * 1024;
        public const int DefaultMaxOldSpaceBytes = 16 * 1024 * 1024;
        public const int DefaultScriptTimeoutMilliSeconds = 60000;
        public const int DefaultRuntimeMaxCount = 8;
        public const int DefaultScriptCacheMaxCount = 1000;
        public const int DefaultScriptCacheExpirationSeconds = 600;

        public int MaxExecutableBytes
        {
            get { return SettingToInt("MaxExecutableBytes", DefaultMaxExecutableBytes); }
        }

        public int MaxYoungSpaceBytes
        {
            get { return SettingToInt("MaxYoungSpaceBytes", DefaultMaxYoungSpaceBytes); }
        }

        public int MaxOldSpaceBytes
        {
            get { return SettingToInt("MaxOldSpaceBytes", DefaultMaxOldSpaceBytes); }
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

        public int ScriptCacheExpirationSeconds
        {
            get { return SettingToInt("ScriptCacheExpirationSeconds", DefaultScriptCacheExpirationSeconds); }
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
            MaxExecutableBytes = ManagerSettings.DefaultMaxExecutableBytes;
            MaxYoungSpaceBytes = ManagerSettings.DefaultMaxOldSpaceBytes;
            MaxOldSpaceBytes = ManagerSettings.DefaultMaxYoungSpaceBytes;
            ScriptTimeoutMilliSeconds = ManagerSettings.DefaultScriptTimeoutMilliSeconds;
            RuntimeMaxCount = ManagerSettings.DefaultRuntimeMaxCount;
            ScriptCacheMaxCount = ManagerSettings.DefaultScriptCacheMaxCount;
            ScriptCacheExpirationSeconds = ManagerSettings.DefaultScriptCacheExpirationSeconds;
        }

        public int MaxExecutableBytes { get; set; }

        public int MaxYoungSpaceBytes { get; set; }

        public int MaxOldSpaceBytes { get; set; }

        public int ScriptTimeoutMilliSeconds { get; set; }

        public int RuntimeMaxCount { get; set; }

        public int ScriptCacheMaxCount { get; set; }

        public int ScriptCacheExpirationSeconds { get; set; }

    }
}