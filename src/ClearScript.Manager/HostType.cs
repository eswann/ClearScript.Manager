using System;
using Microsoft.ClearScript;

namespace ClearScript.Manager
{
    public class HostType
    {
        public string Name { get; set; }

        public HostItemFlags Flags { get; set; }

        public HostTypeCollection HostTypeCollection { get; set; }

        public Type Type { get; set; }
    }
}