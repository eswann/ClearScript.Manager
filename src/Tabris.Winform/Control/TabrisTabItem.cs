﻿//-----------------------------------------------------------------------
// <copyright file="TabrisTabItem.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Tabris.Winform.Control
{
    using DSkin.Controls;
    using DSkin.DirectUI;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// 
    /// </summary>
    public class TabrisTabItem : DSkinTabItem
    {
     

        static Image ChromeSelect = Properties.Resources.ChromeSelect;
        static Image ChromeInactive = Properties.Resources.ChromeInactive;
        static Image ChromeClose = Properties.Resources.ChromeClose;
        static Image ChromeCloseHover = Properties.Resources.ChromeCloseHover;
        DuiButton closeBtn = new DuiButton() { NormalImage = ChromeClose, HoverImage = ChromeCloseHover, PressedImage = ChromeCloseHover, Anchor = AnchorStyles.Right, Location = new Point(178, 17) };
        public TabrisTabItem()
        {
            this.IsPureColor = true;
            this.InheritanceSize = new SizeF(0, 1);
            this.Width = 208;
            this.HoverImage = ChromeInactive;
            this.PressedImage = ChromeSelect;
            this.NormalImage = ChromeInactive;
            this.DrawButtonSudoku = true;
            this.DrawButtonSudokuPadding = new Padding(15, 5, 15, 5);
            this.ImageSize = new Size(16, 16);
            this.ImageOffset = new Point(12, 0);
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.TextInnerPadding = new Padding(32, 5, 28, 5);
            this.AutoEllipsis = true;
            this.Font = new System.Drawing.Font("微软雅黑", 9);
            this.Controls.Add(closeBtn);
            this.MouseEventBubble = false;
            closeBtn.MouseClick += closeBtn_MouseClick;

            this.MouseClick += this_MouseClick;
        }
      
     

        void this_MouseClick(object sender, DuiMouseEventArgs e)
        {
            CurrentPannelShow();
        }

        void closeBtn_MouseClick(object sender, DuiMouseEventArgs e)
        {
           
            DSkinTabBar c = this.HostControl as DSkinTabBar;
            if (c != null)
            {
                var currentIndex = c.Items.IndexOf(this);
                if (currentIndex == 0)
                {
                    return;
                }

                if (c.TabControl != null)
                {
                    TabPage p;
                    if (this.TabPage == null)
                    {
                        p = c.TabControl.TabPages[c.Items.IndexOf(this)];
                    }
                    else
                    {
                        p = this.TabPage;
                    }
                    if (p != null)
                    {
                        c.TabControl.TabPages.Remove(p);
                        p.Dispose();
                    }
                }
                this.Visible = false;
                int index = currentIndex + 1;
                DuiBaseControl d = c.Items[index];
                var lastIndex = currentIndex - 1;
                if (lastIndex > 0)
                {
                    var item = c.Items[lastIndex] as TabrisTabItem;
                    if (item != null) c.SetSelect(item);
                }
                
                LastPannelShow();
                int left = d.Left;
                d.DoEffect(left, left - this.Width + 13, 150, "Left", (p) =>
                {
                    if (!p)
                    {
                        c.Items.Remove(this);
                        if (c.Items.Count > 1)
                        {
                            c.Items[0].Margin = new Padding();
                        }
                    }
                });

            }

            var container = this.Tag as TabrisControlContainer;
            if (container != null)
            {

                container.ButtonPannel?.Hide();
                container.LogPannel?.Hide();
                container?.Dispose();
            }

            var container2 = this.Tag as ViewControlContainer;
            if (container2 != null)
            {
                container2.OnClosing();
                container2.ButtonPannel?.Hide();
                container2.LogPannel?.Hide();
                container2?.Dispose();
            }
            this.Dispose();
        }


        private void CurrentPannelShow()
        {
            try
            {
                DSkinTabBar c = this.HostControl as DSkinTabBar;
                if (c != null)
                {
                    var currentIndex = c.Items.IndexOf(this);
                    for (int i = 0; i < c.Items.Count; i++)
                    {
                        var item = c.Items[i] as TabrisTabItem;
                        if (item == null) continue;
                        if (item.Tag == null) continue;
                        var container = item.Tag as TabrisControlContainer;
                        if (container == null) continue;
                        if (i == currentIndex)
                        {
                            container.ButtonPannel?.Show();
                            container.LogPannel?.Show();
                        }
                        else
                        {
                            container.ButtonPannel?.Hide();
                            container.LogPannel?.Hide();
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LastPannelShow()
        {
            try
            {
                DSkinTabBar c = this.HostControl as DSkinTabBar;
                if (c != null)
                {
                    var currentIndex = c.Items.IndexOf(this);
                    var lastIndex = currentIndex - 1;
                    if (lastIndex < 0) return;
                    for (int i = 0; i < c.Items.Count; i++)
                    {
                        var item = c.Items[i] as TabrisTabItem;
                        if (item == null) continue;
                        if (item.Tag == null) continue;
                        var container = item.Tag as TabrisControlContainer;
                        if (container == null) continue;
                        if (i == lastIndex)
                        {
                            container.ButtonPannel.Show();
                            container.LogPannel.Show();
                        }
                        else
                        {
                            container.ButtonPannel.Hide();
                            container.LogPannel.Hide();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnControlStateChanged(EventArgs e)
        {
            base.OnControlStateChanged(e);
            if (this.ControlState == ControlStates.Pressed)
            {
                this.BringToFront();
                CurrentPannelShow();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            DSkinTabBar c = this.HostControl as DSkinTabBar;
            if (c != null && c.Items.IndexOf(this) > 0)
            {
                this.Margin = new Padding(-13, 0, 0, 0);
            }
            else
            {
                this.Margin = new Padding();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.ToolTip = Text;
        }

    }
}