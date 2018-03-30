using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.WebView.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class TimerPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterPackage(RequireManager RequireManager)
        {
            RequireManager.RegisterPackage(new TimerPackageExecutor());
            RequireManager.RegisterPackage(new TimerPackage());
        }
    }
}