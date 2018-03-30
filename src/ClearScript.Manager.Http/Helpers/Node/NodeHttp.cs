
namespace JavaScript.Manager.Http.Helpers.Node
{
    
    public class NodeHttp
    {
        public NodeHttpRequest request(dynamic options, dynamic callback)
        {
            var req = new NodeHttpRequest(options, callback);
            return req;
        }

        public string getResult(dynamic options)
        {
            var req = new NodeHttpRequest(options);
            return req.end();
        }
    }
}
