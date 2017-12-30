//-----------------------------------------------------------------------
// <copyright file="TabrisPackageHelpers.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using JavaScript.Manager.Http.Packages;
using JavaScript.Manager.Loaders;
using JavaScript.Manager.Sql.Packages;

namespace JavaScript.Manager.Tabris.Packages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 打包所有的
    /// </summary>
    public class TabrisPackageHelpers
    {
        public static void RegisterTabrisPackages()
        {
            //tabris
            RequireManager.RegisterPackage(new TabrisPackage());

            //sql
            SqlPackageHelpers.RegisterSqlPackages();

            //http
            HttpPackageHelpers.RegisterRequestPackages();

        }
    }
}