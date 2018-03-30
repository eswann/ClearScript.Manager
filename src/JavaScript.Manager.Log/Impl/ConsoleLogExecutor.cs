//-----------------------------------------------------------------------
// <copyright file="EmptyLogExecutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using JavaScript.Manager.Log.Interface;

namespace JavaScript.Manager.Log.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// Console的LOG执行
    /// </summary>
    public class ConsoleLogExecutor: ILogExecutor
    {
        
        public void Info(string msg, string trace = null)
        {
            this.log(msg, trace);
        }

        public void Warn(string msg, string trace = null)
        {
            this.log(msg, trace);
        }
        public void Error(string msg, string trace = null)
        {
            this.log(msg, trace);
        }
        public void Debug(string msg, string trace = null)
        {
            this.log(msg,trace);
        }
        private void log(string msg, string trace = null)
        {
            Console.WriteLine(msg + (trace??string.Empty));
        }
    }
}