using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.Http.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class HttpPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterPackage(RequireManager RequireManager)
        {
            RequireManager.RegisterPackage(new HttpPackage());
            RequireManager.RegisterPackage(new RequestPackage());
        }
    }
}