using System.Net.Http;

namespace ClearScript.Manager.Http.Helpers.Node
{
    
    public class NodeHttp
    {
        public NodeHttpRequest request(dynamic options, dynamic callback)
        {
            var req = new NodeHttpRequest(options, callback);
            return req;
        }
    }
}
