//-----------------------------------------------------------------------
// <copyright file="TabrisPackageHelpers.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using JavaScript.Manager.Http.Packages;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Log.Packages;
using JavaScript.Manager.Sql.Packages;
using JavaScript.Manager.Tabris.Packages;

namespace JavaScript.Manager.Tabris
{
    /// <summary>
    /// 打包所有的
    /// </summary>
    public class Tabris
    {
        /// <summary>
        /// 注册所有需要使用的包
        /// </summary>
        /// <param name="RequireManager"></param>
        /// <param name="options"></param>
        public static void Register(RequireManager RequireManager, TabrisOptions options = null)
        {
            //tabris
            RequireManager.RegisterPackage(new TabrisPackage());

            //sql
            SqlPackageHelpers.RegisterPackage(RequireManager,options?.DbExecutor);

            //http
            HttpPackageHelpers.RegisterPackage(RequireManager);

            //log
            LogPackageHelpers.RegisterPackage(RequireManager,options?.LogExecutor);

        }


    }

    public class TabrisOptions
    {
        public object DbExecutor { get; set; }    
        public object LogExecutor { get; set; }    
    }
}