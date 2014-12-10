using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace ClearScript.Manager.Http.Helpers.Node
{
    public class NodeHttpResponse : NodeBuffer
    {
        private NodeHttpRequest _nodeHttpRequest;
        private readonly Task<HttpResponseMessage> _resp;
        private bool _dataFired;
        private readonly Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public NodeHttpResponse(NodeHttpRequest nodeHttpRequest, Task<HttpResponseMessage> resp)
            : base(null)
        {
            // TODO: Complete member initialization
            _nodeHttpRequest = nodeHttpRequest;
            _resp = resp;

            if (!resp.IsFaulted)
            {
                this.statusCode = (int)resp.Result.StatusCode;

                this.headers = new PropertyBag();

                foreach (var kvp in resp.Result.Headers)
                {
                    this.headers[kvp.Key] = kvp.Value.FirstOrDefault();
                }

                if (resp.Result.Content != null)
                {
                    foreach (var kvp in resp.Result.Content.Headers)
                    {
                        this.headers[kvp.Key] = kvp.Value.FirstOrDefault();
                    }
                }
            }
        }

        public void InitEvents()
        {
            _resp.Result.Content.ReadAsStreamAsync().ContinueWith(x =>
            {
                this.InnerStream = x.Result;
                this.OnData();
            });
        }
        public object body { get; set; }
        internal HttpResponseMessage GetHttpResponseMessage()
        {
            return _resp.Result;
        }

        public void on(string eventName, dynamic callbackFn)
        {

            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<dynamic>();
            }

            events.Add(callbackFn);

            //close
            //readable
            //data
            //end
            //error
        }


        //todo .. rework as chunked
        private void OnData()
        {
            List<dynamic> listeners;
            if (_listeners.TryGetValue("data", out listeners))
            {

                listeners.ForEach(listener => listener.call(null, this));
            }
            if (_listeners.TryGetValue("end", out listeners))
            {

                listeners.ForEach(listener => listener.call(null));
            }
        }

        public string httpVersion { get; set; }

        public dynamic headers { get; set; }

        public int statusCode { get; set; }

        public byte[] read(int? size = null)
        {
            throw new NotImplementedException();
        }

        public void pipe(dynamic destinationStream, dynamic options = null)
        {
            throw new NotImplementedException();
        }

        public void unpipe(dynamic destinationStream = null)
        {
            throw new NotImplementedException();
        }

        public void unshift(dynamic chunk = null)
        {
            throw new NotImplementedException();
        }
    }
}