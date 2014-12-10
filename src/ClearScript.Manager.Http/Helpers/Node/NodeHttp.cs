using System.Net.Http;

namespace ClearScript.Manager.Http.Helpers.Node
{
    
    public class NodeHttp
    {
        public NodeHttpRequest request(dynamic options, dynamic callback)
        {
            var client = new HttpClient();
            var requestMessage = new HttpRequestMessage();
            var req = new NodeHttpRequest(client, requestMessage, options, callback);
            return req;
        }
    }
}
