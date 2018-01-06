//-----------------------------------------------------------------------
// <copyright file="ButtonPannel.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


using DSkin.DirectUI;
using System.IO;

namespace Tabris.Winform.Control
{
    using DSkin.Controls;
    using JavaScript.Manager;
    using JavaScript.Manager.Extensions;
    using Microsoft.ClearScript;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
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
        private DSkin.Controls.DSkinButton SaveButton = new DSkin.Controls.DSkinButton();
        private readonly DSkinBrowser codemirrow;
        private RuntimeManager manager;
        private readonly Action<LogLevel, string, string> logAction;

        private string fileOutPath = string.Empty;
        private bool isRun = false;
        public int Index { get; set; }

        public Action<string> OnTitleChange { get; set; }
        public Action OnModify { get; set; }
        public ButtonPannel(DSkinBrowser brower, Action<LogLevel, string, string> logAction)
        {
            this.logAction = logAction;
            init();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabrisWinform));
            this.codemirrow = brower;
            this.codemirrow.AllowDrop = true;
            this.codemirrow.GlobalObject = this;

            this.Dock = System.Windows.Forms.DockStyle.Fill;
            RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel3.RightBottom")));
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.reloadRuntime);
            this.Controls.Add(this.btExcutorSelected);
            this.Controls.Add(this.btnExcutor);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.bottomPannel);
            this.Location = new System.Drawing.Point(0, 0);
            initEvent();

            manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = 0 });
            JavaScript.Manager.Tabris.Tabris.Register(manager.RequireManager,new JavaScript.Manager.Tabris.TabrisOptions
            {
                LogExecutor = new WinformLogExcutor(logAction)
            });

        }

        /// <summary>
        /// 复制选中到粘贴板
        /// </summary>
        public void CopyFromclipboard()
        {
            var getClipData = Clipboard.GetText();
            if (!string.IsNullOrEmpty(getClipData))
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(getClipData);
                var getClipDatabase64 = Convert.ToBase64String(plainTextBytes);
                this.codemirrow.InvokeJS("insertCode(\"" + getClipDatabase64 + "\")");
            }
        }

        /// <summary>
        /// 从粘贴板粘贴内容到编辑器
        /// </summary>
        public void PasteToclipboard()
        {
            var selectedCode = this.codemirrow.InvokeJS("getPasteCode()").ToString();
            if (!string.IsNullOrEmpty(selectedCode))
            {
                Clipboard.SetText(selectedCode);
            }
        }

        /// <summary>
        /// 删除选中
        /// </summary>
        public void DeleteSeletectd()
        {
            this.codemirrow.InvokeJS("window.cmEditor.editor.replaceSelection('')");
        }

        /// <summary>
        /// 格式化选中
        /// </summary>
        public void FormatSeletectd()
        {
            this.codemirrow.InvokeJS("autoFormatSelection()");
        }

        /// <summary>
        /// 注释
        /// </summary>
        public void Annotation(bool flag)
        {
            this.codemirrow.InvokeJS(flag?"commentSelection(true)": "commentSelection(false)");
        }
        /// <summary>
        /// 是否存在有选中
        /// </summary>
        /// <returns></returns>
        public bool HaveSelected()
        {
            var selectedCode = this.codemirrow.InvokeJS("getSelectedCode()").ToString();
            return !string.IsNullOrEmpty(selectedCode);
        }

      
        private void initEvent()
        {
            this.reloadRuntime.Click += new System.EventHandler(this.reloadRuntime_Click);

            this.btExcutorSelected.Click += new System.EventHandler(this.btExcutorSelected_Click);
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);

            this.btnExcutor.Click += new System.EventHandler(this.btnExcutor_Click);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void btnExcutor_Click(object sender, EventArgs e)
        {
            var code = this.codemirrow.InvokeJS("getCode()").ToString();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("执行内容为空");
                return;
            }

            invokeJsCode(code);
        }

        private void btExcutorSelected_Click(object sender, EventArgs e)
        {
            var selectedCode = this.codemirrow.InvokeJS("getSelectedCode()").ToString();
            if (string.IsNullOrEmpty(selectedCode))
            {
                MessageBox.Show("获取选择内容为空");
                return;
            }
            invokeJsCode(selectedCode);
        }

        private void reloadRuntime_Click(object sender, EventArgs e)
        {
            try
            {
                var globalTimeout = this.runtimeTimeout.Text;
                var intTimeout = 0;
                int.TryParse(globalTimeout, out intTimeout);
                manager.Dispose();
                manager = new RuntimeManager(new ManualManagerSettings { ScriptTimeoutMilliSeconds = intTimeout });
                JavaScript.Manager.Tabris.Tabris.Register(manager.RequireManager,new JavaScript.Manager.Tabris.TabrisOptions
                {
                    LogExecutor = new WinformLogExcutor(logAction)
                });

                if (intTimeout > 0)
                {
                    logAction(LogLevel.INFO,"重新加载运行时成功,全局ScriptTimeoutMilliSeconds设置为：" + intTimeout,"");
                }
                else
                {
                    logAction(LogLevel.INFO,"重新加载运行时成功","");
                }

            }
            catch (Exception ex)
            {
                logAction(LogLevel.ERROR, "重新加载运行时失败" , ex.Message);
            }
        }
        [JSFunction]
        public void Modify()
        {
            OnModify();
        }
        [JSFunction]
        public void ExcuteSelected()
        {
            if (isRun)
            {
                logAction(LogLevel.WARN, "请等待当前任务执行完", "");
                return;
            }
            btExcutorSelected_Click(null, null);
        }
        [JSFunction]
        public void Excute()
        {
            if (isRun)
            {
                logAction(LogLevel.WARN, "请等待当前任务执行完", "");
                return;
            }
            btnExcutor_Click(null, null);
        }

        [JSFunction]
        public bool Save()
        {

            try
            {
                var code = this.codemirrow.InvokeJS("getCode()").ToString();
               


                if (!string.IsNullOrEmpty(fileOutPath))
                {
                    Task.Factory.StartNew(() =>
                    {
                        using (StreamWriter sw = new StreamWriter(fileOutPath, false))
                        {
                            sw.Write(code);
                            sw.Flush();
                        }

                        logAction(LogLevel.INFO, "保存成功", "");
                    });
                    return true;
                }

                //弹出保存框
                SaveFileDialog jsFile = new SaveFileDialog();
                jsFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                jsFile.RestoreDirectory = true;
                jsFile.Filter = "Js文件|*.js";
                if (jsFile.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(jsFile.FileName, false))
                    {
                        sw.Write(code);
                        sw.Flush();
                    }

                    fileOutPath = jsFile.FileName;
                    logAction(LogLevel.INFO, "保存成功", "");
                }

                return true;
            }
            catch (Exception ex)
            {
                logAction(LogLevel.ERROR, "保存出错", ex.Message);
                return true;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fileOutPath))
                {
                    var fileNameExt = fileOutPath.Substring(fileOutPath.LastIndexOf("\\") + 1);
                    if (OnTitleChange != null) OnTitleChange(fileNameExt);
                }
             
            }

        }
        private void invokeJsCode(string code)
        {
            if (this.catchBox.CheckState.Equals(CheckState.Checked))
            {
                code = "try{\n" + code + "\n}catch(err){\nhost.err=err.message;\nhost.ex=err;\n}";
            }
            isRun = true;
            Enable(false);
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    
                    code = "var tabris;" + "(function (){\n  tabris = tabris || require('javascript_tabris'); \n" + code + "\n})();";
                    dynamic host = new ExpandoObject();
                    var option = new ExecutionOptions
                    {
                        HostObjects = new List<HostObject>
                        {
                            new HostObject { Name = "host", Target = host }
                        }
                    };

                    await manager.ExecuteAsync(Guid.NewGuid().ToString(), code, option);
                    try
                    {
                        if (!string.IsNullOrEmpty(host.err.ToString()))
                        {
                            logAction(LogLevel.ERROR, host.err,"");
                        }
                        var exception = host.ex as DynamicObject;
                        if (exception != null)
                        {
                            var kv = exception.GetDynamicProperties();
                            if (kv != null)
                            {
                                foreach (var itemKeyValuePair in kv)
                                {
                                    if (itemKeyValuePair.Value is Exception)
                                    {
                                        Exception ex = (Exception)itemKeyValuePair.Value;
                                        while (ex.InnerException != null)
                                            ex = ex.InnerException;

                                        logAction(LogLevel.ERROR, ex.Message,"");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    //if (Index == 0)
                    //{
                    //    Thread.Sleep(10000);
                    //    logAction(LogLevel.ERROR, "ces", "");
                    //}
                }
                catch (ScriptEngineException ex)
                {
                    logAction(LogLevel.ERROR, ((Microsoft.ClearScript.ScriptEngineException)ex).ErrorDetails,"");
                }
                catch (Exception ex)
                {
                    logAction(LogLevel.ERROR, ex.Message,"");
                }
                finally
                {
                    Enable(true);
                    isRun = false;
                }


            }).ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    Exception ex = t.Exception;
                    while (ex is AggregateException && ex.InnerException != null)
                        ex = ex.InnerException;
                    logAction(LogLevel.ERROR, ex.Message,"");
                }
                else if (t.IsCanceled)
                {
                    logAction(LogLevel.ERROR, "Canclled.","");
                }
            });


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
                SaveButton.Enabled = flag;
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
            this.btnExcutor.Text = "Process         (F5)";
            this.btnExcutor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnExcutor.TextPadding = 0;

            // 
            // SaveButton
            // 
            this.SaveButton.AdaptImage = true;
            this.SaveButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(186)))), ((int)(((byte)(233)))));
            this.SaveButton.ButtonBorderColor = System.Drawing.Color.Gray;
            this.SaveButton.ButtonBorderWidth = 1;
            this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveButton.HoverColor = System.Drawing.Color.Empty;
            this.SaveButton.HoverImage = null;
            this.SaveButton.IsPureColor = false;
            this.SaveButton.Location = new System.Drawing.Point(30, 213);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.NormalImage = null;
            this.SaveButton.PressColor = System.Drawing.Color.Empty;
            this.SaveButton.PressedImage = null;
            this.SaveButton.Radius = 10;
            this.SaveButton.ShowButtonBorder = true;
            this.SaveButton.Size = new System.Drawing.Size(100, 40);
            this.SaveButton.Text = "Save To File (Ctrl+S)";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SaveButton.TextPadding = 0;

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
            this.runtimeTimeout.Enabled = true;

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            manager.Dispose();
            codemirrow.Dispose();
        }
    }
}