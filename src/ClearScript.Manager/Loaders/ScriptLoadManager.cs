using System;
using System.Collections.Generic;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Manages the loading of scripts for the runtime manager.
    /// </summary>
    public static class ScriptLoadManager
    {
        private static readonly List<IScriptLoader> _loaders = new List<IScriptLoader> {new FileScriptLoader()};

        /// <summary>
        /// Register a script loader with the manager.
        /// </summary>
        /// <param name="loader">The script loader to register.</param>
        public static void RegisterLoader(this IScriptLoader loader)
        {
            _loaders.Add(loader);
        }

        /// <summary>
        /// Uses the manager to load a script.
        /// </summary>
        /// <param name="script">The script to load.</param>
        public static void LoadScript(this IncludeScript script)
        {
            foreach (var scriptLoader in _loaders)
            {
                if (scriptLoader.ShouldUse(script))
                {
                    try { 
                        scriptLoader.LoadCode(script);
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(string.Format("{0} failed to load script with Script ID:{1} and Script Uri:{2}. Check InnerException for more info.",
                                scriptLoader.Name, script.ScriptId, script.Uri), ex);
                    }
                }
            }

            throw new ArgumentException(
                string.Format("The provided script could not be loaded with any of the available script loaders. Script ID:{0}  Script Uri:{1}.", script.ScriptId, script.Uri));
        }

    }
}