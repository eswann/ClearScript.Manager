using ClearScript.Manager.Http.Helpers.Node;
using ClearScript.Manager.Loaders;

namespace ClearScript.Manager.Http.Packages
{
    public class HttpPackage : RequiredPackage
    {
        public HttpPackage()
        {
            PackageId = "http";
            HostObjects.Add(new HostObject { Name = "http", Target = new NodeHttp() });
            HostTypes.Add(new HostType{ Name="Buffer", Type = typeof(NodeBuffer)});
        }

    }
}