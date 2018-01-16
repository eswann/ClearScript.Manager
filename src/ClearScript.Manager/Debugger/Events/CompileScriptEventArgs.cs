namespace JavaScript.Manager.Debugger.Events
{
    using System;
    using JavaScript.Manager.Debugger.Messages;

    public sealed class CompileScriptEventArgs : EventArgs
    {
        public CompileScriptEventArgs(EventResponse compileScriptEvent)
        {
            CompileScriptEvent = compileScriptEvent;
        }

        public EventResponse CompileScriptEvent { get; private set; }
    }
}