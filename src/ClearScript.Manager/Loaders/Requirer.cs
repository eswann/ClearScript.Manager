using JavaScript.Manager.Extensions;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;

namespace JavaScript.Manager.Loaders
{
    /// <summary>
    /// Used to register require packages and also provided to scripts to return required resources via the "require" call.
    /// </summary>
    public class Requirer
    {
        internal ScriptCompiler Compiler { get; set; }
        internal V8ScriptEngine Engine { get; set; }
        internal RequireManager RequireManager { get; set; }

        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        public object Require(string packageId)
        {
            return Require(packageId, null);
        }


        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <param name="scriptUri">A script uri.  This is only needed if the packageId doesn't meet the script convention name and the package is not a registered package.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        public object Require(string packageId, string scriptUri)
        {
            RequiredPackage package;
            bool hasUri = !String.IsNullOrEmpty(scriptUri);
            bool packageCreated = false;

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
                        "The provided packageId is not a valid package name. The packageId must be a valid file path or uri if path characters are contained in the name.");
            }

            if (!RequireManager.TryGetPackage(packageId, out package))
            {
                if (!hasUri)
                {
                    throw new KeyNotFoundException(String.Format("The package with ID {0} was not found, did you register this package?", packageId));
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

            if (!String.IsNullOrEmpty(package.ScriptUri))
            {
                var compiledScript = Compiler.Compile(new IncludeScript
                {
                    Uri = package.ScriptUri,
                    PrependCode = "var " + packageId + " = {};",
                    RequiredPackage = package
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

        private string DerivePackageIdFromUri(string path)
        {
            string[] segments = path.Split(new []{'/','\\'}, StringSplitOptions.RemoveEmptyEntries);

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