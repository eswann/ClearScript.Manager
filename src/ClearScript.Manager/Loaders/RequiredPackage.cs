using System.Collections.Generic;

namespace ClearScript.Manager.Loaders
{
    public class RequiredPackage
    {
        private readonly IList<HostObject> _hostObjects = new List<HostObject>();
        private readonly IList<HostType> _hostTypes = new List<HostType>();

        public string PackageId { get; set; }

        public string ScriptUri { get; set; }

        public IList<HostObject> HostObjects
        {
            get { return _hostObjects; }
        }

        public IList<HostType> HostTypes
        {
            get { return _hostTypes; }
        }
    }
}