using System;
using Microsoft.ClearScript;

namespace JavaScript.Manager
{
    /// <summary>
    /// A type to make available in JavaScript.
    /// </summary>
    public class HostType
    {
        /// <summary>
        /// Name of the type that is used to reference the type within the JavaScript.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// HostItemFlags to apply to the HostType.  Limits the scope of the type and which type members are available.
        /// See ClearScript documentation for more information.
        /// </summary>
        public HostItemFlags Flags { get; set; }

        /// <summary>
        /// Host type collections provide convenient scriptable access to all the types defined in one or more host assemblies. 
        /// They are hierarchical collections where leaf nodes represent types and parent nodes represent namespaces. 
        /// See ClearScript documentation for more information.
        /// </summary>
        public HostTypeCollection HostTypeCollection { get; set; }

        /// <summary>
        /// An individual type to make available to the script.
        /// </summary>
        public Type Type { get; set; }
    }
}