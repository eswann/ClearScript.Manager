using Microsoft.ClearScript.V8;

namespace JavaScript.Manager.Debugger
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public sealed class DebuggerConnection
    {
        private readonly Regex m_contentLength = new Regex(@"Content-Length: (\d+)", RegexOptions.Compiled);
        private readonly AsyncProducerConsumerCollection<string> m_messages;
        private INetworkClient m_networkClient;
        private readonly object m_networkClientLock = new object();

        public DebuggerConnection(string uri)
            : this(new Uri(uri))
        {
        }

        public int Port { get; set; }
        public DebuggerConnection(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (uri.IsAbsoluteUri == false)
                throw new ArgumentException("An absolute uri is required.", "uri");
            Port = uri.Port;
            switch (uri.Scheme)
            {
                case "tcp":
                    if (uri.Port < 0)
                    {
                        throw new ArgumentException("tcp:// URI must include port number", "uri");
                    }
                    m_networkClient = new TcpNetworkClient(uri.Host, uri.Port);
                    break;
                case "ws":
                case "wss":
                    m_networkClient = new WebSocketNetworkClient(uri);
                    break;
                default:
                    throw new ArgumentException("tcp://, ws:// or wss:// URI required", "uri");
            }

            m_messages = new AsyncProducerConsumerCollection<string>();
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Close()
        {
            lock (m_networkClientLock)
            {
                if (m_networkClient != null)
                {
                    m_networkClient.Dispose();
                    m_networkClient = null;
                }
            }
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">Message.</param>
        public void SendMessage(string message)
        {

            if (message == null)
                throw new ArgumentNullException("message");

            if (!Connected)
            {
                return;
            }

            DebugWriteLine("Request: " + message);

            //int byteCount = Encoding.UTF8.GetByteCount(message);
            //string request = string.Format("Content-Length: {0}{1}{1}{2}", byteCount, Environment.NewLine, message);

            m_messages.Add(message);
        }

        /// <summary>
        /// Fired when received inbound message.
        /// </summary>
        public Action<string> OutputMessage;

        /// <summary>
        /// Fired when connection was closed.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionClosed;

        /// <summary>
        /// Gets a value indicating whether connection established.
        /// </summary>
        public bool Connected
        {
            get
            {
                lock (m_networkClientLock)
                {
                    return m_networkClient != null && m_networkClient.Connected;
                }
            }
        }


        public string TargetId
        {
            get { return m_networkClient.TargetId; }
        }

        /// <summary>
        /// Initializes the connection.
        /// </summary>
        public void Connect()
        {
            //Close();

            //Task.Factory.StartNew(ReadStreamAsync);
            Task.Factory.StartNew(WriteStreamAsync);
        }

        /// <summary>
        /// Writes messages to debugger input stream.
        /// </summary>
        private async void WriteStreamAsync()
        {
            while (Connected)
            {
                var message = await m_messages.TakeAsync().ConfigureAwait(false);
                DebugWriteLine(message);
                V8DebugAgentFactory.SendCommand(Port,message);
            }


            //INetworkClient networkClient;
            //lock (m_networkClientLock)
            //{
            //    networkClient = m_networkClient;
            //}
            //if (networkClient == null)
            //{
            //    return;
            //}

            //try
            //{
            //    using (var streamWriter = new StreamWriter(networkClient.GetStream()))
            //    {
            //        while (Connected)
            //        {
            //            var message = await m_messages.TakeAsync().ConfigureAwait(false);
            //            DebugWriteLine(message);
            //            await streamWriter.WriteAsync(message).ConfigureAwait(false);
            //            await streamWriter.FlushAsync().ConfigureAwait(false);
            //        }
            //    }
            //}
            ////catch (SocketException)
            ////{
            ////}
            ////catch (ObjectDisposedException)
            ////{
            ////}
            ////catch (IOException)
            ////{
            ////}
            //catch (Exception e)
            //{
            //    DebugWriteLine(string.Format("DebuggerConnection: failed to write message {0}.", e));
            //    throw;
            //}
        }

        /// <summary>
        /// Reads data from debugger output stream.
        /// </summary>
        private async void ReadStreamAsync()
        {
            
            //DebugWriteLine("DebuggerConnection: established connection.");

            //INetworkClient networkClient;
            //lock (m_networkClientLock)
            //{
            //    networkClient = m_networkClient;
            //}
            //if (networkClient == null)
            //{
            //    return;
            //}

            //try
            //{
            //    using (var streamReader = new StreamReader(networkClient.GetStream(), Encoding.Default))
            //    {
            //        DebugWriteLine(Connected?"connected":"not connectd");
            //        while (Connected)
            //        {
            //            // Read message header
            //            var result = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            //            if(string.IsNullOrEmpty(result)) continue;

            //            DebugWriteLine("Response: " + result);

            //            // Notify subscribers
            //            EventHandler<MessageEventArgs> outputMessage = OutputMessage;
            //            if (outputMessage != null)
            //            {
            //                outputMessage(this, new MessageEventArgs(result));
            //            }
            //        }
            //    }
            //}
            //catch (SocketException)
            //{
            //}
            //catch (ObjectDisposedException)
            //{
            //}
            //catch (IOException)
            //{
            //}
            //catch (Exception e)
            //{
            //    DebugWriteLine(string.Format("DebuggerConnection: message processing failed {0}.", e));
            //    throw;
            //}
            //finally
            //{
            //    DebugWriteLine("DebuggerConnection: connection was closed.");

            //    EventHandler<EventArgs> connectionClosed = ConnectionClosed;
            //    if (connectionClosed != null)
            //    {
            //        connectionClosed(this, EventArgs.Empty);
            //    }
            //}
        }

        [Conditional("DEBUG")]
        private void DebugWriteLine(string message)
        {
            Debug.WriteLine("[{0}] {1}", DateTime.UtcNow.TimeOfDay, message);
        }
    }
}