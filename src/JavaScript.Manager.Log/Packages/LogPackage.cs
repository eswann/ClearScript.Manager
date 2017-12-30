using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Log.Impl;
using JavaScript.Manager.Log.Interface;

namespace JavaScript.Manager.Log.Package
{
    public class LogExcutorPackage : RequiredPackage
    {
        public LogExcutorPackage(Type logExecutoryType = null)
        {
            PackageId = "javascript_log_factory_logExecutor";
            if (logExecutoryType != null)
            {
                var isTarget = typeof(ILogExecutor).IsAssignableFrom(logExecutoryType);
                if (!isTarget)
                {
                    throw new NotSupportedException(logExecutoryType.Name + " is not implements ILogExecutor");
                }
                HostObjects.Add(new HostObject
                {
                    Name = "javascript_log_factory_logExecutor",
                    Target = Activator.CreateInstance(logExecutoryType)
                });
            }
            else
            {
                HostObjects.Add(new HostObject { Name = "javascript_log_factory_logExecutor", Target = new ConsoleLogExecutor() });
            }
           
        }

    }

    public class LogPackage : RequiredPackage
    {
        public LogPackage()
        {
            PackageId = "javascript_log_factory";
            ScriptUri = "JavaScript.Manager.Log.Scripts.log.js";
        }
    }


}
