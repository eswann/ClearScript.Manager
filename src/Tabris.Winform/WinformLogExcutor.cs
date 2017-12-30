//-----------------------------------------------------------------------
// <copyright file="WinformLogExcutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using JavaScript.Manager.Log.Interface;

namespace Tabris.Winform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public enum LogLevel
    {
        [Description("&nbsp;&nbsp;&nbsp;&nbsp;INFO ")]
        INFO,
        [Description("&nbsp;&nbsp;&nbsp;WARN ")]
        WARN,
        [Description("&nbsp;&nbsp;ERROR ")]
        ERROR,
        [Description("&nbsp;&nbsp;DEBUG ")]
        DEBUG
    }

    /// <summary>
    /// 
    /// </summary>
    public class WinformLogExcutor : AbstractLogExcutor
    {
        private Action<LogLevel, string, string> logAction;
        public WinformLogExcutor(Action<LogLevel, string, string> _logAction)
        {
            logAction = _logAction;
        }
        public override void LogInfo(string msg, string trace = null)
        {
            logAction(LogLevel.INFO, msg, trace);
        }

        public override void LogWarn(string msg, string trace = null)
        {
            logAction(LogLevel.WARN, msg, trace);
        }

        public override void LogError(string msg, string trace = null)
        {
            logAction(LogLevel.ERROR, msg, trace);
        }

        public override void LogDebug(string msg, string trace = null)
        {
            logAction(LogLevel.DEBUG, msg, trace);
        }
    }
}