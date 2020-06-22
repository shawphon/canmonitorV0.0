namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCANIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载DBCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.加载DBCToolStripMenuItem,
            this.ConToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1341, 32);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSBCANIIToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(98, 28);
            this.toolStripMenuItem1.Text = "选择设备";
            // 
            // uSBCANIIToolStripMenuItem
            // 
            this.uSBCANIIToolStripMenuItem.Name = "uSBCANIIToolStripMenuItem";
            this.uSBCANIIToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.uSBCANIIToolStripMenuItem.Text = "USBCAN";
            this.uSBCANIIToolStripMenuItem.Click += new System.EventHandler(this.USBCANIIToolStripMenuItem_Click);
            // 
            // 加载DBCToolStripMenuItem
            // 
            this.加载DBCToolStripMenuItem.Name = "加载DBCToolStripMenuItem";
            this.加载DBCToolStripMenuItem.Size = new System.Drawing.Size(99, 28);
            this.加载DBCToolStripMenuItem.Text = "加载DBC";
            this.加载DBCToolStripMenuItem.Click += new System.EventHandler(this.加载DBCToolStripMenuItem_Click);
            // 
            // ConToolStripMenuItem
            // 
            this.ConToolStripMenuItem.Name = "ConToolStripMenuItem";
            this.ConToolStripMenuItem.Size = new System.Drawing.Size(98, 28);
            this.ConToolStripMenuItem.Text = "添加通道";
            this.ConToolStripMenuItem.Click += new System.EventHandler(this.AddChannelToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1341, 825);
            this.tabControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1341, 857);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uSBCANIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载DBCToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem ConToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
    }
}

