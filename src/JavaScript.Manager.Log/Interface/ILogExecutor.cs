//-----------------------------------------------------------------------
// <copyright file="IDbExecutor.cs" company="Company">
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
    /// 执行打log操作的接口定义
    /// </summary>
    public interface ILogExecutor
    {
        /// <summary>
        /// log
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="trace">错误的trace</param>
        void Info(string msg, string trace=null);
        void Warn(string msg, string trace=null);
        void Error(string msg, string trace=null);
        void Debug(string msg, string trace=null);
    }
}