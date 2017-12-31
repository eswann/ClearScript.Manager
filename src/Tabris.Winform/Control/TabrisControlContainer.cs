//-----------------------------------------------------------------------
// <copyright file="TabrisControlContainer.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Tabris.Winform.Control
{
    using DSkin.Controls;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class TabrisControlContainer
    {
        public ButtonPannel ButtonPannel { get; set; }

        public DSkinWkeBrowser DSkinWkeBrowser { get; set; }

        public void Dispose()
        {
            try
            {
                ButtonPannel.Dispose();
                DSkinWkeBrowser.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}