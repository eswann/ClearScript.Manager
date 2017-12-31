using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntData.ORM.Data;
using AntData.ORM.DataProvider;
using AntData.ORM.DataProvider.MySql;
using AntData.ORM.DataProvider.SqlServer;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Sql.Impl;
using JavaScript.Manager.Sql.Interface;
using Microsoft.ClearScript;

namespace JavaScript.Manager.Sql.Package
{
    public class SqlExecutor : RequiredPackage
    {
        public SqlExecutor(object sqlExecutory = null)
        {
            PackageId = "javascript_sql_factory_sqlExecutor";
            if (sqlExecutory != null)
            {
                var type = sqlExecutory as Type;
                if (type != null)
                {
                    var isTarget = typeof(IDbExecutor).IsAssignableFrom(type);
                    if (!isTarget)
                    {
                        throw new NotSupportedException(type.Name + " is not implements IDbExecutor");
                    }
                    HostObjects.Add(new HostObject
                    {
                        Name = "javascript_sql_factory_sqlExecutor",
                        Target = Activator.CreateInstance(type)
                    });
                }
                else
                {
                    var isTarget = typeof(AbstractDbDefaultExecutor).IsAssignableFrom(sqlExecutory.GetType());
                    if (!isTarget)
                    {
                        throw new NotSupportedException(sqlExecutory.GetType().Name + " is not implements AbstractDbDefaultExecutor");
                    }
                    HostObjects.Add(new HostObject
                    {
                        Name = "javascript_sql_factory_sqlExecutor",
                        Target = sqlExecutory
                    });
                }
            }
            else
            {
                HostObjects.Add(new HostObject { Name = "javascript_sql_factory_sqlExecutor", Target = new DbDefaultExecutor() });
            }
           
        }

    }

    public class SqlPackage : RequiredPackage
    {
        public SqlPackage()
        {
            PackageId = "javascript_sql_factory";
            ScriptUri = "JavaScript.Manager.Sql.Scripts.sql.js";
            HostObjects.Add(new HostObject { Name = "xHost", Target = new ExtendedHostFunctions() });
        }
    }

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

}
