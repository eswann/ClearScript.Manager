using Microsoft.ClearScript;

namespace ClearScript.Manager
{
    /// <summary>
    /// A specific object instance to inject into the javascript to execute.
    /// </summary>
    public class HostObject
    {
        /// <summary>
        /// Name of the object used to access the object in the JavaScript.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// HostItemFlags to apply to the HostType.  Limits the scope of the type and which type members are available.
        /// See ClearScript documentation for more information.
        /// </summary>
        public HostItemFlags Flags { get; set; }

        /// <summary>
        /// The target object to inject into the JavaScript.
        /// </summary>
        public object Target { get; set; }
    }
}