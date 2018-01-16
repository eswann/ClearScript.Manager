using System.Diagnostics;
using System.Net;
using System.Text;

namespace JavaScript.Manager.Debugger
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Threading;

    public sealed class WebSocketNetworkClient : INetworkClient
    {
        private readonly ClientWebSocket m_webSocket;
        private readonly WebSocketStream m_stream;


        public string TargetId { get; set; }

        public WebSocketNetworkClient(Uri uri)
        {
            try
            {
                var httpRequest = WebRequest.Create(uri.OriginalString.Replace("ws","http") + "/json");
                httpRequest.Method = WebRequestMethods.Http.Get;
                httpRequest.Timeout = 5000;
                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var re = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(re))
                        {
                            TargetId = re.Split(new string[] {"id\": \""}, StringSplitOptions.None)[1].Split('\"')[0];
                            Debug.WriteLine("targetId:" + TargetId);
                        }
                    }
                }
            }
            catch (WebException)
            {
                // If it fails or times out, just go ahead and try to connect anyway, and rely on normal error reporting path.
            }

            try
            {
                m_webSocket = new ClientWebSocket();
                m_webSocket.ConnectAsync(new Uri(uri.OriginalString + "/" + TargetId), CancellationToken.None).GetAwaiter().GetResult();
                m_stream = new WebSocketStream(m_webSocket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public bool Connected
        {
            get { return m_webSocket.State == WebSocketState.Open; }
        }

        public void Dispose()
        {
            try
            {
                m_webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                m_webSocket.Dispose();
            }
            catch (WebSocketException)
            {
                // We don't care about any errors when cleaning up and closing connection.
            }
        }

        public Stream GetStream()
        {
            return m_stream;
        }
    }
}