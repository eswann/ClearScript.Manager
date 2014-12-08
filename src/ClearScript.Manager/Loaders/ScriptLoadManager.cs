using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static void RegisterLoader(IScriptLoader loader)
        {
            _loaders.Add(loader);
        }

        /// <summary>
        /// Uses the manager to load a script.
        /// </summary>
        /// <param name="script">The script to load.</param>
        public static async Task LoadScript(IncludeScript script)
        {
            foreach (var scriptLoader in _loaders)
            {
                if (scriptLoader.ShouldUse(script))
                    await scriptLoader.LoadCodeAsync(script);
            }
        }

    }
}