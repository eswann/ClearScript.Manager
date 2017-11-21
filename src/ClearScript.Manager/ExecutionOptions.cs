using System.Collections.Generic;
using JetBrains.Annotations;

namespace ClearScript.Manager
{
    /// <summary>
    /// Options for execution of scripts by the Runtime Manager
    /// </summary>
    public class ExecutionOptions
    {
        private IList<HostObject> _hostObjects;
        private IList<HostType> _hostTypes;
        private IList<IncludeScript> _scripts;

        /// <summary>
        /// Objects to inject into the JavaScript runtime.
        /// </summary>
        public IList<HostObject> HostObjects
        {
            get => _hostObjects ?? (_hostObjects = new List<HostObject>());
            set => _hostObjects = value;
        }

        /// <summary>
        /// Types to make available to the JavaScript runtime.
        /// </summary>
        public IList<HostType> HostTypes
        {
            get => _hostTypes ?? (_hostTypes = new List<HostType>());
            set => _hostTypes = value;
        }

        /// <summary>
        /// Indicates that this script should be added to the script cache once compiled.  Default is True.
        /// </summary>
        public bool AddToCache { get; set; } = true;

        /// <summary>
        /// Indicates the duration that a script should be cached.
        /// </summary>
        public int? CacheExpirationSeconds { get; set; }

        /// <summary>
        /// External JavaScripts to import before executing the current script.
        /// </summary>
        public IList<IncludeScript> Scripts
        {
            get => _scripts ?? (_scripts = new List<IncludeScript>());
            set => _scripts = value;
        }
    }
}
