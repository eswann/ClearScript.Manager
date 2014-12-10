using System.Net.Http;

namespace ClearScript.Manager.Http.Helpers.Node
{
    
    public class NodeHttp
    {
        public NodeHttpRequest request(dynamic optoins, dynamic callback)
        {
            var client = new HttpClient();
            var requestMessage = new HttpRequestMessage();
            var req = new NodeHttpRequest(client, requestMessage, optoins, callback);
            return req;
        }
    }
}
