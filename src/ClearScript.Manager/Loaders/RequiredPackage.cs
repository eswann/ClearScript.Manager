using System.Collections.Generic;
using System.IO;

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
        /// if the ScriptUri is the embedded resource
        /// </summary>
        /// <param name="embeddedUrl"></param>
        /// <returns></returns>
        public virtual string GetEmbeddedAsset(string embeddedUrl)
        {
            try
            {
                var thisAssembly = GetType().Assembly;
                using (var stream = thisAssembly.GetManifestResourceStream(embeddedUrl))
                {
                    if (stream == null)
                    {
                        return string.Empty;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }
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