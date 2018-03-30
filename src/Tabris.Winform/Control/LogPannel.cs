//-----------------------------------------------------------------------
// <copyright file="LogPannel.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Tabris.Winform.Control
{
    using DSkin.DirectUI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// 每个Tab对应一个LogPannel
    /// </summary>
    public class LogPannel : DSkin.Controls.DSkinPanel
    {
        public event EventHandler OnLoging;
        private DSkin.Controls.DSkinListBox logList;
        public LogPannel()
        {
            this.logList = new DSkin.Controls.DSkinListBox();

            ((System.ComponentModel.ISupportInitialize)(this.logList)).BeginInit();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabrisWinform));

            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.logList);
            this.Location = new System.Drawing.Point(4, 440);
            this.RightBottom = ((System.Drawing.Image)(resources.GetObject("dSkinPanel1.RightBottom")));
            this.Size = new System.Drawing.Size(762, 179);


            // 
            // logList
            // 
            this.logList.BackColor = System.Drawing.Color.Transparent;
            this.logList.Borders.TopColor = System.Drawing.SystemColors.ControlDarkDark;
            this.logList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logList.Location = new System.Drawing.Point(0, 0);
            this.logList.ScrollBarWidth = 12;
            this.logList.Size = new System.Drawing.Size(762, 179);
            this.logList.Value = 0D;

        }

        public void LogClear()
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                try
                {
                    logList.Items.Clear();
                }
                catch (Exception)
                {
                    
                }

            }));
        }

        public void Log(LogLevel level, string msgStr, string trace = null)
        {
           
            var msgAll = msgStr + trace;
            if (OnLoging != null)
                OnLoging(new LogEventModel
                {
                    LogLevel = level,
                    Message = msgAll
                }, new EventArgs());

            try
            {
                this.BeginInvoke(new EventHandler(delegate
                   {
                       try
                       {
                           foreach (var msg in Split(msgAll, 70))
                           {
                               var levelStr = GetDescription(level);
                               if (level.Equals(LogLevel.ERROR))
                               {
                                   logList.Items.Add(new DuiHtmlLabel
                                   {
                                       Text = string.Format("&nbsp;&nbsp; <label color='red'>[{0:yyyy-MM-dd HH:mm:ss} {1}]--------{2} </label>", DateTime.Now, levelStr, msg),
                                       AutoSize = true
                                   });

                               }
                               else if (level.Equals(LogLevel.WARN))
                               {
                                   logList.Items.Add(new DuiHtmlLabel
                                   {
                                       Text = string.Format("&nbsp;&nbsp; <label color='blue'>[{0:yyyy-MM-dd HH:mm:ss} {1}]--------{2} </label>", DateTime.Now, levelStr, msg),
                                       AutoSize = true
                                   });
                               }
                               else
                               {
                                   logList.Items.Add(new DuiHtmlLabel
                                   {
                                       Text = string.Format("&nbsp;&nbsp; [{0:yyyy-MM-dd HH:mm:ss} {1}]--------{2}", DateTime.Now, levelStr, msg),
                                       AutoSize = true
                                   });
                               }
                           }

                           SetTimeout(100, () =>
                           {
                               logList.Value = 1;
                           });
                       }
                       catch (Exception)
                       {
                           logList.Items.Add(new DuiTextBox()
                           {
                               Text = msgAll,
                               Width = 800
                           });
                       }


                   }));

            }
            catch (Exception)
            {

            }
           

        }

        private string GetDescription(System.Enum value, Boolean nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }


        /// <summary>
        /// 在指定时间过后执行指定的表达式
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        private void SetTimeout(double interval, Action action)
        {
            System.Timers.Timer timer = new System.Timers.Timer(interval);
            timer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                timer.Enabled = false;
                action();
            };
            timer.Enabled = true;
        }

        private IEnumerable<string> Split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }
    }
}