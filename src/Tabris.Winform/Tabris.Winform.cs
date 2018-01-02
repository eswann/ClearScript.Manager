using DSkin.Common;
using DSkin.DirectUI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tabris.Winform.Control;

namespace Tabris.Winform
{
    public partial class TabrisWinform : DSkin.Forms.DSkinForm
    {
        DuiButton addButton = new DuiButton { NormalImage = Properties.Resources.ChromeAdd, HoverImage = Properties.Resources.ChromeAddHover, Margin = new Padding(0, 3, 0, 0), Name = "add", MouseEventBubble = false };
        private string tabrisUrl = string.Empty;
        public TabrisWinform()
        {
            InitializeComponent();

            var domanPath = AppDomain.CurrentDomain.BaseDirectory;
            var tabrisFolder = Path.Combine(domanPath, "tabris");
            if (!Directory.Exists(tabrisFolder))
            {
                throw new FileNotFoundException(tabrisFolder);
            }
            var indexFile = Path.Combine(tabrisFolder, "tabris.html");
            if (!File.Exists(indexFile))
            {
                throw new FileNotFoundException(indexFile);
            }

            tabrisUrl = "file:///" + indexFile;

            this.MinimumSize = new Size(770, 570);

        }


        private void TabrisWinform_Load(object sender, EventArgs e)
        {
            dSkinTabBar1.Items.ItemAdded += Items_ItemAdded;
            dSkinTabBar1.Items.ItemRemoved += Items_ItemRemoved;
            dSkinTabControl1.ItemSize = new Size(1, 1);
            dSkinTabBar1.Items.Add(addButton);
            addButton.MouseClick += add_MouseClick;
            dSkinTabBar1.InnerDuiControl.MouseDown += InnerDuiControl_MouseDown;

            add_MouseClick(null, null);
        }
        void InnerDuiControl_MouseDown(object sender, DuiMouseEventArgs e)
        {
            DSkin.NativeMethods.MouseToMoveControl(this.Handle);
        }
        void Items_ItemRemoved(object sender, CollectionEventArgs<DuiBaseControl> e)
        {
            if (dSkinTabBar1.Items.Count == 1)
            {
                addButton.Margin = new Padding(0, 3, 0, 0);
            }
            else
            {
                addButton.Margin = new Padding(-2, 3, 0, 0);
            }
            SetItemSize();
        }

        void Items_ItemAdded(object sender, CollectionEventArgs<DuiBaseControl> e)
        {
            SetItemSize();
        }
        void add_MouseClick(object sender, DuiMouseEventArgs e)
        {
            var index = dSkinTabBar1.Items.IndexOf(addButton);
            TabrisTabItem item = new TabrisTabItem()
            {
                Text = "new " + (index + 1),
                Image = Properties.Resources.JSS
            };
            dSkinTabBar1.Items.Insert(index, item);
            //item.SendToBack();
            //DSkin.Controls.DSkinBaseControl db = new DSkin.Controls.DSkinBaseControl { Dock = DockStyle.Fill };
            DSkin.Controls.DSkinBrowser brower = new DSkin.Controls.DSkinBrowser
            {
                Dock = DockStyle.Fill,
                Url = tabrisUrl,
                ContextMenuStrip = this.codemirrowMenu
            };
            //db.DUIControls.Add(d);
            TabPage page = new TabPage();
            page.Controls.Add(brower);
            item.TabPage = page;
            dSkinTabControl1.TabPages.Add(page);
            dSkinTabBar1.LayoutContent();
            dSkinTabBar1.SetSelect(item);
            
            LogPannel logPannel = new LogPannel();
            ButtonPannel buttonPannel = new ButtonPannel(brower, logPannel.Log)
            {
                Index = index,
                OnTitleChange = s =>
                {
                    item.Text = s;
                    var tag = item.Tag as TabrisControlContainer;
                    if (tag != null)
                    {
                        tag.TagName = s;
                    }
                },
                OnModify = () =>
                {
                    var tag = item.Tag as TabrisControlContainer;
                    if (tag != null)
                    {
                        item.Text = " * " + tag.TagName;
                    }
                    
                }
            };

            this.dSkinPanel3.Controls.Add(buttonPannel);
            this.dSkinPanel1.Controls.Add(logPannel);
            item.Tag = new TabrisControlContainer
            {
                ButtonPannel = buttonPannel,
                LogPannel = logPannel,
                TagName = item.Text
            };
        }
       
        private void dSkinTabBar1_SizeChanged(object sender, EventArgs e)
        {
            SetItemSize();
        }
        private void SetItemSize()
        {
            if (dSkinTabBar1.Items.Count > 1)
            {
                int w = 150;
                if ((dSkinTabBar1.Items.Count - 1) * 150 + 15 > dSkinTabBar1.Width)
                {
                    w = (dSkinTabBar1.Width - 45) / (dSkinTabBar1.Items.Count - 1) + 13;
                }
                foreach (DuiBaseControl item in dSkinTabBar1.Items)
                {
                    if (item is TabrisTabItem)
                    {
                        item.Width = w;
                    }
                }
            }
        }

    

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetSelectedTabrisControlContainer()?.ButtonPannel.PasteToclipboard();
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetSelectedTabrisControlContainer()?.ButtonPannel.CopyFromclipboard();
        }

        private void codemirrowMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var seleted = GetSelectedTabrisControlContainer();
            if (seleted == null )
            {
                return;
            }

            if (seleted.ButtonPannel.HaveSelected())
            {
                复制ToolStripMenuItem.Text = "复制";
            }
            else
            {
                复制ToolStripMenuItem.Text = "复制全部";
            }

            if (string.IsNullOrEmpty(Clipboard.GetText()))
            {
                粘贴ToolStripMenuItem.Enabled = false;
            }

        }

        private TabrisControlContainer GetSelectedTabrisControlContainer()
        {
            try
            {
                if (this.dSkinTabBar1.Items.Count == 1) return null;
                var selectItem = this.dSkinTabBar1.Items[this.dSkinTabBar1.SelectedIndex];
                return selectItem.Tag as TabrisControlContainer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void codemirrowMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            复制ToolStripMenuItem.Text = "复制";
            粘贴ToolStripMenuItem.Enabled = true;
        }
    }


}
