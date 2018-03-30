using JavaScript.Manager.Extensions;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace JavaScript.Manager.Http.Helpers.Node
{
    public class NodeHttpRequest
    {
        IWebProxy Proxy = null;
        private readonly NodeHttpRequestOptions _options;
        private readonly Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public NodeHttpRequest(object options, DynamicObject callback = null)
        {

            _options = options as NodeHttpRequestOptions ?? new NodeHttpRequestOptions((dynamic)options);
            if (!string.IsNullOrEmpty(_options.proxy))
            {

                try
                {
                    Proxy = new WebProxy(_options.proxy.ToLower().Contains("http://") ? _options.proxy : "http://" + _options.proxy, true);
                }
                catch (Exception)
                {

                }
            }


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


        public string end()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            List<dynamic> listeners;
            _listeners.TryGetValue("response", out listeners);
            try
            {
                request = (HttpWebRequest)WebRequest.Create(_options.url);
                request.Method = string.IsNullOrEmpty(_options.method) ? "GET" : _options.method;

                request.Timeout = _options.timeout * 1000;
                if (Proxy != null)
                {
                    request.Proxy = Proxy;
                }

                if (_options._CookieContainer != null)
                {
                    request.CookieContainer = _options._CookieContainer;
                }

                if (!string.IsNullOrEmpty(_options.Accept))
                {
                    request.ContentType = _options.Accept;
                }
                if (_options.url.StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                if (_options.headers != null)
                {
                    var header = (DynamicObject) _options.headers;
                    foreach (var kvp in header.GetDynamicProperties())
                    {
                        request.Headers[kvp.Key] = kvp.Value.ToString();
                    }
                }

                if (!request.Method.ToLower().Equals("get"))
                {
                    if (!string.IsNullOrEmpty(_options.data))
                    {
                        byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(_options.data);
                        request.ContentLength = data.Length;
                        Stream newStream = request.GetRequestStream();
                        newStream.Write(data, 0, data.Length);
                        newStream.Close();
                    }
                }
                response = (HttpWebResponse)request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var content = reader.ReadToEnd();
                    if (listeners != null)
                    {
                        var nodeResponse = new NodeHttpResponse(this, content);
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
                    return content;
                }


            }
            catch (Exception ex)
            {
                if (listeners != null)
                {
                    var nodeResponse = new NodeHttpResponse(this);
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
                    while (ex.InnerException != null) ex = ex.InnerException;
                    nodeResponse.OnError(ex.Message);
                }
                return ex.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        public void abort()
        {
            //do cancelation
        }


    }
}