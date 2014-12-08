using System.Threading.Tasks;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Interface for all classes that load scripts from a path or URI
    /// </summary>
    public interface IScriptLoader
    {
        /// <summary>
        /// Determines if this script loader should be used at all.
        /// </summary>
        /// <param name="script">Script to evaluate.</param>
        /// <returns>Bool indicating if this loader should be used.</returns>
        bool ShouldUse(IncludeScript script);

        /// <summary>
        /// Loads the scripts Code property from the other available properties of the IncludeScript, typically using hte Uri.
        /// </summary>
        /// <param name="script"></param>
        Task LoadCodeAsync(IncludeScript script);
    }

}
