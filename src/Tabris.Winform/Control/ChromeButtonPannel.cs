using CefSharp;
using CefSharp.WinForms;
using System;

namespace Tabris.Winform.Control
{
    public class ChromeButtonPannel : DSkin.Controls.DSkinPanel
    {
        private DSkin.Controls.DSkinButton btnExcutor = new DSkin.Controls.DSkinButton();
        private readonly ChromiumWebBrowser _brower;

        public ChromeButtonPannel(ChromiumWebBrowser brower)
        {
            _brower = brower;
            init();
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnExcutor);
            this.Location = new System.Drawing.Point(0, 0);
            initEvent();
        }
        private void initEvent()
        {

            this.btnExcutor.Click += new System.EventHandler(this.btnExcutor_Click);
        }

        private void btnExcutor_Click(object sender, EventArgs e)
        {
            _brower.ShowDevTools();
        }

        private void init()
        {
          
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
            this.btnExcutor.Location = new System.Drawing.Point(8, 20);
            this.btnExcutor.Name = "btnExcutor";
            this.btnExcutor.NormalImage = null;
            this.btnExcutor.PressColor = System.Drawing.Color.Empty;
            this.btnExcutor.PressedImage = null;
            this.btnExcutor.Radius = 10;
            this.btnExcutor.ShowButtonBorder = true;
            this.btnExcutor.Size = new System.Drawing.Size(100, 40);
            this.btnExcutor.TabIndex = 0;
            this.btnExcutor.Text = "OpShowDevTool";
            this.btnExcutor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnExcutor.TextPadding = 0;
            this.btnExcutor.Cursor = System.Windows.Forms.Cursors.Hand;



        }

    }
}
