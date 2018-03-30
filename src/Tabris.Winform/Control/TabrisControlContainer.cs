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


    public class ViewControlContainer
    {
        public ChromeButtonPannel ButtonPannel { get; set; }

        public LogPannel LogPannel { get; set; }
        public Action OnClosing { get; set; }

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