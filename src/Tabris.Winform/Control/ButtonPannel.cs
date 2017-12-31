//-----------------------------------------------------------------------
// <copyright file="ButtonPannel.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Tabris.Winform.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;


    /// <summary>
    /// 每个Tab对应一个Button Pannel
    /// </summary>
    public class ButtonPannel : DSkin.Controls.DSkinPanel
    {
        private DSkin.Controls.DSkinButton btnExcutor = new DSkin.Controls.DSkinButton();
        private DSkin.Controls.DSkinButton btExcutorSelected = new DSkin.Controls.DSkinButton();
        private DSkin.Controls.DSkinCheckBox catchBox = new DSkin.Controls.DSkinCheckBox();
        private DSkin.Controls.DSkinButton reloadRuntime = new DSkin.Controls.DSkinButton();
        private DSkin.Controls.DSkinTextBox runtimeTimeout = new DSkin.Controls.DSkinTextBox();
        private DSkin.Controls.DSkinLabel dSkinLabel1 = new DSkin.Controls.DSkinLabel();
        private DSkin.Controls.DSkinPanel bottomPannel = new DSkin.Controls.DSkinPanel();
        private int _index;
        public ButtonPannel(int index)
        {
            init();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabrisWinform));
            _index = index;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel3.RightBottom")));
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.reloadRuntime);
            this.Controls.Add(this.btExcutorSelected);
            this.Controls.Add(this.btnExcutor);
            this.Controls.Add(this.bottomPannel);
            this.Location = new System.Drawing.Point(0, 0);
            this.bottomPannel.BringToFront();
            initEvent();

            this.runtimeTimeout.Text = "" + index;
        }

      
        private void initEvent()
        {
            this.reloadRuntime.Click += new System.EventHandler(this.reloadRuntime_Click);

            this.btExcutorSelected.Click += new System.EventHandler(this.btExcutorSelected_Click);

            this.btnExcutor.Click += new System.EventHandler(this.btnExcutor_Click);
        }

        private void btnExcutor_Click(object sender, EventArgs e)
        {

        }

        private void btExcutorSelected_Click(object sender, EventArgs e)
        {
        }

        private void reloadRuntime_Click(object sender, EventArgs e)
        {
        }
        private void Enable(bool flag)
        {
            this.Invoke(new EventHandler(delegate
            {
                btnExcutor.Enabled = flag;
                btExcutorSelected.Enabled = flag;
                reloadRuntime.Enabled = flag;
                catchBox.Enabled = flag;
                runtimeTimeout.Enabled = flag;
            }));
        }
        private void init()
        {
            // 
            // bottomPannel
            // 
            this.bottomPannel.Dock = DockStyle.Bottom;
            this.bottomPannel.Controls.Add(this.dSkinLabel1);
            this.bottomPannel.Controls.Add(this.runtimeTimeout);
            this.bottomPannel.Controls.Add(this.catchBox);
            this.bottomPannel.Name = "bottomPannel";
            this.bottomPannel.Size = new System.Drawing.Size(164, 137);
            this.bottomPannel.Text = "dSkinPanel5";
            this.bottomPannel.BackColor = System.Drawing.Color.Transparent;
            this.bottomPannel.Location = new System.Drawing.Point(0, 274);
            // 
            // btExcutorSelected
            // 
            this.btExcutorSelected.AdaptImage = true;
            this.btExcutorSelected.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(186)))), ((int)(((byte)(233)))));
            this.btExcutorSelected.ButtonBorderColor = System.Drawing.Color.Gray;
            this.btExcutorSelected.ButtonBorderWidth = 1;
            this.btExcutorSelected.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btExcutorSelected.HoverColor = System.Drawing.Color.Empty;
            this.btExcutorSelected.HoverImage = null;
            this.btExcutorSelected.IsPureColor = false;
            this.btExcutorSelected.Location = new System.Drawing.Point(30, 82);
            this.btExcutorSelected.Name = "btExcutorSelected";
            this.btExcutorSelected.NormalImage = null;
            this.btExcutorSelected.PressColor = System.Drawing.Color.Empty;
            this.btExcutorSelected.PressedImage = null;
            this.btExcutorSelected.Radius = 10;
            this.btExcutorSelected.ShowButtonBorder = true;
            this.btExcutorSelected.Size = new System.Drawing.Size(100, 40);
            this.btExcutorSelected.Text = "Process(Selected)";
            this.btExcutorSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btExcutorSelected.TextPadding = 0;
           
            // 
            // btnExcutor
            // 
            this.btnExcutor.AdaptImage = true;
            this.btnExcutor.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(186)))), ((int)(((byte)(233)))));
            this.btnExcutor.ButtonBorderColor = System.Drawing.Color.Gray;
            this.btnExcutor.ButtonBorderWidth = 1;
            this.btnExcutor.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnExcutor.HoverColor = System.Drawing.Color.Empty;
            this.btnExcutor.HoverImage = null;
            this.btnExcutor.IsPureColor = false;
            this.btnExcutor.Location = new System.Drawing.Point(30, 20);
            this.btnExcutor.Name = "btnExcutor";
            this.btnExcutor.NormalImage = null;
            this.btnExcutor.PressColor = System.Drawing.Color.Empty;
            this.btnExcutor.PressedImage = null;
            this.btnExcutor.Radius = 10;
            this.btnExcutor.ShowButtonBorder = true;
            this.btnExcutor.Size = new System.Drawing.Size(100, 40);
            this.btnExcutor.Text = "Process";
            this.btnExcutor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnExcutor.TextPadding = 0;

            // 
            // reloadRuntime
            // 
            this.reloadRuntime.AdaptImage = true;
            this.reloadRuntime.BaseColor = System.Drawing.Color.Crimson;
            this.reloadRuntime.ButtonBorderColor = System.Drawing.Color.Gray;
            this.reloadRuntime.ButtonBorderWidth = 1;
            this.reloadRuntime.DialogResult = System.Windows.Forms.DialogResult.None;
            this.reloadRuntime.HoverColor = System.Drawing.Color.Empty;
            this.reloadRuntime.HoverImage = null;
            this.reloadRuntime.IsPureColor = false;
            this.reloadRuntime.Location = new System.Drawing.Point(30, 148);
            this.reloadRuntime.Name = "reloadRuntime";
            this.reloadRuntime.NormalImage = null;
            this.reloadRuntime.PressColor = System.Drawing.Color.Empty;
            this.reloadRuntime.PressedImage = null;
            this.reloadRuntime.Radius = 10;
            this.reloadRuntime.ShowButtonBorder = true;
            this.reloadRuntime.Size = new System.Drawing.Size(100, 40);
            this.reloadRuntime.Text = "Reload(JsRuntime)";
            this.reloadRuntime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.reloadRuntime.TextPadding = 0;


            // 
            // runtimeTimeout
            // 
            this.runtimeTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runtimeTimeout.BitmapCache = false;
            this.runtimeTimeout.Location = new System.Drawing.Point(8, 86);
            this.runtimeTimeout.Name = "runtimeTimeout";
            this.runtimeTimeout.Size = new System.Drawing.Size(137, 21);
            this.runtimeTimeout.TransparencyKey = System.Drawing.Color.Empty;
            this.runtimeTimeout.WaterFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.runtimeTimeout.WaterText = "unlimited default";
            this.runtimeTimeout.WaterTextOffset = new System.Drawing.Point(5, 5);


            // 
            // dSkinLabel1
            // 
            this.dSkinLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dSkinLabel1.AutoSize = false;
            this.dSkinLabel1.Location = new System.Drawing.Point(8, 47);
            this.dSkinLabel1.Name = "dSkinLabel1";
            this.dSkinLabel1.Size = new System.Drawing.Size(139, 33);
            this.dSkinLabel1.TabIndex = 5;
            this.dSkinLabel1.Text = "Timeout For Runtime(Need reload runtime)";
            this.dSkinLabel1.Enabled = true;

            // 
            // catchBox
            // 
            this.catchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.catchBox.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.catchBox.Checked = true;
            this.catchBox.CheckFlagColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.catchBox.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.catchBox.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.catchBox.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(239)))), ((int)(((byte)(219)))));
            this.catchBox.CheckRectBackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(239)))), ((int)(((byte)(219)))));
            this.catchBox.CheckRectBackColorPressed = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(226)))), ((int)(((byte)(188)))));
            this.catchBox.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.catchBox.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.catchBox.CheckRectWidth = 14;
            this.catchBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.catchBox.InnerPaddingWidth = 2;
            this.catchBox.InnerRectInflate = 3;
            this.catchBox.Location = new System.Drawing.Point(8, 113);
            this.catchBox.Name = "catchBox";
            this.catchBox.Size = new System.Drawing.Size(137, 18);
            this.catchBox.SpaceBetweenCheckMarkAndText = 3;
            this.catchBox.TabIndex = 2;
            this.catchBox.Text = "Catch Global Error";
            this.catchBox.TextColorDisabled = System.Drawing.Color.Gray;
            this.catchBox.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.catchBox.Enabled = true;


        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }
    }
}