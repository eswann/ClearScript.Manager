using JavaScript.Manager.Loaders;
using JavaScript.Manager.Sql.Package;

namespace JavaScript.Manager.Sql.Packages
{
    /// <summary>
    /// Helpers for registering packages.
    /// </summary>
    public static class SqlPackageHelpers
    {
        /// <summary>
        /// Registers packages needed for using the request include.
        /// </summary>
        public static void RegisterPackage(RequireManager RequireManager, object sqlExcutor = null)
        {
            RequireManager.RegisterPackage(new SqlExecutor(sqlExcutor));
            RequireManager.RegisterPackage(new SqlPackage());
        }
    }
}