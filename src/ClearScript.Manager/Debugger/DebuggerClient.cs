namespace JavaScript.Manager.Debugger
{
    using JavaScript.Manager.Debugger.Events;
    using JavaScript.Manager.Debugger.Messages;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class DebuggerClient
    {
        private readonly DebuggerConnection m_connection;

        private ConcurrentDictionary<int, TaskCompletionSource<Response>> m_messages =
            new ConcurrentDictionary<int, TaskCompletionSource<Response>>();

        private int m_currentSequence = 1;

        public DebuggerClient(DebuggerConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            m_connection = connection;
           
            m_connection.ConnectionClosed += OnConnectionClosed;
        }

        /// <summary>
        /// Sends a request and returns a response.
        /// </summary>
        /// <param name="request">Command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<Response> SendRequestAsync(Request request, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            request.Sequence = m_currentSequence++;

            try
            {
                var promise = m_messages.GetOrAdd(request.Sequence, i => new TaskCompletionSource<Response>());

                var serializedRequest = JsonConvert.SerializeObject(request);

                m_connection.SendMessage(serializedRequest);
                cancellationToken.ThrowIfCancellationRequested();

                Debug.WriteLine("await promise.Task.ConfigureAwait(false)");
                var response = await promise.Task.ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                Debug.WriteLine("await promise. end");
                return response;
            }
            finally
            {
                TaskCompletionSource<Response> promise;
                m_messages.TryRemove(request.Sequence, out promise);
            }
        }

        public async Task<Response> SendRequestAsync(string method, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new {id = m_currentSequence++, method = method};

            try
            {
                var promise = m_messages.GetOrAdd(request.id, i => new TaskCompletionSource<Response>());

                var serializedRequest = JsonConvert.SerializeObject(request);

                m_connection.SendMessage(serializedRequest);
                cancellationToken.ThrowIfCancellationRequested();

                Debug.WriteLine("await promise.Task.ConfigureAwait(false)");
                var response = await promise.Task.ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                Debug.WriteLine("await promise. end");
                return response;
            }
            finally
            {
                TaskCompletionSource<Response> promise;
                m_messages.TryRemove(request.id, out promise);
            }
        }

        /// <summary>
        /// Break point event handler.
        /// </summary>
        public event EventHandler<BreakpointEventArgs> BreakpointEvent;

        /// <summary>
        /// Compile script event handler.
        /// </summary>
        public event EventHandler<CompileScriptEventArgs> CompileScriptEvent;

        /// <summary>
        /// Exception event handler.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionEvent;

        /// <summary>
        /// Handles disconnect from debugger.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnConnectionClosed(object sender, EventArgs e)
        {
            var messages = Interlocked.Exchange(ref m_messages, new ConcurrentDictionary<int, TaskCompletionSource<Response>>());
            foreach (var kv in messages)
            {
                var exception = new IOException("Debugger connection closed.");
                kv.Value.SetException(exception);
            }

            messages.Clear();
        }

        /// <summary>
        /// Process message from debugger connection.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Event arguments.</param>
        public void OnOutputMessage(object sender, MessageEventArgs args)
        {
            try
            {
                Debug.WriteLine(args.Message);

                var response = JsonConvert.DeserializeObject<Response>(args.Message);
                HandleResponseMessage(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //var message = JObject.Parse(args.Message);
            //var messageType = (string)message["type"];

            //switch (messageType)
            //{
            //    case "event":
            //        var eventResponse = JsonConvert.DeserializeObject<EventResponse>(args.Message);
            //        HandleEventMessage(eventResponse);
            //        break;

            //    case "response":
            //        var response = JsonConvert.DeserializeObject<Response>(args.Message);
            //        HandleResponseMessage(response);
            //        break;

            //    default:
            //        Debug.Fail(string.Format("Unrecognized type '{0}' in message: {1}", messageType, message));
            //        break;
            //}
        }

        /// <summary>
        /// Handles event message.
        /// </summary>
        /// <param name="eventResponse">Event Response.</param>
        private void HandleEventMessage(EventResponse eventResponse)
        {
            var eventType = eventResponse.Event;
            switch (eventType)
            {
                case "afterCompile":
                    var compileScriptHandler = CompileScriptEvent;
                    if (compileScriptHandler != null)
                    {
                        //var compileScriptEvent = new CompileScriptEvent(message);
                        //compileScriptHandler(this, new CompileScriptEventArgs(compileScriptEvent));
                    }
                    break;

                case "break":
                    var breakpointHandler = BreakpointEvent;
                    if (breakpointHandler != null)
                    {
                        breakpointHandler(this, new BreakpointEventArgs(eventResponse));
                    }
                    break;

                case "exception":
                    var exceptionHandler = ExceptionEvent;
                    if (exceptionHandler != null)
                    {
                        //var exceptionEvent = new ExceptionEvent(message);
                        //exceptionHandler(this, new ExceptionEventArgs(exceptionEvent));
                    }
                    break;

                case "beforeCompile":
                case "breakForCommand":
                case "newFunction":
                case "scriptCollected":
                    break;

                default:
                    Debug.Fail(string.Format("Unrecognized type '{0}' in event message: {1}", eventType, eventResponse.Body));
                    break;
            }
        }

        /// <summary>
        /// Handles the response
        /// </summary>
        /// <param name="response">Response.</param>
        private void HandleResponseMessage(Response response)
        {
            try
            {
                TaskCompletionSource<Response> promise;
                var messageId = response.Sequence;

                if (m_messages.TryGetValue(messageId, out promise))
                {
                    promise.SetResult(response);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}