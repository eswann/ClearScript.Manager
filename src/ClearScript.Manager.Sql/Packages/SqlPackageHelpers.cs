using ClearScript.Manager.Loaders;
using ClearScript.Manager.Sql.Package;

namespace ClearScript.Manager.Http.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class SqlPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterSqlPackages()
        {
            RequireManager.RegisterPackage(new SqlExecutor());
            RequireManager.RegisterPackage(new SqlPackage());
        }
    }
}