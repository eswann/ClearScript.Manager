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
            options.url = (config.GetMember<object>("uri") ?? config.GetMember<object>("url"));
            options.host = uriObj.Host;
            options.hostname = uriObj.Host;
            options.scheme = uriObj.Scheme;
            options.path = uriObj.PathAndQuery;
            options.port = uriObj.Port;
            options.method = config.GetMember("method", "GET");
            options.headers = config.GetMember<DynamicObject>("headers");
            bool isJson = config.GetMember("json", false);

            var req = new NodeHttpRequest(new HttpClient(), new HttpRequestMessage(), options);
            Action<NodeHttpResponse> wrapperCallback = resp =>
            {
                if (callback == null)
                {
                    return;
                }
                //    string body = null;
                object body = null;
                var apiResp = resp.GetHttpResponseMessage();
                if (apiResp.Content != null && apiResp.Content.Headers.ContentLength > 0)
                {
                    if (isJson)
                    {
                        string xxx = apiResp.Content.ReadAsStringAsync().Result;
                        var parser = (dynamic)engine.Evaluate("JSON.parse");
                        body = parser(xxx);
                    }
                    else
                    {
                        body = apiResp.Content.ReadAsStringAsync().Result;
                    }
                }

                callback.AsDynamic().call(null, null, resp, body);
            };
            req.@on("response", wrapperCallback);

            req.end();
        }
    }
}