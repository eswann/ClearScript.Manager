using System;
using System.Dynamic;
using System.Net.Http;
using ClearScript.Manager.Extensions;

namespace ClearScript.Manager.Http.Helpers.Node
{
    public class NodeRequestModule
    {
        public static void MakeRequest(string url, dynamic callback)
        {
        }

        internal static void MakeRequest(DynamicObject config, DynamicObject callback, Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            var options = new NodeHttpRequestOptions(config);
            var uriObj = new Uri((config.GetMember<object>("uri") ?? config.GetMember<object>("url")).ToString());
            options.url = (config.GetMember<string>("uri") ?? config.GetMember<string>("url"));
            options.host = uriObj.Host;
            options.hostname = uriObj.Host;
            options.scheme = uriObj.Scheme;
            options.path = uriObj.PathAndQuery;
            options.port = uriObj.Port;
            options.method = config.GetMember("method", "GET");
            options.headers = config.GetMember<DynamicObject>("headers");
            bool isJson = config.GetMember("json", false);

            var req = new NodeHttpRequest(options);
            Action<NodeHttpResponse> wrapperCallback = resp =>
            {
                if (callback == null)
                {
                    return;
                }
                //    string body = null;
                object body = null;
                var apiResp = resp.body as string;
                if (isJson)
                {
                    var parser = (dynamic)engine.Evaluate("JSON.parse");
                    body = parser(apiResp);
                }
                else
                {
                    body = apiResp;
                }

                callback.AsDynamic().call(null, null, resp, body);
            };
            req.@on("response", wrapperCallback);

            req.end();
        }
    }
}