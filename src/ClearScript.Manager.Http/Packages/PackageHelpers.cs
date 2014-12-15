using ClearScript.Manager.Loaders;

namespace ClearScript.Manager.Http.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class PackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterRequestPackages()
        {
            RequireManager.RegisterPackage(new HttpPackage());
            RequireManager.RegisterPackage(new RequestPackage());
        }
    }
}