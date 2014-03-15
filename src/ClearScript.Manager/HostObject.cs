using Microsoft.ClearScript;

namespace ClearScript.Manager
{
    public class HostObject
    {
        public string Name { get; set; }

        public HostItemFlags Flags { get; set; }

        public object Target { get; set; }
    }
}