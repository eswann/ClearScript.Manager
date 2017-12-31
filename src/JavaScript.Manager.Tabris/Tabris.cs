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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 打包所有的
    /// </summary>
    public class Tabris
    {

        /// <summary>
        /// 注册所有需要使用的包
        /// </summary>
        /// <param name="options"></param>
        public static void Register(TabrisOptions options = null)
        {
            //tabris
            RequireManager.RegisterPackage(new TabrisPackage());

            //sql
            SqlPackageHelpers.RegisterPackage(options?.DbExecutor);

            //http
            HttpPackageHelpers.RegisterPackage();

            //log
            LogPackageHelpers.RegisterPackage(options?.LogExecutor);

        }


    }

    public class TabrisOptions
    {
        public object DbExecutor { get; set; }    
        public object LogExecutor { get; set; }    
    }
}