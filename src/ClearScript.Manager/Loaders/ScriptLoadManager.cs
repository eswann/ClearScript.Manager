using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Manages the loading of scripts for the runtime manager.
    /// </summary>
    public static class ScriptLoadManager
    {
        private static readonly List<IScriptLoader> Loaders = new List<IScriptLoader> {new FileScriptLoader()};

        /// <summary>
        /// Register a script loader with the manager.
        /// </summary>
        /// <param name="loader">The script loader to register.</param>
        public static void RegisterLoader([NotNull] this IScriptLoader loader)
        {
            if (loader == null) throw new ArgumentNullException(nameof(loader));
            Loaders.Add(loader);
        }

        /// <summary>
        /// Uses the manager to load a script.
        /// </summary>
        /// <param name="script">The script to load.</param>
        public static void LoadScript([NotNull] this IncludeScript script)
        {
            if (script == null) throw new ArgumentNullException(nameof(script));

            foreach (var scriptLoader in Loaders)
            {
                if (scriptLoader.ShouldUse(script))
                {
                    try { 
                        scriptLoader.LoadCode(script);
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"{scriptLoader.Name} failed to load script with Script ID:{script.ScriptId} and Script Uri:{script.Uri}. Check InnerException for more info.", ex);
                    }
                }
            }

            throw new ArgumentException(
                $"The provided script could not be loaded with any of the available script loaders. Script ID:{script.ScriptId}  Script Uri:{script.Uri}.");
        }

    }
}