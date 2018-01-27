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
    /// <summary>
    /// 
    /// </summary>
    public class TabrisPackage : RequiredPackage
    {
        public TabrisPackage()
        {
            PackageId = "javascript_tabris";
            ScriptUri = "JavaScript.Manager.Tabris.Scripts.tabris.js";
            RequiredPackageType = RequiredPackageType.EmbeddedFile;
        }
    }
}