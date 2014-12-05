using System.Collections.Generic;

namespace ClearScript.Manager
{
    /// <summary>
    /// Options for execution of of scripts by the Runtime Manager
    /// </summary>
    public class ExecutionOptions
    {
        private bool _addToCache = true;

        /// <summary>
        /// Objects to inject into the JavaScript runtime.
        /// </summary>
        public IEnumerable<HostObject> HostObjects { get; set; }

        /// <summary>
        /// Types to make available to the JavaScript runtime.
        /// </summary>
        public IEnumerable<HostType> HostTypes { get; set; }

        /// <summary>
        /// Indicates that this script should be added to the script cache once compiled.  Default is True.
        /// </summary>
        public bool AddToCache
        {
            get { return _addToCache; }
            set { _addToCache = value; }
        }

        /// <summary>
        /// Indicates the duration that a script should be cached.
        /// </summary>
        public int? CacheExpirationSeconds { get; set; }

        /// <summary>
        /// External JavaScripts to import before executing the current script.
        /// </summary>
        public IList<IncludeScript> Scripts { get; set; }
    }
}
