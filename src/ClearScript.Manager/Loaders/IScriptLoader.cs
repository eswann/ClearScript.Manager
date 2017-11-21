using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Interface for all classes that load scripts from a path or URI
    /// </summary>
    public interface IScriptLoader
    {
        /// <summary>
        /// Name of the script loader.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines if this script loader should be used at all.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <returns><see cref="bool"/> indicating if this loader should be used.</returns>
        bool ShouldUse([NotNull] IncludeScript script);

        /// <summary>
        /// Loads the scripts Code property from the other available properties of the IncludeScript, typically using the Uri.
        /// </summary>
        /// <param name="script">Script to load</param>
        void LoadCode([NotNull] IncludeScript script);
    }

}
