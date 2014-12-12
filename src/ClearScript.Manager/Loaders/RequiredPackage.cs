using System.Collections.Generic;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Provides a definition for a required package. Must contain a script or a hostobject.  
    /// </summary>
    public class RequiredPackage
    {
        private readonly IList<HostObject> _hostObjects = new List<HostObject>();
        private readonly IList<HostType> _hostTypes = new List<HostType>();

        /// <summary>
        /// The ID of the package.
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// A script URI of a package javascript.  The export from the package is returned from the require call.
        /// </summary>
        public string ScriptUri { get; set; }

        /// <summary>
        /// Host objects needed for the package.  If a script is not included, the first host object is returned when the package is required.
        /// </summary>
        public IList<HostObject> HostObjects
        {
            get { return _hostObjects; }
        }

        /// <summary>
        /// Host types needed for the package.
        /// </summary>
        public IList<HostType> HostTypes
        {
            get { return _hostTypes; }
        }
    }
}