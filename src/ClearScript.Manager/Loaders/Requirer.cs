using JavaScript.Manager.Extensions;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Concurrent;
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

        private int _lastRequireIdex;
        private readonly ConcurrentDictionary<int, string> _LastRequireDic = new ConcurrentDictionary<int, string>();

        public Requirer()
        {
            _LastRequireDic.TryAdd(0, null);
        }
        /// <summary>
        /// Called via a javascript to require and return the requested package.
        /// </summary>
        /// <param name="packageId">ID of the RequirePackage to require.</param>
        /// <returns>The return object to use for the require. Either the export from the require script or the returned HostObject if not script is present.</returns>
        public object Require(string packageId)
        {
            return Require(packageId, null);
        }

        private FilePath GetRealFilePath(string url)
        {
            var filePath = url.GetAbsolutePath(_LastRequireDic[_lastRequireIdex].GetFolderPath());

            _lastRequireIdex++;
            _LastRequireDic.TryAdd(_lastRequireIdex, filePath);

            return new FilePath
            {
                NativeRequirePath = filePath,
                PackageId = DerivePackageIdFromUri(url)+ "_" + filePath.MD516(),
            };
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

            if (packageId.StartsWith("/"))
            {
                packageId = "." + packageId;
            }
            if (packageId.Contains("/") || packageId.Contains("\\"))
            {
                //http 和 file 类型的都会进来
                if (!hasUri)
                {
                    scriptUri = packageId;
                    hasUri = true;
                }

                //拿到真正的地址
                var real = GetRealFilePath(scriptUri);

                packageId = real.PackageId;
                scriptUri = real.NativeRequirePath;


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

            if (package.Exports != null)
            {
                return package.Exports;
            }

            var options = new ExecutionOptions
            {
                HostObjects = package.HostObjects,
                HostTypes = package.HostTypes
            };

            Engine.ApplyOptions(options);

            if (!String.IsNullOrEmpty(package.ScriptUri))
            {
                var includScript = new IncludeScript
                {
                    Uri = package.ScriptUri,
                    PrependCode = "var " + package.PackageId + " = {};",
                    RequiredPackage = package
                };

                var compiledScript = Compiler.Compile(includScript);

                if (compiledScript == null && includScript.Exports != null)//DLL场景
                {
                    if (packageCreated)
                    {
                        package.Exports = includScript.Exports;
                        RequireManager.RegisterPackage(package);
                    }
                    _LastRequireDic.TryRemove(_lastRequireIdex, out string _aa);
                    _lastRequireIdex--;
                    return package.Exports;
                }
              
                Engine.Execute(compiledScript);

                var outputObject = DynamicExtensions.GetProperty(Engine.Script, package.PackageId);

                if (outputObject is Undefined)
                {
                    //try to find pascal case if camel is not present.
                    outputObject = DynamicExtensions.GetProperty(Engine.Script, package.PackageId.ToPascalCase());
                }

                if (packageCreated)
                {
                    RequireManager.RegisterPackage(package);
                }

               
                _LastRequireDic.TryRemove(_lastRequireIdex,out string _a);
                _lastRequireIdex--;
                return outputObject.exports;
            }

            if (options.HostObjects.SafeAny())
                return options.HostObjects[0].Target;

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