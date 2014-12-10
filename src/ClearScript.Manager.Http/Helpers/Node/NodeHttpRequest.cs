using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace ClearScript.Manager.Http.Helpers.Node
{
    public class NodeHttpRequest
    {
        private readonly HttpRequestMessage _requestMessage;
        private readonly NodeHttpRequestOptions _options;
        private readonly HttpClient _client;
        private Task<HttpResponseMessage> _responseTask;
        private Stream _requestStream;
        //private DynamicObject _callback;
        private readonly Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();
        
        public NodeHttpRequest(HttpClient client, HttpRequestMessage requestMessage, object options, DynamicObject callback = null)
        {
            _client = client;
            _requestMessage = requestMessage;
            _options = options as NodeHttpRequestOptions ?? new NodeHttpRequestOptions((dynamic)options);

            if (callback != null)
            {
                @on("response", callback);
            }

        }
        public void on(string eventName, dynamic callbackFn)
        {
            //response
            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<dynamic>();
            }

            events.Add(callbackFn);
        }

        public void setHeader(string name, object value)
        {
            var headers = _options.headers = _options.headers ?? (dynamic)new PropertyBag();
            headers.AsDynamic()[name] = value;
        }

        public string getHeader(string name)
        {
            if (_options.headers == null)
            {
                return null;
            }
            return _options.headers.AsDynamic()[name] as string;
        }

        public void removeHeader(string name)
        {
            var headers = _options.headers = _options.headers ?? (dynamic)new PropertyBag();
            headers.AsDynamic().Remove(name);
        }


        public void write(string text, string encoding = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var enc = string.IsNullOrWhiteSpace(encoding) ? System.Text.Encoding.UTF8 : System.Text.Encoding.GetEncoding(encoding);
            var data = enc.GetBytes(text);
            write(data);
        }

        public void write(byte[] data, string encoding = null)
        {
            if (data != null)
            {
                if (_requestStream == null)
                {
                    _requestStream = new MemoryStream();
                    _requestMessage.Content = new StreamContent(_requestStream);
                }
                _requestStream.Write(data, 0, data.Length);
            }
        }

        public void end(byte[] data = null, string encoding = null)
        {
            write(data, encoding);
            var uriBuilder = new UriBuilder(_options.scheme, _options.hostname, _options.port.Value, _options.path).Uri;
            _requestMessage.RequestUri = uriBuilder;
            if (_options.headers != null)
            {
                foreach (var kvp in _options.headers.GetProperties())
                {
                    _requestMessage.Headers.TryAddWithoutValidation(kvp.Key, new[] { (kvp.Value ?? new object()).ToString() });
                }
            }
            //todo set up cancel optons
            _responseTask = _client.SendAsync(_requestMessage)
                .ContinueWith<HttpResponseMessage>(OnResponse, TaskContinuationOptions.NotOnFaulted);
        }

        public void abort()
        {
            //do cancelation
        }

        HttpResponseMessage OnResponse(Task<HttpResponseMessage> responseTask)
        {
            var resp = responseTask.Result;
            List<dynamic> listeners;
            if (_listeners.TryGetValue("response", out listeners))
            {
                var nodeResponse = new NodeHttpResponse(this, responseTask);
                listeners.ForEach(listener =>
                {
                    if (listener is Action<NodeHttpResponse>)
                    {
                        ((Action<NodeHttpResponse>)listener)(nodeResponse);
                    }
                    else
                    {
                        listener.call(null, nodeResponse);
                    }
                });
                nodeResponse.InitEvents();
            }
            return resp;
        }
    }
}