using System;
using System.Collections.Generic;

namespace JavaScript.Manager.Http.Helpers.Node
{
    public class NodeHttpResponse 
    {
        private readonly string _resp;
        private bool _dataFired;
        private readonly Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public NodeHttpResponse(NodeHttpRequest nodeHttpRequest, string resp = null)
           
        {
            _resp = resp;
            body = resp;
        }

        public void InitEvents()
        {
            try
            {
                OnData(_resp);

            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                OnError(ex.Message);
            }
        }
        public object body { get; set; }

      
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
        private void OnData(string body)
        {
            List<dynamic> listeners;
            if (_listeners.TryGetValue("end", out listeners))
            {
                listeners.ForEach(listener => listener.call(null, body));
            }
        }
        public void OnError(string error)
        {
            List<dynamic> listeners;
            if (_listeners.TryGetValue("err", out listeners))
            {
                listeners.ForEach(listener => listener.call(null, error));
            }
        }



    }
}