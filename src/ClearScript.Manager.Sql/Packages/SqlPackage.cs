using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Sql.Interface;
using Microsoft.ClearScript;

namespace JavaScript.Manager.Sql.Package
{
    public class SqlExecutor : RequiredPackage
    {
        public SqlExecutor(object sqlExecutory)
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
                throw new ArgumentException("sqlExecutory");
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

}
