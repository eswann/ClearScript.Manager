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
using JavaScript.Manager.Sql.Helpers;
using Microsoft.ClearScript;

namespace JavaScript.Manager.Sql.Package
{
    public class SqlExecutor : RequiredPackage
    {
        public SqlExecutor()
        {
            PackageId = "javascript_sqlExecutor";
            HostObjects.Add(new HostObject { Name = "javascript_sqlExecutor", Target = new DbExecutor() });
        }

    }

    public class SqlPackage : RequiredPackage
    {
        public SqlPackage()
        {
            PackageId = "sql";
            ScriptUri = "JavaScript.Manager.Sql.Scripts.sql.js";
            HostObjects.Add(new HostObject { Name = "xHost", Target = new ExtendedHostFunctions() });
        }


    }

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

    public class Table
    {
        
    }
}
