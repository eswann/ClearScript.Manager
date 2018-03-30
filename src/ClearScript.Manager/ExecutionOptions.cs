using System.Collections.Generic;

namespace JavaScript.Manager
{
    /// <summary>
    /// Options for execution of of scripts by the Runtime Manager
    /// </summary>
    public class ExecutionOptions
    {
        private bool _addToCache = true;
        private IList<HostObject> _hostObjects;
        private IList<HostType> _hostTypes;
        private IList<IncludeScript> _scripts;

        /// <summary>
        /// Objects to inject into the JavaScript runtime.
        /// </summary>
        public IList<HostObject> HostObjects
        {
            get { return _hostObjects ?? (_hostObjects = new List<HostObject>()); }
            set { _hostObjects = value; }
        }

        /// <summary>
        /// Types to make available to the JavaScript runtime.
        /// </summary>
        public IList<HostType> HostTypes
        {
            get { return _hostTypes ?? (_hostTypes = new List<HostType>()); }
            set { _hostTypes = value; }
        }

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
        public IList<IncludeScript> Scripts
        {
            get { return _scripts ?? (_scripts = new List<IncludeScript>()); }
            set { _scripts = value; }
        }
    }
}
