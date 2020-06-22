using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CANDriverLayer;
using CANSignalLayer;
using CSVFileOperationPart;

namespace ZHISIGHT
{
    public partial class Form1 : Form
    {
        #region 构造函数
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region 变量成员
        CSVFileOperation csv;
        private uint countDevice = 0;
        private ICANDriver intfCANDriver;
        private ICANSignal intfCANSignal;
        private FileInfo dbcFileInfo;
        private Dictionary<uint, int> DevInd_Sts = new Dictionary<uint, int>();
        private ConfigFileForm configFileForm;
        CommunicationForm commuForm;

        #endregion

        #region 封装字段
        public ICANDriver IntfCANDriver { get => intfCANDriver; set => intfCANDriver = value; }
        public ICANSignal IntfCANSignal { get => intfCANSignal; set => intfCANSignal = value; }
        public FileInfo DBCFileInfo { get => dbcFileInfo; set => dbcFileInfo = value; }
        public CommunicationForm CommuForm { get => commuForm; set => commuForm = value; }
        internal CSVFileOperation Csv { get => csv; set => csv = value; }
        public uint CountDevice { get => countDevice; set => countDevice = value; }
        #endregion

        #region 方法成员
        private void Form1_Load(object sender, EventArgs e)
        {
            CommuForm = new CommunicationForm();
            configFileForm = new ConfigFileForm();
        }

        private void 加载DBCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configFileForm.ShowDialog(this);
        }

        private void AddChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (configFileForm.StrCSVPath == null || configFileForm.StrDBCPath == null)
            {
                MessageBox.Show("请先加载配置等文件！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dbcFileInfo.strFilePath = configFileForm.StrDBCPath.PadRight(261, '\0').ToCharArray(); //dbc 路径和名
            dbcFileInfo.type = 1;//other NOt J1939      （DBCFileName and Type） 
            csv = new CSVFileOperation(configFileForm.StrCSVPath);
            csv.OpenCSVtoDataTable();

            intfCANDriver = new CANDriver(commuForm.DevType, commuForm.DevInd, commuForm.CanInd, commuForm.PInitConfig);
            Ctx ctx = new Ctx();
            ctx.CANChannelIndex = commuForm.CanInd;
            ctx.DeviceIndex = commuForm.DevInd;
            ctx.DeviceType = commuForm.DevType;
            intfCANSignal = new CANSignal(dbcFileInfo, IntfCANDriver, csv, ctx); //连接之后做的事情 创建Signal对象
            MyTabPage myTabPage = new MyTabPage(intfCANSignal, intfCANDriver, csv);
            myTabPage.Text = "Device" + commuForm.DevInd.ToString() + "-CANInd" + commuForm.CanInd.ToString();
            myTabPage.Name = "TabPage" + myTabPage.Text;

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (myTabPage.Text == tabPage.Text)
                {
                    return;
                }
            }
            if (DevInd_Sts == null || !DevInd_Sts.Keys.Contains(commuForm.DevInd))
            {
                if (intfCANDriver.Open() == 1)
                {
                    DevInd_Sts.Add(commuForm.DevInd, 1);
                    MessageBox.Show("设备打开成功", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("设备打开失败，请检查设备！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                intfCANDriver.SetDeviceOpenStatus();
                MessageBox.Show("设备已成功打开！", "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }

            tabControl1.Controls.Add(myTabPage);
        }

        private void USBCANIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //创建配置窗体
            if (commuForm != null)
            {
                CommuForm.ShowDialog(this);
            }
            else
            {
                commuForm = new CommunicationForm();
                commuForm.ShowDialog(this);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                foreach (TabPage tabPage in tabControl1.TabPages)               //为每一个开启的设备关闭
                {
                    MyTabPage myTabPage = tabPage as MyTabPage;
                    myTabPage.IntfCANDriver.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// 自定义的MyTextBox控件
    /// </summary>
    class MyTextBox : TextBox
    {
        private string strValue = "";

        public string StrValue { get => strValue; set => strValue = value; }
    }

    class MyTabPage : TabPage       //继承的TabPage 主要用来存储 每一个TabPage对应的设备通道及文件信息。
    {
        #region 变量成员
        private ICANSignal intfCANSignal;
        private ICANDriver intfCANDriver;
        private CSVFileOperation cSV;
        private GroupBox recGroup = new GroupBox();
        private GroupBox transGroup = new GroupBox();
        private SplitContainer splitContainer = new SplitContainer();
        private ToolStripButton toolStripButton = new ToolStripButton();
        private ToolStrip toolStrip = new ToolStrip();
        private Timer timer0 = new Timer();
        #endregion

        #region 封装字段
        public ICANSignal IntfCANSignal { get => intfCANSignal; set => intfCANSignal = value; }
        public ICANDriver IntfCANDriver { get => intfCANDriver; set => intfCANDriver = value; }
        public CSVFileOperation CSV { get => cSV; set => cSV = value; }
        public GroupBox RecGroup { get => recGroup; set => recGroup = value; }
        public GroupBox TransGroup { get => transGroup; set => transGroup = value; }
        public SplitContainer SplitContainer { get => splitContainer; set => splitContainer = value; }
        public ToolStripButton ToolStripButton { get => toolStripButton; set => toolStripButton = value; }
        public ToolStrip ToolStrip { get => toolStrip; set => toolStrip = value; }
        public Timer Timer0 { get => timer0; set => timer0 = value; }
        #endregion

        #region 构造函数
        public MyTabPage(ICANSignal iCANSignal, ICANDriver iCANDriver, CSVFileOperation csv)
        {
            intfCANDriver = iCANDriver;
            intfCANSignal = iCANSignal;
            cSV = csv;

            toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            recGroup.SuspendLayout();
            transGroup.SuspendLayout();
            //
            //Timer                             //定时器
            //
            timer0.Enabled = true;
            timer0.Interval = 100;
            timer0.Tick += Timer0_Tick;
            // 
            // RecGroup
            // 
            recGroup.AutoSize = true;
            recGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            recGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            recGroup.Location = new System.Drawing.Point(0, 0);
            recGroup.Name = "RecGroup";
            recGroup.Size = new System.Drawing.Size(642, 754);
            recGroup.TabStop = false;
            recGroup.Text = "Received";
            AddControls(recGroup);
            // 
            // TransGroup
            // 
            transGroup.AutoSize = true;
            transGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            transGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            transGroup.Location = new System.Drawing.Point(0, 0);
            transGroup.Name = "TransGroup";
            transGroup.Size = new System.Drawing.Size(681, 754);
            transGroup.TabStop = false;
            transGroup.Text = "Transmitted";
            AddControls(transGroup);
            // 
            // toolStripButton
            // 
            toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton.Name = "toolStripButton";
            toolStripButton.Size = new System.Drawing.Size(50, 28);
            toolStripButton.Text = "连接";
            toolStripButton.Click += ToolStripButton_Click;
            // 
            // toolStrip
            // 
            toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripButton});
            toolStrip.Location = new System.Drawing.Point(3, 3);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new System.Drawing.Size(1327, 33);
            toolStrip.Text = "toolStrip";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(this.RecGroup);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(this.TransGroup);
            splitContainer.Size = new System.Drawing.Size(1327, 754);
            splitContainer.SplitterDistance = 642;
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(3, 36);
            splitContainer.Name = "splitContainer1";
            // 
            // tabpage添加控件
            // 
            Controls.Add(this.splitContainer);
            Controls.Add(this.toolStrip);
            Location = new System.Drawing.Point(4, 28);
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(1333, 793);
            UseVisualStyleBackColor = true;
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel1.PerformLayout();
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            splitContainer.ResumeLayout(false);
            splitContainer.PerformLayout();
            recGroup.ResumeLayout(false);
            recGroup.PerformLayout();
            transGroup.ResumeLayout(false);
            transGroup.PerformLayout();
        }
        #endregion

        #region 成员方法
        private void Timer0_Tick(object sender, EventArgs e)
        {
            MyTextBox textBox1;
            double value;
            //获取TransGroup中的所有的控件并发送
            foreach (var textBox in TransGroup.Controls)
            {
                if (textBox is MyTextBox)
                {
                    bool flag;
                    textBox1 = textBox as MyTextBox;
                    string[] str = textBox1.Name.Trim().Split(' ');     //str[0]: MessageID, str[1]: SiganlName
                    try
                    {
                        value = Convert.ToDouble(textBox1.StrValue);
                        flag = IntfCANSignal.SetSignalByNameFromApp(Convert.ToUInt32(str[0]), System.Text.Encoding.UTF8.GetBytes(str[1]), value);
                        if (!flag)
                        {
                            timer0.Stop();
                            MessageBox.Show("信号: " + str[1] + " 不存在,请确保配置文件和DBC文件中的信号、所属消息ID相同！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        textBox1.Text = textBox1.StrValue;
                    }
                }
            }

            //更新Received中的控件信息
            foreach (var textBox in RecGroup.Controls)
            {
                if (textBox is MyTextBox)
                {
                    textBox1 = textBox as MyTextBox;
                    string[] str = textBox1.Name.Trim().Split(' ');     //str[0]: MessageID, str[1]: SiganlName
                    textBox1.Text = IntfCANSignal.GetSignalByNameToApp(Convert.ToUInt32(str[0]), System.Text.Encoding.UTF8.GetBytes(str[1])).ToString();
                }
            }
        }

        private void ToolStripButton_Click(object sender, EventArgs e)
        {
            //开启定时器，打开CAN通道，开启接发线程
            if (toolStripButton.Text == "连接")
            {
                if (intfCANDriver.Init() == 1 && intfCANDriver.Start() == 1)
                {
                    toolStripButton.Text = "断开连接";
                    MessageBox.Show("通道打开成功！", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //开启定时器
                    intfCANSignal.StartTimer();
                    timer0.Start();
                }
                else
                {
                    MessageBox.Show("通道打开失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                //关闭定时器                
                intfCANSignal.StopTimer();
                timer0.Stop();
                IntfCANDriver.Reset();
                toolStripButton.Text = "连接";
                MessageBox.Show("通道关闭成功！", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddControls(GroupBox groupBox)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < cSV.DataTable.Rows.Count; i++)
            {
                if (list.Contains(cSV.DataTable.Rows[i]["MessageID"].ToString()))
                {
                    continue;
                }
                list.Add(cSV.DataTable.Rows[i]["MessageID"].ToString());
            }           //获取csv中message种类

            int xPosition = 20;
            int yPosition = 20;
            int count = 0;
            for (int i = 0; i < list.Count; i++)                                //自动生成控件
            {
                DataRow[] dataRows = new DataRow[100];
                string str0 = "GroupName = '" + groupBox.Text + "'" + " and MessageID = '" + list[i] + "'";
                dataRows = cSV.DataTable.Select(str0);

                if (dataRows.Count() != 0)
                {
                    if (yPosition > 520)
                    {
                        count++;
                        xPosition = 20 + 250 * count;
                        yPosition = 20;
                    }
                    Label label = new Label
                    {
                        AutoSize = true,
                        Location = new System.Drawing.Point(xPosition, yPosition),   //添加message控件
                        ForeColor = Color.Blue
                    };
                    label.Name = "label" + list[i];
                    label.Size = new System.Drawing.Size(62, 18);
                    label.Text = list[i];
                    groupBox.Controls.Add(label);
                    yPosition += 25;

                    for (int j = 0; j < dataRows.Length; j++)
                    {
                        if (yPosition > 520)
                        {
                            count++;
                            yPosition = 20;
                            xPosition = 20 + 250 * count;
                        }
                        Label labels = new Label
                        {
                            AutoSize = true,
                            Location = new System.Drawing.Point(xPosition, yPosition + 3),   //添加signal控件
                            Name = "label" + dataRows[j]["SignalName"].ToString(),
                            Size = new System.Drawing.Size(62, 18),
                            Text = dataRows[j]["SignalName"].ToString()
                        };
                        groupBox.Controls.Add(labels);

                        if (yPosition > 520)
                        {
                            count++;
                            yPosition = 20;
                            xPosition = 20 + 250 * count;
                        }
                        MyTextBox textBox = new MyTextBox
                        {
                            Location = new System.Drawing.Point(xPosition + ((labels.Size.Width + 10) > 130 ? (labels.Size.Width + 10) : 130), yPosition),
                            Name = list[i] + " " + dataRows[j]["SignalName"].ToString(),
                            Size = new System.Drawing.Size(50, 28),
                            Text = "0",
                            StrValue = "0",
                        };
                        if (groupBox.Text == "Transmitted")
                        {
                            textBox.KeyPress += TextBox_KeyPress; ;
                        }

                        groupBox.Controls.Add(textBox);

                        yPosition += 25;
                    }
                }
            }
        }//end Add Controls  

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            MyTextBox myTextBox = sender as MyTextBox;
            if (e.KeyChar == '\r')
            {
                myTextBox.BackColor = Color.White;
                try
                {
                    myTextBox.StrValue = Convert.ToDouble(myTextBox.Text).ToString();
                }
                catch (Exception)
                {
                    myTextBox.Text = myTextBox.StrValue;
                }
            }
            else
            {
                myTextBox.BackColor = Color.Yellow;
            }
        }
        #endregion
    }
}
