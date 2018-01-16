namespace JavaScript.Manager.Debugger.Events
{
    using JavaScript.Manager.Debugger.Messages;
    using System;

    public sealed class BreakpointEventArgs : EventArgs
    {
        public BreakpointEventArgs(EventResponse breakpointEvent)
        {
            BreakpointEvent = breakpointEvent;
        }

        public EventResponse BreakpointEvent { get; private set; }
    }
}