using System.Data;
using System.IO;
using System.Windows.Forms;

namespace CSVFileOperationPart
{
    public class CSVFileOperation
    {
        #region 成员变量
        private string filePath;
        private DataTable dataTable;
        private int flagColumnWritten = 0;
        #endregion

        #region 封装字段
        public string FilePath { get => filePath; set => filePath = value; }
        public DataTable DataTable { get => dataTable; set => dataTable = value; }
        public int FlagColumnWritten { get => flagColumnWritten; set => flagColumnWritten = value; }
        #endregion

        #region 构造函数
        public CSVFileOperation(string filePath)
        {
            this.filePath = filePath;
            this.dataTable = new DataTable();
        }
        #endregion

        #region 方法成员
        #region ...
        /// <summary>
        /// 读取CSV
        /// </summary>
        /// <param name="filePath">CSV路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        /// <returns></returns>
        #endregion
        public void SaveCSV()//这边使用的是DataTable的数据类型，这边应该修改为List的数据类型进行写入到File文件中
        {
            if (dataTable == null || dataTable.Rows.Count == 0)   //确保DataTable中有数据
                return;
            string strBufferLine = "";
            try
            {

                StreamWriter strmWriterObj = new StreamWriter(filePath, true, System.Text.Encoding.Default);

                if (flagColumnWritten == 0)
                {
                    foreach (System.Data.DataColumn col in dataTable.Columns)    //写入列头
                        strBufferLine += col.ColumnName + ",";
                    strBufferLine = strBufferLine.Substring(0, strBufferLine.Length - 1);//将逗号给去掉
                    strmWriterObj.WriteLine(strBufferLine);
                    flagColumnWritten++;
                }

                for (int i = 0; i < dataTable.Rows.Count; i++)   //写入记录的数据
                {
                    strBufferLine = "";
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (j > 0)
                            strBufferLine += ",";
                        strBufferLine += dataTable.Rows[i][j].ToString().Replace(",", "");   //因为CSV文件以逗号分割，在这里替换为空，以免冲突
                    }
                    strmWriterObj.WriteLine(strBufferLine);
                }
                strmWriterObj.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File" + filePath + "is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OpenCSVtoDataTable(int n = 0)
        {
            int m = 0;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8);

                string str = "";
                str = reader.ReadLine();
                while (str != null)
                {
                    string[] split = str.Split(',');
                    if (m == n)
                    {
                        System.Data.DataColumn column; //列名
                        for (int c = 0; c < split.Length; c++)
                        {
                            column = new System.Data.DataColumn
                            {
                                DataType = System.Type.GetType("System.String"),
                                ColumnName = split[c]
                            };
                            if (dataTable.Columns.Contains(split[c]))                 //重复列名处理
                                column.ColumnName = split[c] + c.ToString();
                            dataTable.Columns.Add(column);
                        }
                    }
                    if (m > n)
                    {
                        System.Data.DataRow dr = dataTable.NewRow();
                        for (int i = 0; i < split.Length; i++)
                        {
                            dr[i] = split[i];
                        }
                        dataTable.Rows.Add(dr);
                    }
                    str = reader.ReadLine();
                    m += 1;
                }
                reader.Close();
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File" + filePath + "is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
