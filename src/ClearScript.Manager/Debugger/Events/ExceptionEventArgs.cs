namespace JavaScript.Manager.Debugger.Events
{
    using System;
    using JavaScript.Manager.Debugger.Messages;

    public sealed class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(EventResponse exceptionEvent)
        {
            ExceptionEvent = exceptionEvent;
        }

        public EventResponse ExceptionEvent { get; private set; }
    }
}