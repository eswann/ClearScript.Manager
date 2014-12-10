using System;
using System.Dynamic;

namespace ClearScript.Manager.Http.Helpers.Node
{
    public class NodeHttpRequestOptions
    {
        public NodeHttpRequestOptions(DynamicObject config)
        {
            host = config.GetMember<string>("host");
            scheme = config.GetMember("scheme", "http");
            hostname = config.GetMember<string>("hostname") ?? host;

            url = config.GetMember<object>("uri") ?? config.GetMember<object>("url");
            method = config.GetMember<string>("method");
            headers = config.GetMember<DynamicObject>("headers");

            port = config.GetMember("port", Convert.ToInt32, 80);
        }

        public string host
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public string hostname { get; set; }

        public int? port { get; set; }
        
        public string method { get; set; }
        
        public string path { get; set; }
        
        public DynamicObject headers { get; set; }



        public DynamicObject auth { get; set; }
        public DynamicObject agent { get; set; }

        public string scheme { get; set; }

        public dynamic uri
        {
            get { return url; }
            set { url = value; }
        }

        public dynamic url
        {
            get
            {
                if (hostname != null)
                {
                    try
                    {
                        return new UriBuilder(scheme, hostname, port.HasValue ? port.Value : 80, path).Uri;
                    }
                    catch
                    {
                    }
                }
                return null;
            }

            set
            {
                if (value != null)
                {
                    var uriString = value as string;
                    if (uriString != null)
                    {
                        value = new Uri(uriString);

                    }
                    var valAsUri = value as Uri;
                    if (valAsUri != null)
                    {
                        hostname = valAsUri.Host;
                        port = valAsUri.Port;
                        scheme = valAsUri.Scheme;
                        path = valAsUri.PathAndQuery;
                        return;
                    }

                    hostname = value.hostname;
                    port = value.port;
                    scheme = value.protocol;
                    path = value.pathname + value.search;

                }
            }
        }
    }
}