using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.SqlServer;
using JavaScript.Manager.Extensions;
using JavaScript.Manager.Sql.Interface;

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
    public class AntOrmDbExecutor: AbstractDbDefaultExecutor
    {
        public override void Transaction(dynamic callback, dynamic options)
        {
           
        }

        public override DataTable ExecutorQuery(dynamic options)
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

            return dbContext.ExecuteDataTable(sql, new Dictionary<string, AntData.ORM.Common.CustomerParam>());
        }

        public override int ExecutorNonQuery(dynamic options)
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

        public override string ExecutorScalar(dynamic options)
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
