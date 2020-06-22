using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZLGCANDriver
{
    #region 结构体定义
    [StructLayout(LayoutKind.Sequential)]
    public struct ZCAN
    {
        public uint acc_code;
        public uint acc_mask;
        public uint reserved;
        public byte filter;
        public byte timing0;
        public byte timing1;
        public byte mode;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CANFD
    {
        public uint acc_code;
        public uint acc_mask;
        public uint abit_timing;
        public uint dbit_timing;
        public uint brp;
        public byte filter;
        public byte mode;
        public UInt16 pad;
        public uint reserved;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct ZCAN_CHANNEL_INIT_CONFIG
    {
        [FieldOffset(0)]
        public uint can_type; //type:0 TYPE_CAN  1TYPE_CANFD

        [FieldOffset(4)]
        public ZCAN can;

        [FieldOffset(4)]
        public CANFD canfd;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct can_frame
    {
        public uint can_id;  /* 32 bit MAKE_CAN_ID + EFF/RTR/ERR flags */
        public byte can_dlc; /* frame payload length in byte (0 .. CAN_MAX_DLEN) */
        public byte __pad;   /* padding */
        public byte __res0;  /* reserved / padding */
        public byte __res1;  /* reserved / padding */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] data/* __attribute__((aligned(8)))*/;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct canfd_frame
    {
        public uint can_id;  /* 32 bit MAKE_CAN_ID + EFF/RTR/ERR flags */
        public byte len;     /* frame payload length in byte */
        public byte flags;   /* additional flags for CAN FD,i.e error code */
        public byte __res0;  /* reserved / padding */
        public byte __res1;  /* reserved / padding */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] data/* __attribute__((aligned(8)))*/;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ZCAN_Receive_Data
    {
        public can_frame frame;
        public UInt64 timestamp;//us
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct ZCAN_Transmit_Data
    {
        public can_frame frame;
        public UInt32 transmit_type;
    };

    #endregion
    public class ZlgCANDriver : ICANDriver
    {


        #region zlgcandriver.dll
        // zlgcan.dll
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ZCAN_OpenDevice(uint device_type, uint device_index, uint reserved);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_CloseDevice(IntPtr device_handle);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ZCAN_InitCAN(IntPtr device_handle, uint can_index, IntPtr pInitConfig);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_StartCAN(IntPtr channel_handle);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_ResetCAN(IntPtr channel_handle);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_Transmit(IntPtr channel_handle, IntPtr pTransmit, uint len);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_GetReceiveNum(IntPtr channel_handle, byte type);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZCAN_Receive(IntPtr channel_handle, IntPtr data, uint len, int wait_time = -1);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetIProperty(IntPtr device_handle);

        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint ReleaseIProperty(IntPtr pIProperty);
        #endregion
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SetValueFunc(string path, string value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string GetValueFunc(string path, string value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetPropertysFunc(string path, string value);

        public struct IProperty
        {
            public SetValueFunc SetValue;
            public GetValueFunc GetValue;
            public GetPropertysFunc GetPropertys;
        };

        private UInt32 canInd = 0;//通道
        private UInt32 devInd = 0;//设备索引
        private UInt32 devType = 4;//设备类型
        private Int32 isDeviceOpen = 0;//设备是否开启
        private Int32 operationStatus = 0;
        private UInt32[] isChannelOpen = new uint[2] { 0, 0 };//通道一是否开启
        private ZCAN_CHANNEL_INIT_CONFIG pInitConfig;

        private IntPtr deviceHandle = new IntPtr(1);
        private IntPtr initCANHandle = new IntPtr(1);

        public ZlgCANDriver(uint devType, uint devInd, uint canInd, ZCAN_CHANNEL_INIT_CONFIG pInitConfig)
        {
            this.canInd = canInd;
            this.devInd = devInd;
            this.devType = devType;
            this.pInitConfig = pInitConfig;
        }

        #region 封装字段
        public uint DevType { get => devType; set => devType = value; }
        public uint DevInd { get => devInd; set => devInd = value; }
        public uint CanInd { get => canInd; set => canInd = value; }
        public int IsDeviceOpen { get => isDeviceOpen; set => isDeviceOpen = value; }
        public uint[] IsChannelOpen { get => isChannelOpen; set => isChannelOpen = value; }
        public ZCAN_CHANNEL_INIT_CONFIG PInitConfig { get => pInitConfig; set => pInitConfig = value; }
        public Int32 OperationStatus { get => operationStatus; set => operationStatus = value; }
        #endregion

        #region 接口实现成员
        public int Open()
        {
            deviceHandle = ZCAN_OpenDevice(this.devType, this.devInd, 0);
            if ((int)deviceHandle == 0)
            {
                MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return 0;
            }

            return (int)deviceHandle;
        }

        public int Close()
        {
            ZCAN_CloseDevice(this.deviceHandle);
            return 1;
        }

        public int Init()
        {
            //IntPtr property = GetIProperty(deviceHandle);
            //if (null == property)
            //{
            //    //AddData(_T("获取属性失败"));
            //    return 0;
            //}

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pInitConfig));
            Marshal.StructureToPtr(pInitConfig, ptr, true);

            initCANHandle = ZCAN_InitCAN(this.deviceHandle, this.canInd, ptr);
            Marshal.FreeHGlobal(ptr);
            Marshal.DestroyStructure(ptr, typeof(ZCAN_CHANNEL_INIT_CONFIG));
            return (int)initCANHandle;

        }

        public int Start()
        {
            if (initCANHandle == null)
            {
                return 0;
            }
            if (ZCAN_StartCAN(initCANHandle) != 1) //STATUS_OK 1
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 接收函数，传递参数为
        /// </summary>
        /// <param name="pRecFrameBufferFromCANSignal"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public uint Receive(ref ZCAN_Receive_Data[] receiveData)
        {
            if (IsDeviceOpen == 0)
            {
                return 0;
            }
            UInt32 res;
            UInt32 max = 100;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ZCAN_Receive_Data)) * (int)max);
            res = ZCAN_GetReceiveNum(this.initCANHandle, (byte)pInitConfig.can_type);
            if (res == 0)
            {
                return 0;
            }
            res = res > 100 ? 100 : res;
            res = ZCAN_Receive(initCANHandle, pt, res, 50);
            for (int i = 0; i < res; i++)
            {
                receiveData[i] = (ZCAN_Receive_Data)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(ZCAN_Receive_Data))), typeof(ZCAN_Receive_Data));
            }
            Marshal.DestroyStructure(pt, typeof(ZCAN_Receive_Data));
            Marshal.FreeHGlobal(pt);

            return res;
        }

        /// <summary>
        /// 发送函数，pTxFrameBufferFromCANSignal 帧数据， len :本次需传输的帧长， 返回值: 发送成功的帧长
        /// </summary>
        /// <param name="pTxFrameBufferFromCANSignal"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public uint Transmit(ZCAN_Transmit_Data transmitData, uint len)
        {
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ZCAN_Transmit_Data)));
            Marshal.StructureToPtr(transmitData, pt, true);
            uint res = ZCAN_Transmit(this.initCANHandle, pt, len);
 
            return res; //0时，没有成功发送数据
        }

        public int Reset()
        {
            throw new NotImplementedException();
        }

        public int ReadError()
        {
            throw new NotImplementedException();
        }

        public void SetDeviceOpenStatus()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
