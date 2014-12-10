using System;
using System.Collections.Generic;
using ClearScript.Manager.Extensions;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Loaders
{
    public class Requirer
    {
        private static readonly Dictionary<string, RequiredPackage> _packages = new Dictionary<string, RequiredPackage>();

        private ScriptCompiler _compiler;
        private V8ScriptEngine _engine;
        

        public static void RegisterPackage(RequiredPackage package)
        {
            _packages.Add(package.PackageId, package);
        }


        public static void ClearPackages()
        {
            _packages.Clear();
        }

        internal static Requirer Build(ScriptCompiler compiler, V8ScriptEngine engine)
        {
            var requirer = new Requirer
            {
                _compiler = compiler,
                _engine = engine
            };
            
            //Need to add this as a host object to the script
            requirer._engine.AddHostObject("require", new Func<string, object>(requirer.Require));

            return requirer;
        }

        public object Require(string packageId)
        {
            RequiredPackage package;

            if (!_packages.TryGetValue(packageId, out package))
            {
                throw new KeyNotFoundException(string.Format("The package with ID {0} was not found, did you register this package?", packageId));
            }

            var options = new ExecutionOptions
            {
                HostObjects = package.HostObjects,
                HostTypes = package.HostTypes
            };

            _engine.ApplyOptions(options);

            var compiledScript = _compiler.Compile(new IncludeScript { Uri = package.ScriptUri, PrependCode = packageId + "={};" });

            _engine.Execute(compiledScript);

            var result = DynamicExtensions.GetProperty(_engine.Script, packageId);

            return result;
        }

    }
}