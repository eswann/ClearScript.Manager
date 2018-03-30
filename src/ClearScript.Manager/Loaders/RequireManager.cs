﻿using Microsoft.ClearScript.V8;
using System;
using System.Collections.Concurrent;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Manages Require packages.
    /// </summary>
    public class RequireManager
    {
        private readonly ConcurrentDictionary<string, RequiredPackage> _packages = new ConcurrentDictionary<string, RequiredPackage>();

        /// <summary>
        /// Register a packaged for potential requirement.
        /// </summary>
        /// <param name="package">The package to register.</param>
        public  void RegisterPackage(RequiredPackage package)
        {
            _packages.TryAdd(package.PackageId, package);
        }

        /// <summary>
        /// Try to get the package from the registered packages.
        /// </summary>
        /// <param name="packageId">Id of the package to retrieve.</param>
        /// <param name="package">Package to return.</param>
        /// <returns>Bool indicating if package was found.</returns>
        public  bool TryGetPackage(string packageId, out RequiredPackage package)
        {
            return _packages.TryGetValue(packageId, out package);
        }

        /// <summary>
        /// Clears all registered packages.
        /// </summary>
        public  void ClearPackages()
        {
            _packages.Clear();
        }

        /// <summary>
        /// Builds and returns a requirer to include with an engine.
        /// </summary>
        /// <param name="compiler">Compiles the required scripts if needed.</param>
        /// <param name="engine">The engine that will run the required script.</param>
        /// <returns></returns>
        internal  Requirer BuildRequirer(ScriptCompiler compiler, V8ScriptEngine engine)
        {
            var requirer = new Requirer
            {
                Compiler = compiler,
                Engine = engine,
                RequireManager = this
            };

            //Need to add this as a host object to the script
            requirer.Engine.AddHostObject("require", new Func<string, object>(requirer.Require));
            requirer.Engine.AddHostObject("requireNamed", new Func<string, string, object>(requirer.Require));

            return requirer;
        }
    }
}