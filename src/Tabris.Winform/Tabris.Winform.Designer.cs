namespace Tabris.Winform
{
    partial class TabrisWinform
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabrisWinform));
            this.dSkinPanel1 = new DSkin.Controls.DSkinPanel();
            this.logList = new DSkin.Controls.DSkinListBox();
            this.dSkinPanel2 = new DSkin.Controls.DSkinPanel();
            this.dSkinPanel4 = new DSkin.Controls.DSkinPanel();
            this.codemirrow = new DSkin.Controls.DSkinWkeBrowser();
            this.dSkinPanel3 = new DSkin.Controls.DSkinPanel();
            this.catchBox = new DSkin.Controls.DSkinCheckBox();
            this.btExcutorSelected = new DSkin.Controls.DSkinButton();
            this.btnExcutor = new DSkin.Controls.DSkinButton();
            this.reloadRuntime = new DSkin.Controls.DSkinButton();
            this.runtimeTimeout = new DSkin.Controls.DSkinTextBox();
            this.dSkinLabel1 = new DSkin.Controls.DSkinLabel();
            this.dSkinPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logList)).BeginInit();
            this.dSkinPanel2.SuspendLayout();
            this.dSkinPanel4.SuspendLayout();
            this.dSkinPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dSkinPanel1
            // 
            this.dSkinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.dSkinPanel1.Controls.Add(this.logList);
            this.dSkinPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dSkinPanel1.Location = new System.Drawing.Point(4, 440);
            this.dSkinPanel1.Name = "dSkinPanel1";
            this.dSkinPanel1.RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel1.RightBottom")));
            this.dSkinPanel1.Size = new System.Drawing.Size(762, 179);
            this.dSkinPanel1.TabIndex = 0;
            this.dSkinPanel1.Text = "dSkinPanel1";
            // 
            // logList
            // 
            this.logList.BackColor = System.Drawing.Color.Transparent;
            this.logList.Borders.TopColor = System.Drawing.SystemColors.ControlDarkDark;
            this.logList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logList.Location = new System.Drawing.Point(0, 0);
            this.logList.Name = "logList";
            this.logList.ScrollBarWidth = 12;
            this.logList.Size = new System.Drawing.Size(762, 179);
            this.logList.TabIndex = 0;
            this.logList.Text = "dSkinListBox1";
            this.logList.Value = 0D;
            // 
            // dSkinPanel2
            // 
            this.dSkinPanel2.BackColor = System.Drawing.Color.Transparent;
            this.dSkinPanel2.Controls.Add(this.dSkinPanel4);
            this.dSkinPanel2.Controls.Add(this.dSkinPanel3);
            this.dSkinPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dSkinPanel2.Location = new System.Drawing.Point(4, 34);
            this.dSkinPanel2.Name = "dSkinPanel2";
            this.dSkinPanel2.RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel2.RightBottom")));
            this.dSkinPanel2.Size = new System.Drawing.Size(762, 406);
            this.dSkinPanel2.TabIndex = 1;
            this.dSkinPanel2.Text = "dSkinPanel2";
            // 
            // dSkinPanel4
            // 
            this.dSkinPanel4.BackColor = System.Drawing.Color.Transparent;
            this.dSkinPanel4.Controls.Add(this.codemirrow);
            this.dSkinPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dSkinPanel4.Location = new System.Drawing.Point(0, 0);
            this.dSkinPanel4.Name = "dSkinPanel4";
            this.dSkinPanel4.RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel4.RightBottom")));
            this.dSkinPanel4.Size = new System.Drawing.Size(598, 406);
            this.dSkinPanel4.TabIndex = 2;
            this.dSkinPanel4.Text = "dSkinPanel4";
            // 
            // codemirrow
            // 
            this.codemirrow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codemirrow.Location = new System.Drawing.Point(0, 0);
            this.codemirrow.Name = "codemirrow";
            this.codemirrow.Size = new System.Drawing.Size(598, 406);
            this.codemirrow.TabIndex = 0;
            this.codemirrow.Text = "dSkinWkeBrowser1";
            this.codemirrow.Url = "";
            this.codemirrow.ZoomFactor = 1F;
            // 
            // dSkinPanel3
            // 
            this.dSkinPanel3.BackColor = System.Drawing.Color.Transparent;
            this.dSkinPanel3.Controls.Add(this.runtimeTimeout);
            this.dSkinPanel3.Controls.Add(this.dSkinLabel1);
            this.dSkinPanel3.Controls.Add(this.reloadRuntime);
            this.dSkinPanel3.Controls.Add(this.catchBox);
            this.dSkinPanel3.Controls.Add(this.btExcutorSelected);
            this.dSkinPanel3.Controls.Add(this.btnExcutor);
            this.dSkinPanel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.dSkinPanel3.Location = new System.Drawing.Point(598, 0);
            this.dSkinPanel3.Name = "dSkinPanel3";
            this.dSkinPanel3.RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel3.RightBottom")));
            this.dSkinPanel3.Size = new System.Drawing.Size(164, 406);
            this.dSkinPanel3.TabIndex = 1;
            this.dSkinPanel3.Text = "dSkinPanel3";
            // 
            // catchBox
            // 
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
            this.catchBox.Location = new System.Drawing.Point(40, 381);
            this.catchBox.Name = "catchBox";
            this.catchBox.Size = new System.Drawing.Size(106, 19);
            this.catchBox.SpaceBetweenCheckMarkAndText = 3;
            this.catchBox.TabIndex = 2;
            this.catchBox.Text = "全局Catch异常";
            this.catchBox.TextColorDisabled = System.Drawing.Color.Gray;
            this.catchBox.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
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
            this.btExcutorSelected.TabIndex = 1;
            this.btExcutorSelected.Text = "执行选中";
            this.btExcutorSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btExcutorSelected.TextPadding = 0;
            this.btExcutorSelected.Click += new System.EventHandler(this.btExcutorSelected_Click);
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
            this.btnExcutor.TabIndex = 0;
            this.btnExcutor.Text = "执行";
            this.btnExcutor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnExcutor.TextPadding = 0;
            this.btnExcutor.Click += new System.EventHandler(this.btnExcutor_Click);
            // 
            // reloadRuntime
            // 
            this.reloadRuntime.AdaptImage = true;
            this.reloadRuntime.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(186)))), ((int)(((byte)(233)))));
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
            this.reloadRuntime.TabIndex = 3;
            this.reloadRuntime.Text = "重新加载运行时";
            this.reloadRuntime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.reloadRuntime.TextPadding = 0;
            this.reloadRuntime.Click += new System.EventHandler(this.reloadRuntime_Click);
            // 
            // runtimeTimeout
            // 
            this.runtimeTimeout.BitmapCache = false;
            this.runtimeTimeout.Location = new System.Drawing.Point(40, 354);
            this.runtimeTimeout.Name = "runtimeTimeout";
            this.runtimeTimeout.Size = new System.Drawing.Size(106, 21);
            this.runtimeTimeout.TabIndex = 4;
            this.runtimeTimeout.TransparencyKey = System.Drawing.Color.Empty;
            this.runtimeTimeout.WaterFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.runtimeTimeout.WaterText = "默认无限制";
            this.runtimeTimeout.WaterTextOffset = new System.Drawing.Point(5, 5);
            // 
            // dSkinLabel1
            // 
            this.dSkinLabel1.Location = new System.Drawing.Point(8, 332);
            this.dSkinLabel1.Name = "dSkinLabel1";
            this.dSkinLabel1.Size = new System.Drawing.Size(153, 16);
            this.dSkinLabel1.TabIndex = 5;
            this.dSkinLabel1.Text = "超时(毫)需重新加载运行时";
            // 
            // TabrisWinform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CaptionCenter = true;
            this.CaptionFont = new System.Drawing.Font("Ebrima", 9F, System.Drawing.FontStyle.Bold);
            this.CaptionHeight = 25;
            this.ClientSize = new System.Drawing.Size(770, 623);
            this.Controls.Add(this.dSkinPanel2);
            this.Controls.Add(this.dSkinPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IconRectangle = new System.Drawing.Rectangle(1, 1, 0, 24);
            this.Name = "TabrisWinform";
            this.Radius = 5;
            this.ShowIcon = false;
            this.Text = "Tabris-jsql脚本工具V1.0";
            this.dSkinPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logList)).EndInit();
            this.dSkinPanel2.ResumeLayout(false);
            this.dSkinPanel4.ResumeLayout(false);
            this.dSkinPanel3.ResumeLayout(false);
            this.dSkinPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinPanel dSkinPanel1;
        private DSkin.Controls.DSkinPanel dSkinPanel2;
        private DSkin.Controls.DSkinButton btnExcutor;
        private DSkin.Controls.DSkinPanel dSkinPanel4;
        private DSkin.Controls.DSkinWkeBrowser codemirrow;
        private DSkin.Controls.DSkinPanel dSkinPanel3;
        private DSkin.Controls.DSkinButton btExcutorSelected;
        private DSkin.Controls.DSkinListBox logList;
        private DSkin.Controls.DSkinCheckBox catchBox;
        private DSkin.Controls.DSkinButton reloadRuntime;
        private DSkin.Controls.DSkinTextBox runtimeTimeout;
        private DSkin.Controls.DSkinLabel dSkinLabel1;
    }
}

