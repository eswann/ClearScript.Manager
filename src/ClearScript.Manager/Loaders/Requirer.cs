using System;
using System.Collections.Generic;
using ClearScript.Manager.Extensions;
using JetBrains.Annotations;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ClearScript.Manager.Loaders
{
    /// <summary>
    /// Used to register require packages and also provided to scripts to return required resources via the "require" call.
    /// </summary>
    public class Requirer
    {
        [NotNull]
        internal ScriptCompiler Compiler { get; set; }

        [NotNull]
        internal V8ScriptEngine Engine { get; set; }

        internal Requirer([NotNull] ScriptCompiler compiler, [NotNull] V8ScriptEngine engine)
        {
            Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
            Engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        [CanBeNull]
        public object Require([NotNull] string packageId)
        {
            if (packageId == null) throw new ArgumentNullException(nameof(packageId));

            return Require(packageId, null);
        }

        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <param name="scriptUri">A script Uri.  This is only needed if the packageId doesn't meet the script convention name and the package is not a registered package.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        [CanBeNull]
        public object Require([NotNull] string packageId, [CanBeNull] string scriptUri)
        {
            if (packageId == null) throw new ArgumentNullException(nameof(packageId));

            var hasUri = !string.IsNullOrEmpty(scriptUri);
            var packageCreated = false;

            if (packageId.Contains("/") || packageId.Contains("\\"))
            {
                if (!hasUri)
                {
                    scriptUri = packageId;
                    hasUri = true;
                }

                packageId = DerivePackageIdFromUri(scriptUri);
                
                if (packageId.Length == 0)
                    throw new ArgumentException(
                        "The provided packageId is not a valid package name. The packageId must be a valid file path or Uri if path characters are contained in the name.");
            }

            if (!RequireManager.TryGetPackage(packageId, out var package))
            {
                if (!hasUri)
                {
                    throw new KeyNotFoundException(
                        $"The package with ID {packageId} was not found, did you register this package?");
                }
                
                package = new RequiredPackage {PackageId = packageId, ScriptUri = scriptUri};
                packageCreated = true;
            }

            var options = new ExecutionOptions
            {
                HostObjects = package.HostObjects,
                HostTypes = package.HostTypes
            };

            Engine.ApplyOptions(options);

            if (!string.IsNullOrEmpty(package.ScriptUri))
            {
                var compiledScript = Compiler.Compile(new IncludeScript {
                    Uri = package.ScriptUri,
                    PrependCode = $"var {packageId} = {{}};"
                });

                Engine.Execute(compiledScript);

                var outputObject = DynamicExtensions.GetProperty(Engine.Script, packageId);

                if (outputObject is Undefined)
                {
                    //try to find pascal case if camel is not present.
                    outputObject = DynamicExtensions.GetProperty(Engine.Script, packageId.ToPascalCase());
                }

                if (packageCreated)
                {
                    RequireManager.RegisterPackage(package);
                }

                return outputObject.exports;
            }

            if (options.HostObjects.SafeAny())
                return options.HostObjects[0];

            return null;
        }

        [NotNull]
        private string DerivePackageIdFromUri([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            var segments = path.Split(new []{'/','\\'}, StringSplitOptions.RemoveEmptyEntries);

            var resource = segments[segments.Length - 1];

            var periodIndex = resource.IndexOfAny(new[]{'.','?'});

            if (periodIndex > 0)
            {
                resource = resource.Substring(0, periodIndex);
            }

            return resource.ToCamelCase();
        }
    }
}