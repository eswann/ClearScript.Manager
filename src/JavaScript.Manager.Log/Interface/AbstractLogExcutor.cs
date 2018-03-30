//-----------------------------------------------------------------------
// <copyright file="AbstractLogExcutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace JavaScript.Manager.Log.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 抽象的log执行器
    /// </summary>
    public abstract class AbstractLogExcutor: ILogExecutor
    {
        public abstract void LogInfo(string msg, string trace = null);
        public abstract void LogWarn(string msg, string trace = null);
        public abstract void LogError(string msg, string trace = null);
        public abstract void LogDebug(string msg, string trace = null);
        public void Info(string msg, string trace = null)
        {
           this.LogInfo(msg,trace);
        }

        public void Warn(string msg, string trace = null)
        {
            this.LogWarn(msg, trace);
        }

        public void Error(string msg, string trace = null)
        {
            this.LogError(msg, trace);
        }

        public void Debug(string msg, string trace = null)
        {
            this.LogDebug(msg, trace);
        }
    }
}