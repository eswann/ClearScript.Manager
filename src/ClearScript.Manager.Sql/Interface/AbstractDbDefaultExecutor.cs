//-----------------------------------------------------------------------
// <copyright file="AbstractDbDefaultExecutor.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Data;
using System.Dynamic;
using System.Transactions;
using JavaScript.Manager.Extensions;

namespace JavaScript.Manager.Sql.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 抽象的DbExecutor的实现
    /// </summary>
    public abstract class AbstractDbDefaultExecutor : IDbExecutor
    {
        public abstract void Transaction(dynamic callback, dynamic options);
        public abstract DataTable ExecutorQuery(dynamic options);
        public abstract int ExecutorNonQuery(dynamic options);
        public abstract string ExecutorScalar(dynamic options);

        public void UseTransaction(dynamic callback, dynamic options)
        {
            if (callback == null)
            {
                //直接交给子类处理
                Transaction(callback, options);
                return;
            }

            if (options == null)
            {
                //直接交给子类处理
                Transaction(callback, options);
                return;
            }
            var _options = options as DynamicObject;
            bool skipBase = _options.GetMember<bool>("skipBase");
            if (skipBase)
            {
                //直接交给子类处理
                Transaction(callback, options);
            }

            //父类先处理 然后在交给子类处理
            var useTsOption = false;
            TransactionOptions transactionOption = new TransactionOptions();
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

            if (useTsOption)
            {
                using (TransactionScope scope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
                {
                    Transaction(callback, options);
                    scope.Complete();
                }
            }
            else
            {
                using (TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    Transaction(callback, options);
                    scope.Complete();
                }
            }
        }

        public List<List<object>> DbExecutorQuery(dynamic options)
        {
            var table = this.ExecutorQuery(options);
            if (table == null)
            {
                return new List<List<object>>();
            }
            List<List<Object>> obj = new List<List<object>>();
            if (table.Rows.Count == 1 && table.Columns.Count == 1)
            {
                obj.Add(new List<object> { new { key = "null_key_", value = table.Rows[0][0] } });
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
            return this.ExecutorNonQuery(options);
        }

        public string DbExecutorScalar(dynamic options)
        {
            return this.ExecutorScalar(options);
        }
    }
}