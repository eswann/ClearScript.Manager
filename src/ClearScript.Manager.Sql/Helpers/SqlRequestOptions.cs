//-----------------------------------------------------------------------
// <copyright file="SqlRequestOptions.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Dynamic;
using JavaScript.Manager.Extensions;

namespace JavaScript.Manager.Sql.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class SqlRequestOptions
    {
        public SqlRequestOptions(DynamicObject config)
        {
            DbType = config.GetMember<string>("dbType","SqlServer");
            DbConnectionString = config.GetMember<string>("connectionString");
        }

        public string DbType { get; set; }
        public string DbConnectionString { get; set; }
    }
}