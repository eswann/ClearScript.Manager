//-----------------------------------------------------------------------
// <copyright file="TabrisPackage.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using JavaScript.Manager.Loaders;

namespace JavaScript.Manager.Tabris.Packages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class TabrisPackage : RequiredPackage
    {
        public TabrisPackage()
        {
            PackageId = "tabris";
            ScriptUri = "JavaScript.Manager.Tabris.Scripts.tabris.js";
        }
    }
}