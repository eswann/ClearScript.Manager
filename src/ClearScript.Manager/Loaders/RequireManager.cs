using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Manages Require packages.
    /// </summary>
    public class RequireManager
    {
        private static readonly ConcurrentDictionary<string, RequiredPackage> Packages = new ConcurrentDictionary<string, RequiredPackage>();

        /// <summary>
        /// Register a packaged for potential requirement.
        /// </summary>
        /// <param name="package">The package to register.</param>
        public static void RegisterPackage(RequiredPackage package)
        {
            Packages.TryAdd(package.PackageId, package);
        }

        /// <summary>
        /// Try to get the package from the registered packages.
        /// </summary>
        /// <param name="packageId">Id of the package to retrieve.</param>
        /// <param name="package">Package to return.</param>
        /// <returns><see cref="bool"/> indicating if package was found.</returns>
        public static bool TryGetPackage([NotNull] string packageId, out RequiredPackage package)
        {
            if (packageId == null) throw new ArgumentNullException(nameof(packageId));

            return Packages.TryGetValue(packageId, out package);
        }

        /// <summary>
        /// Clears all registered packages.
        /// </summary>
        public static void ClearPackages()
        {
            Packages.Clear();
        }

        /// <summary>
        /// Builds and returns a requirer to include with an engine.
        /// </summary>
        /// <param name="compiler">Compiles the required scripts if needed.</param>
        /// <param name="engine">The engine that will run the required script.</param>
        /// <returns></returns>
        internal static Requirer BuildRequirer([NotNull] ScriptCompiler compiler, [NotNull] V8ScriptEngine engine)
        {
            if (compiler == null) throw new ArgumentNullException(nameof(compiler));
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            var requirer = new Requirer(compiler, engine);

            //Need to add this as a host object to the script
            requirer.Engine.AddHostObject("require", new Func<string, object>(requirer.Require));
            requirer.Engine.AddHostObject("requireNamed", new Func<string, string, object>(requirer.Require));

            return requirer;
        }
    }
}