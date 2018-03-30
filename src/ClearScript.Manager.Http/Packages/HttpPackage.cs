﻿using JavaScript.Manager.Http.Helpers.Node;
using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.Http.Packages
{
    public class HttpPackage : RequiredPackage
    {
        public HttpPackage()
        {
            PackageId = "javascript_request_factory_http";
            HostObjects.Add(new HostObject { Name = "javascript_request_factory_http", Target = new NodeHttp() });
            //HostTypes.Add(new HostType{ Name="Buffer", Type = typeof(NodeBuffer)});
        }

    }
}