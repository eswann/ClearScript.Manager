//-----------------------------------------------------------------------
// <copyright file="ButtonPannel.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


using CefSharp;
using CefSharp.WinForms;
using JavaScript.Manager.Debugger;
using System.IO;

namespace Tabris.Winform.Control
{
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
        private readonly ChromiumWebBrowser codemirrow;
        private RuntimeManager manager;
        private readonly Action<LogLevel, string, string> logAction;

        private string fileOutPath = string.Empty;
        private bool isRun = false;
        public int Index { get; set; }

        public Action<string> OnTitleChange { get; set; }
        public Action OnModify { get; set; }
        public ButtonPannel(ChromiumWebBrowser brower, Action<LogLevel, string, string> logAction)
        {
            this.logAction = logAction;
            init();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabrisWinform));
            this.codemirrow = brower;
            this.codemirrow.AllowDrop = true;
            this.codemirrow.MenuHandler = new JSFunc(this);
            codemirrow.RegisterJsObject("csharpJsFunction", new JSFunc(this), new BindingOptions { CamelCaseJavascriptNames = false });

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

            manager = new RuntimeManager(new ManualManagerSettings
            {
                ScriptTimeoutMilliSeconds = 0,
                V8DebugEnabled = true,
                V8DebugPort = 9229,// 9229,
                LocalV8DebugEnabled = false
            });
            JavaScript.Manager.Tabris.Tabris.Register(manager.RequireManager, new JavaScript.Manager.Tabris.TabrisOptions
            {
                LogExecutor = new WinformLogExcutor(logAction)
            });


        }

        /// <summary>
        /// 复制选中到粘贴板
        /// </summary>
        public void CopyFromclipboard()
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                var getClipData = Clipboard.GetText();
                if (!string.IsNullOrEmpty(getClipData))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(getClipData);
                    var getClipDatabase64 = Convert.ToBase64String(plainTextBytes);
                    InvokeJS("insertCode(\"" + getClipDatabase64 + "\")");
                    OnModify();
                }
               
            }));
           
        }

        private string InvokeJS(string code)
        {
            try
            {
                JavascriptResponse task = this.codemirrow.GetMainFrame().EvaluateScriptAsync(code, null).ConfigureAwait(false).GetAwaiter().GetResult();
                return task.Success ? (task.Result.ToString() ?? "null") : task.Message;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 从粘贴板粘贴内容到编辑器
        /// </summary>
        public void PasteToclipboard()
        {
            var selectedCode = InvokeJS("getPasteCode()");
            if (!string.IsNullOrEmpty(selectedCode))
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    Clipboard.SetText(selectedCode);
                }));
           
            }
        }

        /// <summary>
        /// 删除选中
        /// </summary>
        public void DeleteSeletectd()
        {
            InvokeJS("window.cmEditor.editor.replaceSelection('')");
            OnModify();
        }

        /// <summary>
        /// 格式化选中
        /// </summary>
        public void FormatSeletectd()
        {
            InvokeJS("autoFormatSelection()");
            OnModify();
        }

        /// <summary>
        /// 注释
        /// </summary>
        public void Annotation(bool flag)
        {
            InvokeJS(flag ? "commentSelection(true)" : "commentSelection(false)");
            OnModify();
        }
        /// <summary>
        /// 是否存在有选中
        /// </summary>
        /// <returns></returns>
        public bool HaveSelected()
        {
            var selectedCode = InvokeJS("getSelectedCode()");
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
            if (sender is string)
            {
                if (string.IsNullOrEmpty(sender.ToString()))
                {
                    MessageBox.Show("执行内容为空");
                    return;
                }

                invokeJsCode(sender.ToString());
                return;
            }
            //codemirrow.ShowDevTools();
            var code = InvokeJS("getCode()");
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("执行内容为空");
                return;
            }

            invokeJsCode(code);
        }

        private void btExcutorSelected_Click(object sender, EventArgs e)
        {
            if (sender is string)
            {
                if (string.IsNullOrEmpty(sender.ToString()))
                {
                    MessageBox.Show("执行内容为空");
                    return;
                }

                invokeJsCode(sender.ToString());
                return;
            }

            var selectedCode = InvokeJS("getSelectedCode()").ToString();
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
                JavaScript.Manager.Tabris.Tabris.Register(manager.RequireManager, new JavaScript.Manager.Tabris.TabrisOptions
                {
                    LogExecutor = new WinformLogExcutor(logAction)
                });

                if (intTimeout > 0)
                {
                    logAction(LogLevel.INFO, "重新加载运行时成功,全局ScriptTimeoutMilliSeconds设置为：" + intTimeout, "");
                }
                else
                {
                    logAction(LogLevel.INFO, "重新加载运行时成功", "");
                }

            }
            catch (Exception ex)
            {
                logAction(LogLevel.ERROR, "重新加载运行时失败", ex.Message);
            }
        }

        #region JS Function

        public class JSFunc : IContextMenuHandler
        {
            enum MenuItem
            {
                ShowDevTools = 26501,
                CloseDevTools = 26502,
                Copy = 26503,
                Paste = 26504,
                Delete = 26505,
                Format = 26506,
                Annotation = 26507,
                UnAnnotation = 26508,
            }

            private readonly ButtonPannel _buttonPannel;
            public JSFunc(ButtonPannel buttonPannel)
            {
                _buttonPannel = buttonPannel;
            }

            public int SetBreakpoint(int line)
            {
                try
                {
                    if (_buttonPannel.manager.V8DebuggerEngine == null) return 1;
                    var bp = new Breakpoint
                    {
                        LineNumber = line + 3,
                        Column = 0,
                    };
                    var response = _buttonPannel.manager.V8DebuggerEngine.SetBreakpoint(bp).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (response)
                    {
                        return 1;
                    }

                    return -1;
                }
                catch (Exception ex)
                {
                    return -1;
                }

            }

            public void Modify()
            {

                _buttonPannel.OnModify();
            }

            public void ExcuteSelected(string code)
            {
                if (_buttonPannel.isRun)
                {
                    _buttonPannel.logAction(LogLevel.WARN, "请等待当前任务执行完", "");
                    return;
                }
                _buttonPannel.btExcutorSelected_Click(code, null);

            }
            public void Excute(string code)
            {
                if (_buttonPannel.isRun)
                {
                    _buttonPannel.logAction(LogLevel.WARN, "请等待当前任务执行完", "");
                    return;
                }

                _buttonPannel.btnExcutor_Click(code, null);

            }

            public bool Save(string code)
            {
                try
                {

                    if (!string.IsNullOrEmpty(_buttonPannel.fileOutPath))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            using (StreamWriter sw = new StreamWriter(_buttonPannel.fileOutPath, false))
                            {
                                sw.Write(code);
                                sw.Flush();
                            }

                            _buttonPannel.logAction(LogLevel.INFO, "保存成功", "");
                        });
                        return true;
                    }
                    this._buttonPannel.Invoke(new EventHandler(delegate
                    {
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

                            _buttonPannel.fileOutPath = jsFile.FileName;
                            _buttonPannel.logAction(LogLevel.INFO, "保存成功", "");
                        }
                    }));

                    return true;
                }
                catch (Exception ex)
                {
                    _buttonPannel.logAction(LogLevel.ERROR, "保存出错", ex.Message);
                    return true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(_buttonPannel.fileOutPath))
                    {
                        var fileNameExt = _buttonPannel.fileOutPath.Substring(_buttonPannel.fileOutPath.LastIndexOf("\\") + 1);
                        if (_buttonPannel.OnTitleChange != null) _buttonPannel.OnTitleChange(fileNameExt);
                    }

                }

            }

            #region MenuHandler

            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                //To disable the menu then call clear
                model.Clear();

                model.AddItem((CefMenuCommand)(int)MenuItem.Copy,          "复制    (Ctrl + C)");
                if (!string.IsNullOrEmpty(Clipboard.GetText()))
                {
                    model.AddItem((CefMenuCommand)(int)MenuItem.Paste,     "粘贴    (Ctrl + V)");
                }
                model.AddItem((CefMenuCommand)(int)MenuItem.Delete, "删除");
                model.AddItem((CefMenuCommand)(int)MenuItem.Format,       "格式化   (Ctrl + Alt + C)");
                
                model.AddItem((CefMenuCommand)(int)MenuItem.Annotation,   "注释     (Ctrl + /)");
                model.AddItem((CefMenuCommand)(int)MenuItem.UnAnnotation, "范注释   (Ctrl +Alt + /)");

                //Removing existing menu item
                //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

                //Add new custom menu items
                model.AddItem((CefMenuCommand)(int)MenuItem.ShowDevTools, "打开 DevTools");
                //model.AddItem((CefMenuCommand)(int)MenuItem.CloseDevTools, "关闭 DevTools");
            }

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                
                if ((int)commandId == (int)MenuItem.ShowDevTools)
                {
                    browser.ShowDevTools();
                }
                if ((int)commandId == (int)MenuItem.CloseDevTools)
                {
                    browser.CloseDevTools();
                }
                if ((int)commandId == (int)MenuItem.Copy)
                {
                    new Task(() =>
                    {
                        _buttonPannel.PasteToclipboard();
                    }).Start();
                }

                if ((int)commandId == (int)MenuItem.Paste)
                {
                    new Task(() =>
                    {
                        _buttonPannel.CopyFromclipboard();
                    }).Start(); 
                }


                if ((int)commandId == (int)MenuItem.Delete)
                {
                    new Task(() =>
                    {
                        _buttonPannel.DeleteSeletectd();
                    }).Start();
                }

                if ((int)commandId == (int)MenuItem.Format)
                {
                    new Task(() =>
                    {
                        _buttonPannel.FormatSeletectd();
                    }).Start();
                }

                if ((int)commandId == (int)MenuItem.Annotation)
                {
                    new Task(() =>
                    {
                        _buttonPannel.Annotation(true);
                    }).Start();
                }

                if ((int)commandId == (int)MenuItem.UnAnnotation)
                {
                    new Task(() =>
                    {
                        _buttonPannel.Annotation(false);
                    }).Start();
                }
                return false;
            }

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }

            #endregion
        }


        public bool Save()
        {
            try
            {
                var code = InvokeJS("getCode()");

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


        #endregion

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
                            logAction(LogLevel.ERROR, host.err, "");
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

                                        logAction(LogLevel.ERROR, ex.Message, "");
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
                    logAction(LogLevel.ERROR, ((Microsoft.ClearScript.ScriptEngineException)ex).ErrorDetails, "");
                }
                catch (Exception ex)
                {
                    logAction(LogLevel.ERROR, ex.Message, "");
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
                    logAction(LogLevel.ERROR, ex.Message, "");
                }
                else if (t.IsCanceled)
                {
                    logAction(LogLevel.ERROR, "Canclled.", "");
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
            this.btExcutorSelected.Cursor = System.Windows.Forms.Cursors.Hand;

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
            this.btnExcutor.Cursor = System.Windows.Forms.Cursors.Hand;

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
            this.SaveButton.Cursor = System.Windows.Forms.Cursors.Hand;

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
            this.reloadRuntime.Cursor = System.Windows.Forms.Cursors.Hand;


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
            try
            {
                codemirrow.CloseDevTools();
            }
            catch { }
            try
            {
                codemirrow.GetBrowser().CloseBrowser(true);
            }
            catch { }

            try
            {
                if (codemirrow != null)
                {
                    codemirrow.Dispose();

                }
            }
            catch { }
        }
    }
}