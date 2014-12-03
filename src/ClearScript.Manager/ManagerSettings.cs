using System;
using System.Configuration;

namespace ClearScript.Manager
{
    /// <summary>
    /// Settings to apply to the RuntimeManager and to created runtimes.
    /// </summary>
    public interface IManagerSettings
    {
        /// <summary>
        /// V8 Max Executable Size in bytes.
        /// </summary>
        int MaxExecutableBytes { get; }

        /// <summary>
        /// V8 Max New Space in bytes.
        /// </summary>
        int MaxNewSpaceBytes { get; }

        /// <summary>
        /// V8 Max Young Space in bytes.
        /// </summary>
        [Obsolete("Use MaxNewSpaceBytes instead.")]
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

    /// <summary>
    /// Settings to apply to the RuntimeManager and to created runtimes.
    /// </summary>
    public class ManagerSettings : IManagerSettings
    {
        /// <summary>
        /// Default MaxExecutableBytes if not present in settings.
        /// </summary>
        public const int DefaultMaxExecutableBytes = 8 * 1024 * 1024;
        /// <summary>
        /// Default MaxNewSpaceBytes if not present in settings.
        /// </summary>
        public const int DefaultMaxNewSpaceBytes = 16 * 1024 * 1024;
        /// <summary>
        /// Default tMaxOldSpaceBytes if not present in settings.
        /// </summary>
        public const int DefaultMaxOldSpaceBytes = 16 * 1024 * 1024;
        /// <summary>
        /// Default ScriptTimeoutMilliSeconds if not present in settings.
        /// </summary>
        public const int DefaultScriptTimeoutMilliSeconds = 60000;
        /// <summary>
        /// Default RuntimeMaxCount if not present in settings.
        /// </summary>
        public const int DefaultRuntimeMaxCount = 8;
        /// <summary>
        /// Default ScriptCacheMaxCount if not present in settings.
        /// </summary>
        public const int DefaultScriptCacheMaxCount = 1000;
        /// <summary>
        /// Default ScriptCacheExpirationSeconds if not present in settings.
        /// </summary>
        public const int DefaultScriptCacheExpirationSeconds = 600;

        public int MaxExecutableBytes
        {
            get { return SettingToInt("MaxExecutableBytes", DefaultMaxExecutableBytes); }
        }

        public int MaxNewSpaceBytes
        {
            get
            {
                return SettingToInt("MaxNewSpaceBytes", SettingToInt("MaxYoungSpaceBytes", DefaultMaxNewSpaceBytes));
            }
        }

        [Obsolete("Use MaxNewSpaceBytes instead.")]
        public int MaxYoungSpaceBytes
        {
            get { return MaxNewSpaceBytes; }
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


        /// <summary>
        /// Parses the setting and converts it to an int or sets the default value if the setting is not present.
        /// </summary>
        /// <param name="settingName">Name of the setting to check.</param>
        /// <param name="defaultValue">Default value if setting is not present.</param>
        /// <returns>Setting or default value.</returns>
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
        /// <summary>
        /// Creates a ManualManagerSettings.
        /// </summary>
        public ManualManagerSettings()
        {
            MaxExecutableBytes = ManagerSettings.DefaultMaxExecutableBytes;
            MaxNewSpaceBytes = ManagerSettings.DefaultMaxOldSpaceBytes;
            MaxOldSpaceBytes = ManagerSettings.DefaultMaxNewSpaceBytes;
            ScriptTimeoutMilliSeconds = ManagerSettings.DefaultScriptTimeoutMilliSeconds;
            RuntimeMaxCount = ManagerSettings.DefaultRuntimeMaxCount;
            ScriptCacheMaxCount = ManagerSettings.DefaultScriptCacheMaxCount;
            ScriptCacheExpirationSeconds = ManagerSettings.DefaultScriptCacheExpirationSeconds;
        }

        public int MaxExecutableBytes { get; set; }

        public int MaxNewSpaceBytes { get; set; }

        [Obsolete("Use MaxNewSpaceBytes instead.")]
        public int MaxYoungSpaceBytes
        {
            get { return MaxNewSpaceBytes; }
            set { MaxNewSpaceBytes = value; }
        }

        public int MaxOldSpaceBytes { get; set; }

        public int ScriptTimeoutMilliSeconds { get; set; }

        public int RuntimeMaxCount { get; set; }

        public int ScriptCacheMaxCount { get; set; }

        public int ScriptCacheExpirationSeconds { get; set; }

    }
}