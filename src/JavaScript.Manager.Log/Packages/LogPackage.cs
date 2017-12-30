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
        public LogExcutorPackage(object logExecutory = null)
        {
            PackageId = "javascript_log_factory_logExecutor";
            if (logExecutory != null)
            {
                var type = logExecutory as Type;
                if (type != null)
                {
                    var isTarget = typeof(ILogExecutor).IsAssignableFrom(type);
                    if (!isTarget)
                    {
                        throw new NotSupportedException(type.Name + " is not implements ILogExecutor");
                    }
                    HostObjects.Add(new HostObject
                    {
                        Name = "javascript_log_factory_logExecutor",
                        Target = Activator.CreateInstance(type)
                    });
                }
                else
                {
                    var isTarget = typeof(AbstractLogExcutor).IsAssignableFrom(logExecutory.GetType());
                    if (!isTarget)
                    {
                        throw new NotSupportedException(logExecutory.GetType().Name + " is not implements ILogExecutor");
                    }
                    HostObjects.Add(new HostObject
                    {
                        Name = "javascript_log_factory_logExecutor",
                        Target = logExecutory
                    });
                }
               
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
