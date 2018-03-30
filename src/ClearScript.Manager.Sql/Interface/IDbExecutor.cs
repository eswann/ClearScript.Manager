//-----------------------------------------------------------------------
// <copyright file="IDbExecutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace JavaScript.Manager.Sql.Interface
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// 执行db操作的接口定义
    /// </summary>
    public interface IDbExecutor
    {
        /// <summary>
        /// 在Transaction下执行
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        void UseTransaction(dynamic callback, dynamic options);

        /// <summary>
        /// 执行查询sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="options"></param>
        /// <returns>返回db items</returns>
        List<List<Object>> DbExecutorQuery(string sql,dynamic options);

        /// <summary>
        /// 执行insert update delete
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="options"></param>
        /// <returns>返回影响的条数</returns>
        int DbExecutorNonQuery(string sql, dynamic options);

        /// <summary>
        /// 执行要拿到返回值 查询 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="options"></param>
        /// <returns>能拿到返回值</returns>
        string DbExecutorScalar(string sql, dynamic options);
    }
}