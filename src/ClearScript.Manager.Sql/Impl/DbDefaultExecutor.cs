//-----------------------------------------------------------------------
// <copyright file="DbExecutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Data;
using System.Dynamic;
using System.Transactions;
using AntData.ORM.Data;
using JavaScript.Manager.Extensions;
using JavaScript.Manager.Sql.Interface;
using JavaScript.Manager.Sql.Package;

namespace JavaScript.Manager.Sql.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// AntORM框架的执行
    /// </summary>
    public class DbDefaultExecutor: IDbExecutor
    {

        /// <summary>
        /// 在Transaction下执行
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public void UseTransaction(dynamic callback, dynamic options)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            var useTsOption = false;
            TransactionOptions transactionOption = new TransactionOptions(); ;
            if (options != null)
            {
                var _options = options as DynamicObject;
                string level = _options.GetMember<string>("level");
                if (!string.IsNullOrEmpty(level))
                {
                    switch (level.ToLower())
                    {
                        case "chaos":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.Chaos;
                            break;
                        case "readcommitted":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                            break;
                        case "readuncommitted":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                            break;
                        case "repeatableread":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
                            break;
                        case "serializable":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.Serializable;
                            break;
                        case "snapshot":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                            break;
                        case "unspecified":
                            useTsOption = true;
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.Unspecified;
                            break;
                    }
                }

                var timeout = _options.GetMember<int>("timeout");
                if (timeout > 0)
                {
                    useTsOption = true;
                    transactionOption.Timeout = TimeSpan.FromSeconds(timeout);
                }
            }

            if (useTsOption)
            {
                using (TransactionScope scope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
                {
                    callback.call(null);
                    scope.Complete();
                }
            }
            else
            {
                using (TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    callback.call(null);
                    scope.Complete();
                }

            }
            
        }

        /// <summary>
        /// 执行查询sql
        /// </summary>
        /// <param name="options"></param>
        /// <returns>返回db items</returns>
        public List<List<Object>> DbExecutorQuery(dynamic options)
        {
            var _options = options as DynamicObject;
            var sql = _options.GetMember<string>("sql");
            var timeout = _options.GetMember<int>("timeout");

            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            var dbContext = CreateDbContext(options);
            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }

            DataTable table = dbContext.ExecuteDataTable(sql, new Dictionary<string, AntData.ORM.Common.CustomerParam>());

            List<List<Object>> obj = new List<List<object>>();
            if (table.Rows.Count == 1 && table.Columns.Count == 1)
            {
                 obj.Add(new List<object> { new { key = "null_key_" , value = table.Rows[0][0] } } );
                return obj;
            }
            
            foreach (var dataRow in table.AsEnumerable())
            {
                List<Object> l = new List<object>();
                foreach (var dataColumn in table.Columns.Cast<DataColumn>())
                {
                    Object a = new { key = dataColumn.ColumnName, value = dataRow[dataColumn] };
                    l.Add(a);
                }
                obj.Add(l);
            }
         
            return obj;
        }

        /// <summary>
        /// 执行insert update delete
        /// </summary>
        /// <param name="options"></param>
        /// <returns>返回影响的条数</returns>
        public int DbExecutorNonQuery(dynamic options)
        {
            var _options = options as DynamicObject;
            var sql = _options.GetMember<string>("sql");
            var timeout = _options.GetMember<int>("timeout");

            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            var dbContext = CreateDbContext(options);

            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }

            return dbContext.ExecuteNonQuery(sql, new Dictionary<string, AntData.ORM.Common.CustomerParam>());
        }

        /// <summary>
        /// 执行要拿到返回值 查询 
        /// </summary>
        /// <param name="options"></param>
        /// <returns>能拿到返回值</returns>
        public string DbExecutorScalar(dynamic options)
        {
            var _options = options as DynamicObject;
            var sql = _options.GetMember<string>("sql");
            var timeout = _options.GetMember<int>("timeout");

            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            var dbContext = CreateDbContext(options);

            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }
            return dbContext.ExecuteScalar(sql, new Dictionary<string, AntData.ORM.Common.CustomerParam>()).ToString();
        }

        #region Private
        /// <summary>
        /// 创建DB执行Context
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private DbContext CreateDbContext(DynamicObject options)
        {

            var DbType = options.GetMember<string>("dbType", "SqlServer");
            var DbConnectionString = options.GetMember<string>("connectionString");
            if (string.IsNullOrEmpty(DbConnectionString))
            {
                throw new ArgumentNullException("DbConnectionString");
            }

            switch (DbType.ToLower())
            {
                case "sqlserver":
                case "mssql":
                    return new SqlServerDb(DbConnectionString);
                case "mysql":
                    return new MySqlServerDb(DbConnectionString);
            }

            throw new NotSupportedException(string.Format("dbType:{0} is not supported", DbType));
        }
        #endregion Private
    }
}