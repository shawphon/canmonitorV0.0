using System;
using ZLGCANDriver;
using System.Threading;
using System.Runtime.InteropServices;
using CSVFileOperationPart;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ZCAN_CHANNEL_INIT_CONFIG initConfig = new ZCAN_CHANNEL_INIT_CONFIG();
            initConfig.can_type = 0;
            ZCAN zCAN = new ZCAN();
            zCAN.acc_code = Convert.ToUInt32("0x" + "00000000", 16);
            zCAN.acc_mask = Convert.ToUInt32("0x" + "FFFFFFFF", 16);
            zCAN.mode = 0;
            zCAN.timing0 = 0x01;
            zCAN.timing1 = 0x1C;


            ZlgCANDriver cANDriver = new ZlgCANDriver(21, 0, 0, initConfig);

            cANDriver.Open();
            cANDriver.Init();
            cANDriver.Start();
            ZCAN_Transmit_Data obj = new ZCAN_Transmit_Data();
            can_frame frame = new can_frame();
            obj.transmit_type = 0; //0=正常发送，1=单次发送，2=自发自收，3=单次自发自收
            frame.can_id = 8;
            frame.can_dlc = 8;
            frame.data = System.Text.Encoding.Default.GetBytes("12331233");
            obj.frame = frame;

            while (true)
            {
                cANDriver.Transmit(obj, 1);
                Thread.Sleep(100);
            }

            Console.WriteLine(cANDriver.Transmit(obj, 1));
            Console.ReadKey();

            //do
            //{
            //    Console.WriteLine("信号设值，请输入信号ID");
            //    UInt32 messgaeid = Convert.ToUInt32( Console.ReadLine());
            //    Console.WriteLine("信号设值，请输入信号名");
            //    byte[] signalname = System.Text.Encoding.Default.GetBytes(Console.ReadLine());
            //    Console.WriteLine("信号设值，请输入信号值");
            //    Double value = Convert.ToDouble(Console.ReadLine());
            //    intfCANSignal.SetSignalByNameFromApp(messgaeid, signalname, value);      //App层设值
            //    Console.WriteLine("继续信号设值，请输入1，否则输入任何值进行下一步！");
            //} while (Console.ReadLine() == "1");

            //do
            //{
            //    Console.WriteLine("是否开始接收，0：不开启接发线程；1：开启接发线程");      //signal层开启线程接发消息
            //} while (Console.ReadLine() != "1");
            //intfCANSignal.StartThread();

            //Thread thread0 = new Thread(StartGetSignalValue)
            //{
            //    IsBackground = true
            //};                       //开启线程启动获得信号值
            //thread0.Start(intfCANSignal);
            //while (true)
            //{
            //    Thread.Sleep(1000);
            //}
            
        }


    }
}
