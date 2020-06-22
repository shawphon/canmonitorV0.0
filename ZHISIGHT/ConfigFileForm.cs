using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZHISIGHT
{
    public partial class ConfigFileForm : Form
    {
        #region 字段
        private string strCSVPath;
        private string strDBCPath;
        #endregion
        
        public string StrCSVPath { get => strCSVPath; set => strCSVPath = value; }
        public string StrDBCPath { get => strDBCPath; set => strDBCPath = value; }

        public ConfigFileForm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "DBC文件(*.dbc)|*.dbc";
            openFileDialog1.Title = "选择DBC文件";
            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog(this);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Dispose();
            openFileDialog2.Dispose();
            this.Visible = false;
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "CSV文件(*.csv)|*.csv";
            openFileDialog2.Title = "选择配置文件";
            openFileDialog2.Multiselect = false;
            openFileDialog2.ShowDialog(this);
        }

        private void OpenFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            strCSVPath = string.Join(System.IO.Path.GetDirectoryName(openFileDialog2.FileName), openFileDialog2.FileName); //路径和名          
            textBox2.Text = strCSVPath;
        }

        private void OpenFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {
            strDBCPath = string.Join(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), openFileDialog1.FileName); //路径和名          
            textBox1.Text = strDBCPath;
        }

        private void ConfigFileForm_Load(object sender, EventArgs e)
        {

        }
    }
}
