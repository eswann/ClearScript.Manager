using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.WebView.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class WebViewPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterPackage(RequireManager RequireManager, object webViewExecutor)
        {
            RequireManager.RegisterPackage(new WebViewExcutorPackage(webViewExecutor));
            RequireManager.RegisterPackage(new WebViewPackage());
        }
    }
}