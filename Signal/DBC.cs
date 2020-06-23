using System;
using System.Runtime.InteropServices;
using DBCHandle = System.UInt32;
using CANDriverLayer;

namespace CANSignalLayer
{
    public class DBC
    {
        #region LibDBCManager.dll 静态方法声明

        public delegate void OnSend(IntPtr pContext, IntPtr pObj); //dll文件中定义的函数指针，类似于C#中的委托函数
        public delegate void OnMultiTransDone(IntPtr pContext, ref DBCMessage pMsg, IntPtr data, UInt16 nLen, Byte nDirection);

        /// <summary>
        /// DBC_Init 初始化解析模块，只需要初始化一次
        /// </summary>
        [DllImport("LibDBCManager.dll")]
        public static extern DBCHandle DBC_Init();

        [DllImport("LibDBCManager.dll")]
        public static extern void DBC_Release(DBCHandle hDBC);

        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_LoadFile(DBCHandle hDBC, ref FileInfo pFileInfo);
        
        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_GetFirstMessage(DBCHandle hDBC, IntPtr pMsg);

        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_GetNextMessage(DBCHandle hDBC, IntPtr PMsg);

        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_GetMessageById(DBCHandle hDBC, UInt32 nID, IntPtr pMsg);

        [DllImport("LibDBCManager.dll")]
        public static extern UInt32 DBC_GetMessageCount(DBCHandle hDBC);

        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_Analyse(DBCHandle hDBC, IntPtr pRecFrame, IntPtr pMessage);

        [DllImport("LibDBCManager.dll")]
        public static extern void DBC_OnReceive(DBCHandle hDBC, IntPtr ptObj);

        [DllImport("LibDBCManager.dll")]
        public static extern void DBC_SetSender(DBCHandle hDBC, OnSend sender, IntPtr pContext);

        [DllImport("LibDBCManager.dll")]
        public static extern void DBC_SetOnMultiTransDoneFunc(DBCHandle hDBC, OnMultiTransDone func, IntPtr pContext);

        [DllImport("LibDBCManager.dll")]
        public static extern bool DBC_Send(DBCHandle hDBC, IntPtr pMsg);

        #endregion

        #region 方法成员
        /// <summary>
        /// 得到DBC文件中所有的消息，返回值：消息数
        /// </summary>
        /// <param name="hDBC"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static uint GetDBCAllMessages(DBCHandle hDBC, ref DBCMessage[] messages)
        {
            uint i = 0;
            try
            {
                IntPtr ptMessage = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DBCMessage)));
                bool flag = DBC_GetFirstMessage(hDBC, ptMessage);
                messages[i] = (DBCMessage)Marshal.PtrToStructure(ptMessage, typeof(DBCMessage));
                Marshal.FreeHGlobal(ptMessage);
                Marshal.DestroyStructure(ptMessage, typeof(DBCMessage));
                i++;
                while (flag)
                {
                    ptMessage = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DBCMessage)));
                    if (!DBC_GetNextMessage(hDBC, ptMessage))
                    {
                        flag = false;
                    }
                    else
                    {
                        messages[i] = (DBCMessage)Marshal.PtrToStructure(ptMessage, typeof(DBCMessage));
                        i++;
                    }
                    Marshal.FreeHGlobal(ptMessage);
                    Marshal.DestroyStructure(ptMessage, typeof(DBCMessage));
                }
                return i;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
    }

    #region LibDBCManager.dll结构体声明
    /*
    /// MAX_FILE_PATH 最长路径 1000
    /// DBC_NAME_LENGTH 名称最长长度 127
    /// DBC_COMMENT_MAX_LENGTH 注释最长长度 200
    /// DBC_UNIT_MAX_LENGTH 单位最长长度 10 
    /// DBC_SIGNAL_MAX_COUNT 一个消息含有的信号的最大数目 512
    */
    /// <summary>
    /// 1.DBC信号属性 DBCSignal
    /// nStartBit 起始位
    /// nLen 位长度
    /// nFactor 转换因子
    /// nOffset 转换偏移值
    /// nMin 最小值
    /// nMax 最大值
    /// nValue 实际值
    /// nRawValue 原始值
    /// is_signed 1:有符号数据，0：无符号
    /// is_motorola
    /// multiplexer_type
    /// unit 单位
    /// strName 名称
    /// strComment 注释
    /// strValDesc 值描述
    /// </summary>
    public struct DBCSignal
    {
        public UInt32 nStartBit;
        public UInt32 nLen;
        public Double nFactor;
        public Double nOffset;
        public Double nMin;
        public Double nMax;
        public Double nValue;
        public UInt64 nRawValue;
        public byte is_signed;
        public byte is_motorola;
        public Byte multiplexer_type;
        public Byte val_type;
        public UInt32 multiplexer_value;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public Byte[] unit;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] strName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 201)]
        public Byte[] strComment;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] strValDesc;
    }

    /// <summary>
    /// DBC消息 DBCMessage
    /// nSignalCount 信号数量
    /// nID
    /// nExtend 1扩展帧，0标准帧
    /// nSize 消息占的字节数目
    /// DBCSignal vSignals[DBC_SIGNAL_MAX_COUNT] 信号集合
    /// strName 消息名称
    /// strComment 注释
    /// </summary>
    public struct DBCMessage
    {
        public UInt32 nSignalCount;
        public UInt32 nID;
        public UInt32 nSize;
        public double cycleTime;
        public byte nExyended;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public DBCSignal[] vSignals;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] strName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 201)]
        public Byte[] strComment;
    }

    /// <summary>
    /// 文件信息作为初始化参数，FileInfo
    /// strFilePath 文件的绝对路径
    /// type 指明DBC文件的协议类型
    /// </summary>
    public struct FileInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 261)]
        public Char[] strFilePath;

        public Byte type; //0 J1939
    }
    public struct Ctx
    {
        public UInt32 DeviceType;
        public UInt32 DeviceIndex;
        public UInt32 CANChannelIndex;
    };
    #endregion

}
