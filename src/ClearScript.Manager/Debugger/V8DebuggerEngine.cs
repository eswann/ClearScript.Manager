using System.Diagnostics;
using System.Linq;

namespace JavaScript.Manager.Debugger
{
    using JavaScript.Manager.Debugger.Events;
    using JavaScript.Manager.Debugger.Messages;
    using Microsoft.ClearScript.V8;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class V8Debugger : IV8Debugger
    {
        public event EventHandler<MessageEventArgs> OutputMessage;
        public void OnDebugMessage(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new MessageEventArgs(message));
            }
        }
    }

    public class V8DebuggerEngine 
    {
        public event EventHandler<ExceptionEventArgs> ExceptionEvent;
        public event EventHandler<BreakpointEventArgs> BreakpointEvent;

        private readonly V8ScriptEngine m_scriptEngine;
        private DebuggerConnection m_debuggerConnection;
        private DebuggerClient m_debuggerClient;
        private IV8Debugger v8Debugger;
        private string m_currentScriptName;
        private readonly List<Breakpoint> m_breakpoints = new List<Breakpoint>(); 

        public V8DebuggerEngine( V8ScriptEngine engine,int debuggingPort)
        {
            v8Debugger = new V8Debugger();
            m_scriptEngine = engine;

            //Create the connection to the debug port.
            m_debuggerConnection = new DebuggerConnection("ws://127.0.0.1:" + debuggingPort);
            m_debuggerConnection.Connect();
            m_debuggerClient = new DebuggerClient(m_debuggerConnection);
            ((V8Debugger)v8Debugger).OutputMessage += m_debuggerClient.OnOutputMessage;
            m_debuggerClient.ExceptionEvent += debuggerClient_ExceptionEvent;
            m_debuggerClient.BreakpointEvent += m_debuggerClient_BreakpointEvent;
            m_currentScriptName = GetRandomScriptTargetName();

            
            V8DebugAgentFactory.AddV8Debugger(debuggingPort, v8Debugger);

            InitDebugger().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public string CurrentScriptName {
            get
            {
                return m_currentScriptName + " [temp]";
            }
        }

        private string GetRandomScriptTargetName()
        {
            return TokenGenerator.GetUniqueKey(10);
        }


        private async Task InitDebugger()
        {
            await m_debuggerClient.SendRequestAsync("Profiler.enable").ConfigureAwait(false);
            await m_debuggerClient.SendRequestAsync("Runtime.enable").ConfigureAwait(false);
            await m_debuggerClient.SendRequestAsync("Debugger.enable").ConfigureAwait(false);
            //var backtrace = new Request("Debugger.setPauseOnExceptions");
            //backtrace.Arguments = new {state = "none"};
            // await m_debuggerClient.SendRequestAsync(backtrace).ConfigureAwait(false);
            var backtrace = new Request("Debugger.setAsyncCallStackDepth");
            backtrace.Arguments = new { maxDepth = 32 };
            await m_debuggerClient.SendRequestAsync(backtrace).ConfigureAwait(false);
            backtrace = new Request("Debugger.setBlackboxPatterns");
            backtrace.Arguments = new { patterns = new List<string>() };
            await m_debuggerClient.SendRequestAsync(backtrace).ConfigureAwait(false);
            await m_debuggerClient.SendRequestAsync("Runtime.runIfWaitingForDebugger").ConfigureAwait(false);
        }

        /// <summary>
        /// The request backtrace returns a backtrace (or stacktrace) from the current execution state.
        /// </summary>
        /// <remarks>
        /// When issuing a request a range of frames can be supplied. The top frame is frame number 0. If no frame range is supplied data for 10 frames will be returned.
        /// </remarks>
        /// <param name="fromFrame"></param>
        /// <param name="toFrame"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        public async Task<Response> Backtrace(int? fromFrame = null, int? toFrame = null, bool? bottom = null)
        {
            var backtrace = new Request("backtrace");
            var backtraceResponse = await m_debuggerClient.SendRequestAsync(backtrace);
            return backtraceResponse;
        }

     

        /// <summary>
        /// The request clearbreakpoint clears a break point.
        /// </summary>
        /// <param name="breakpointNumber">number of the break point to clear</param>
        /// <returns></returns>
        public async Task<Response> ClearBreakpoint(string breakpointNumber)
        {
            var clearBreakpointRequest = new Request("Debugger.removeBreakpoint");
            clearBreakpointRequest.Arguments = new {breakpointId = breakpointNumber};
            var clearBreakpointResponse = await m_debuggerClient.SendRequestAsync(clearBreakpointRequest).ConfigureAwait(false);
            return clearBreakpointResponse;
        }

        /// <summary>
        /// The request continue is a request from the debugger to start the VM running again. As part of the continue request the debugger can specify if it wants the VM to perform a single step action.
        /// </summary>
        /// <param name="stepAction"></param>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public async Task<Response> Continue(StepAction stepAction = StepAction.Next, int? stepCount = null)
        {
            var continueRequest = new Request("continue");

            var continueResponse = await m_debuggerClient.SendRequestAsync(continueRequest);
            return continueResponse;
        }

        /// <summary>
        /// The request disconnect is used to detach the remote debugger from the debuggee.
        /// </summary>
        /// <remarks>
        /// This will trigger the debuggee to disable all active breakpoints and resumes execution if the debuggee was previously stopped at a break.
        /// </remarks>
        /// <returns></returns>
        public async Task<Response> Disconnect()
        {
            var disconnectRequest = new Request("disconnect");

            var disconnectResponse = await m_debuggerClient.SendRequestAsync(disconnectRequest);
            return disconnectResponse;
        }


        /// <summary>
        /// The request evaluate is used to evaluate an expression. The body of the result is as described in response object serialization below.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="disableBreak"></param>
        /// <returns></returns>
        public async Task<Response> EvalImmediate(string expression, bool disableBreak = false)
        {
            var evaluateRequest = new Request("evaluate");
            var evalResponse = await m_debuggerClient.SendRequestAsync(evaluateRequest);
            return evalResponse;
        }

        /// <summary>
        /// The request frame selects a new selected frame and returns information for that. If no frame number is specified the selected frame is returned.
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public async Task<Response> Frame(int? frameNumber)
        {
            var frameRequest = new Request("frame");

            var frameResponse = await m_debuggerClient.SendRequestAsync(frameRequest);
            return frameResponse;
        }

        /// <summary>
        /// The request gc is a request to run the garbage collector in the debuggee.
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GarbageCollect()
        {
            var gcRequest = new Request("gc");

            var gcResponse = await m_debuggerClient.SendRequestAsync(gcRequest);
            return gcResponse;
        }

        /// <summary>
        /// The request listbreakpoints is used to get information on breakpoints that may have been set by the debugger.
        /// </summary>
        /// <returns></returns>
        public async Task<Response> ListAllBreakpoints()
        {
            var listBreakpointsRequest = new Request("listbreakpoints");

            var listBreakpointsResponse = await m_debuggerClient.SendRequestAsync(listBreakpointsRequest);
            return listBreakpointsResponse;
        }

        /// <summary>
        /// Stops the script engine at the current point.
        /// </summary>
        /// <returns></returns>
        public async Task Interrupt()
        {
            m_scriptEngine.Interrupt();

            await ResetDebuggerScriptEngine();
        }

        /// <summary>
        /// The request lookup is used to lookup objects based on their handle.
        /// </summary>
        /// <remarks>
        /// The individual array elements of the body of the result is as described in response object serialization below.
        /// </remarks>
        /// <param name="includeSource"></param>
        /// <param name="handles"></param>
        /// <returns></returns>
        public async Task<Response> Lookup(bool includeSource = false, params int[] handles)
        {
            var lookupRequest = new Request("lookup");
            var response = await m_debuggerClient.SendRequestAsync(lookupRequest);
            return response;
        }

        /// <summary>
        /// The request scope returns information on a given scope for a given frame. If no frame number is specified the selected frame is used.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public async Task<Response> Scope(int number, int? frameNumber = null)
        {
            var scopeRequest = new Request("scope");
            var response = await m_debuggerClient.SendRequestAsync(scopeRequest);
            return response;
        }

        /// <summary>
        /// The request scopes returns all the scopes for a given frame. If no frame number is specified the selected frame is returned.
        /// </summary>
        /// <param name="frameNumber"></param>
        /// <returns></returns>
        public async Task<Response> Scopes(int? frameNumber = null)
        {
            var scopesRequest = new Request("scopes");

            var response = await m_debuggerClient.SendRequestAsync(scopesRequest);
            return response;
        }

        /// <summary>
        /// The request scripts retrieves active scripts from the VM.
        /// </summary>
        /// <remarks>
        /// An active script is source code from which there is still live objects in the VM. This request will always force a full garbage collection in the VM.
        /// </remarks>
        /// <param name="types">types of scripts to retrieve</param>
        /// <param name="ids">array of id's of scripts to return. If this is not specified all scripts are requrned</param>
        /// <param name="includeSource">boolean indicating whether the source code should be included for the scripts returned</param>
        /// <param name="filter">filter string or script id.</param>
        /// <returns></returns>
        public async Task<Response> Scripts(ScriptType? types = null, int[] ids = null, bool includeSource = false,
            string filter = null)
        {
            var scriptsRequest = new Request("scripts");

            var response = await m_debuggerClient.SendRequestAsync(scriptsRequest);
            return response;
        }

        /// <summary>
        /// The request source retrieves source code for a frame.
        /// </summary>
        /// <remarks>
        /// It returns a number of source lines running from the fromLine to but not including the toLine,
        /// that is the interval is open on the "to" end. For example,
        /// requesting source from line 2 to 4 returns two lines (2 and 3).
        /// Also note that the line numbers are 0 based: the first line is line 0.
        /// </remarks>
        /// <param name="frameNumber">frame number (default selected frame)</param>
        /// <param name="fromLine">from line within the source default is line 0</param>
        /// <param name="toLine">to line within the source this line is not included in the result default is the number of lines in the script</param>
        /// <returns></returns>
        public async Task<Response> Source(int? frameNumber = null, int? fromLine = null, int? toLine = null)
        {
            var sourceRequest = new Request("source");

            var response = await m_debuggerClient.SendRequestAsync(sourceRequest);
            return response;
        }

        /// <summary>
        /// The request setbreakpoint creates a new break point.
        /// </summary>
        /// <remarks>
        /// This request can be used to set both function and script break points. A function break point sets a break point in an existing function whereas a script break point sets a break point in a named script. A script break point can be set even if the named script is not found.
        /// </remarks>
        /// <param name="breakpoint"></param>
        /// <returns></returns>
        public async Task<bool> SetBreakpoint(Breakpoint breakpoint)
        {
            try
            {
                breakpoint.TargetId = "V8Runtime:" + m_debuggerConnection.TargetId;
                var existBreak = m_breakpoints.FirstOrDefault(r => r.TargetId.Equals(breakpoint.TargetId));
                if (existBreak!= null)
                {
                    //清除
                    var re = await ClearBreakpoint(existBreak.BreakPointNumber).ConfigureAwait(false);
                    m_breakpoints.Remove(existBreak);
                    return true;
                }


                var response = await SetBreakpointInternal(breakpoint).ConfigureAwait(false);
                if (response != null && !string.IsNullOrEmpty(response.Result.breakpointId.Value.ToString()))
                {
                    var p = response.Result.breakpointId.Value.ToString();
                    breakpoint.BreakPointNumber = p;
                    m_breakpoints.Add(breakpoint);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        private async Task<Response> SetBreakpointInternal(Breakpoint breakpoint)
        {
            var breakPointRequest = new Request("Debugger.setBreakpointByUrl")
            {
                Arguments = breakpoint
            };

            var breakPointResponse = await m_debuggerClient.SendRequestAsync(breakPointRequest).ConfigureAwait(false);
            return breakPointResponse;
        }

        public async Task ResetDebuggerScriptEngine()
        {
           
        }

        private void m_debuggerClient_BreakpointEvent(object sender, BreakpointEventArgs e)
        {
            if (ExceptionEvent != null)
                BreakpointEvent(sender, new BreakpointEventArgs(e.BreakpointEvent));
        }

        private void debuggerClient_ExceptionEvent(object sender, ExceptionEventArgs e)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(sender, new ExceptionEventArgs(e.ExceptionEvent));
        }


        public void Dispose()
        {
            if (m_debuggerClient != null)
            {
                m_debuggerClient.ExceptionEvent -= debuggerClient_ExceptionEvent;
                m_debuggerClient.BreakpointEvent -= m_debuggerClient_BreakpointEvent;
                m_debuggerClient = null;
            }

            if (m_debuggerConnection != null)
            {
                m_debuggerConnection.Close();
                m_debuggerConnection.Dispose();
                m_debuggerConnection = null;
            }

        }
    }
}
