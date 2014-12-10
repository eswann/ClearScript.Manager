using System;
using ClearScript.Manager.Http.Helpers.Node;

namespace ClearScript.Manager.Http
{
    public static class Extensions
    {


        private static readonly Type _nodeHttpType = typeof(NodeHttp);

        public static ExecutionOptions AddHttpHelperObjects(this ExecutionOptions options)
        {
            options.HostObjects.Add(new HostObject { Name = "http", Target = new NodeHttp() });
            options.Scripts.Add(new IncludeScript{Uri = @".\Scripts\request.js"});

            return options;
        }

        public static ExecutionOptions AddHttpHelperTypes(this ExecutionOptions options)
        {
            options.HostTypes.Add(new HostType { Name = "Http", Type = _nodeHttpType });
            options.Scripts.Add(new IncludeScript { Uri = @".\Scripts\request.js" });

            return options;
        }

        public static ExecutionOptions AddHttpHelpers(this ExecutionOptions options)
        {
            return options.AddHttpHelperObjects().AddHttpHelperTypes();
        }

    }
}