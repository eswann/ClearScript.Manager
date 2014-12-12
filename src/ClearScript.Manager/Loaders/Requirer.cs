using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ClearScript.Manager.Extensions;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Used to register require packages and also provided to scripts to return required resources via the "require" call.
    /// </summary>
    public class Requirer
    {
        private static readonly ConcurrentDictionary<string, RequiredPackage> _packages = new ConcurrentDictionary<string, RequiredPackage>();

        private ScriptCompiler _compiler;
        private V8ScriptEngine _engine;
        
        /// <summary>
        /// Register a packaged for potential requirement.
        /// </summary>
        /// <param name="package">The package to register.</param>
        public static void RegisterPackage(RequiredPackage package)
        {
            _packages.TryAdd(package.PackageId, package);
        }

        /// <summary>
        /// Clears all registered packages.
        /// </summary>
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

        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        public object Require(string packageId)
        {
            RequiredPackage package;
            bool addPackage = false;

            if (!_packages.TryGetValue(packageId, out package))
            {
                if (packageId.Contains("/") || packageId.Contains("\\"))
                {
                    package = new RequiredPackage { PackageId = packageId, ScriptUri = packageId };
                    addPackage = true;
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("The package with ID {0} was not found, did you register this package?", packageId));
                }
            }

            var options = new ExecutionOptions
            {
                HostObjects = package.HostObjects,
                HostTypes = package.HostTypes
            };

            _engine.ApplyOptions(options);

            if (!string.IsNullOrEmpty(package.ScriptUri))
            {
                var compiledScript = _compiler.Compile(new IncludeScript {Uri = package.ScriptUri});

                _engine.Execute(compiledScript);

                var outputObject = DynamicExtensions.GetProperty(_engine.Script, packageId);

                if (addPackage)
                {
                    RegisterPackage(package);
                }

                return outputObject;
            }

            if (options.HostObjects.SafeAny())
                return options.HostObjects[0];

            return null;
        }

    }
}