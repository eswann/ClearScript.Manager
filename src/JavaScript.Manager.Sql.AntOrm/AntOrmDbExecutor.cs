using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.SqlServer;
using JavaScript.Manager.Extensions;
using JavaScript.Manager.Sql.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace JavaScript.Manager.Sql.AntOrm
{
    /// <summary>
    /// AntOrm框架的 sqlserver执行Context
    /// </summary>
    public class SqlServerDb : DbContext
    {
        private static readonly IDataProvider _provider = new SqlServerDataProvider(SqlServerVersion.v2008);
        public SqlServerDb(string dbMappingName) : base(dbMappingName)
        {

        }

        protected override IDataProvider provider
        {
            get { return _provider; }
        }
    }

    /// <summary>
    /// AntOrm框架的 mysql执行Context
    /// </summary>
    public class MySqlServerDb : DbContext
    {
        private static readonly IDataProvider _provider = new MySqlDataProvider();
        public MySqlServerDb(string dbMappingName) : base(dbMappingName)
        {
        }

        protected override IDataProvider provider
        {
            get { return _provider; }
        }
    }

    /// <summary>
    /// AntOrm执行
    /// </summary>
    public class AntOrmDbExecutor : AbstractDbDefaultExecutor
    {
        public override void Transaction(dynamic callback, dynamic options)
        {

        }

        public override DataTable ExecutorQuery(string sql, dynamic options)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            var _options = options as DynamicObject;
            var timeout = _options.GetMember<int>("timeout");


            DbContext dbContext = CreateDbContext(options);
            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }

            DataParameter[] pList = GetDataParameter(options);
            if (pList != null && pList.Length > 0)
            {
                return dbContext.QueryTable(sql, pList);
            }
            return dbContext.ExecuteDataTable(sql, new Dictionary<string, AntData.ORM.Common.CustomerParam>());
        }

        public override int ExecutorNonQuery(string sql, dynamic options)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            var _options = options as DynamicObject;
            var timeout = _options.GetMember<int>("timeout");


            DbContext dbContext = CreateDbContext(options);

            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }

            DataParameter[] pList = GetDataParameter(options);
            if (pList != null && pList.Length > 0)
            {
                return dbContext.ExecuteNonQuery(sql, pList);
            }
            return dbContext.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 拿到主键值
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public override string ExecutorScalar(string sql, dynamic options)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }
            var _options = options as DynamicObject;
            var timeout = _options.GetMember<int>("timeout");


            sql = sql.TrimStart();

            DbContext dbContext = CreateDbContext(options);
            var sqlLower = sql.ToLower();
            if (sqlLower.StartsWith("insert"))
            {
                if (dbContext is SqlServerDb)
                {
                    if (!sqlLower.Contains("select scope_identity()"))
                    {
                        sql += sqlLower.EndsWith(";") ? "select scope_identity()" : ";select scope_identity()";
                    }
                }
                else
                {
                    if (!sqlLower.Contains("select last_insert_id()"))
                    {
                        sql += sqlLower.EndsWith(";") ? "select last_insert_id()" : ";select last_insert_id()";
                    }
                }
            }
            if (timeout > 0)
            {
                dbContext.CommandTimeout = timeout;
            }

            DataParameter[] pList = GetDataParameter(options);
            if (pList != null && pList.Length > 0)
            {
                return dbContext.ExecuteScalar(sql, pList).ToString();
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

            var DbType = options.GetMember<string>("type", "SqlServer");
            var DbConnectionString = options.GetMember<string>("name");
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

        private DataParameter[] GetDataParameter(DynamicObject options)
        {
            DynamicObject param = options.GetMember<DynamicObject>("param");
            if (param == null) return null;
            var paramList = param.GetDynamicProperties().ToList();
            if (paramList.Any())
            {
                List<DataParameter> pList = new List<DataParameter>();
                foreach (var pp in paramList)
                {
                    pList.Add(new DataParameter
                    {
                        Name = pp.Key,
                        Value = pp.Value
                    });
                }

                return pList.ToArray();
            }

            return null;
        }
        #endregion Private
    }
}
