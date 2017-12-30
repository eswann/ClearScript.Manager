using System;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Log.Package;

namespace JavaScript.Manager.Log.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class LogPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterPackage(Type logExcutorType = null)
        {
            RequireManager.RegisterPackage(new LogExcutorPackage(logExcutorType));
            RequireManager.RegisterPackage(new LogPackage());
        }
    }
}