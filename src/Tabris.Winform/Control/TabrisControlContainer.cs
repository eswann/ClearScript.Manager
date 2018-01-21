//-----------------------------------------------------------------------
// <copyright file="TabrisControlContainer.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


namespace Tabris.Winform.Control
{
    using System;


    /// <summary>
    /// 
    /// </summary>
    public class TabrisControlContainer
    {
        public ButtonPannel ButtonPannel { get; set; }

        public LogPannel LogPannel { get; set; }
        public string TagName { get; set; }

        public void Dispose()
        {
            try
            {
                ButtonPannel.Dispose();
                LogPannel.Dispose();

            }
            catch (Exception)
            {

            }
        }
    }
}