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
using ClearScript.Manager.Extensions;
using ClearScript.Manager.Sql.Package;

namespace ClearScript.Manager.Sql.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class DbExecutor
    {
       
        public DbContext CreateDbContext(object options)
        {
            var op = options as SqlRequestOptions ?? new SqlRequestOptions((dynamic)options);
            if (string.IsNullOrEmpty(op.DbType))
            {
                op.DbType = "SqlServer";
            }

            if (string.IsNullOrEmpty(op.DbConnectionString))
            {
                throw new ArgumentNullException("DbConnectionString");
            }

            switch (op.DbType.ToLower())
            {
                case "sqlserver":
                case "mssql":
                    return new SqlServerDb(op.DbConnectionString);
                case "mysql":
                    return new MySqlServerDb(op.DbConnectionString);
            }

            throw new NotSupportedException(string.Format("dbType:{0} is not supported", op.DbType));
        }

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
    }
}