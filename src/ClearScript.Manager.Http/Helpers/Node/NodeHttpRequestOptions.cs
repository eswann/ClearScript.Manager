using System;
using System.Dynamic;
using JavaScript.Manager.Extensions;

namespace JavaScript.Manager.Http.Helpers.Node
{
    public class NodeHttpRequestOptions
    {
        public NodeHttpRequestOptions(DynamicObject config)
        {
            host = config.GetMember<string>("host");
            proxy = config.GetMember<string>("proxy");
            data = config.GetMember<string>("data");
            Accept = config.GetMember<string>("accept");
            scheme = config.GetMember("scheme", "http");
            hostname = config.GetMember<string>("hostname") ?? host;
            timeout = config.GetMember("timeout", Convert.ToInt32, 5);
            url = config.GetMember<string>("uri") ?? config.GetMember<string>("url");
            method = config.GetMember<string>("method");
            headers = config.GetMember<DynamicObject>("headers");

            port = config.GetMember("port", Convert.ToInt32, 80);
        }

        public string host
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public int timeout { get; set; }
        public string hostname { get; set; }
        public string Accept { get; set; }
        public string data { get; set; }
        public string proxy { get; set; }

        public int? port { get; set; }
        
        public string method { get; set; }
        
        public string path { get; set; }
        
        public DynamicObject headers { get; set; }



        public DynamicObject auth { get; set; }
        public DynamicObject agent { get; set; }

        public string scheme { get; set; }

        public string url { get; set; }


    }
}