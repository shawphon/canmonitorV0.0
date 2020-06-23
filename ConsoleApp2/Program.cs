using System;

using System.Threading;
using System.Runtime.InteropServices;
using CANSignalLayer;
using CANDriverLayer;
using CSVFileOperationPart;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo fileInfo = new FileInfo
            {
                strFilePath = "C:\\Users\\ASUS\\Desktop\\CANprotocol.dbc".PadRight(261, '\0').ToCharArray(),//这部分会出错！！！！
                type = 1
            };

            ICANDriver intfCANDriver = new CANDriver(21, 0, 0);
            intfCANDriver.Open();
            intfCANDriver.Init();
            intfCANDriver.Start();
            VCI_CAN_OBJ vCI_CAN_OBJ = new VCI_CAN_OBJ();
            vCI_CAN_OBJ.ID = 8;
            vCI_CAN_OBJ.RemoteFlag = 0;
            vCI_CAN_OBJ.ExternFlag = 0;
            vCI_CAN_OBJ.Data = System.Text.Encoding.Default.GetBytes("11112222");
            vCI_CAN_OBJ.DataLen = 8;
            vCI_CAN_OBJ.SendType = 0;
            Thread.Sleep(1000);
            UInt32 res = 0;
            VCI_CAN_OBJ[] recFrame = new VCI_CAN_OBJ[50];
            while (true)
            {
                res = intfCANDriver.Receive(ref recFrame);   //先接收到帧     
                if (res!=0)
                {
                    Console.WriteLine(res);

                }
                Thread.Sleep(100);

            }
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


        /// <summary>
        /// 开启获取信号值
        /// </summary>
        /// <param name="intfcanSignal"></param>
        private static void StartGetSignalValue(object intfcanSignal)
        {
            try
            {
                ICANSignal intfCANSignal = intfcanSignal as ICANSignal;
                while (true)
                {
                    Console.WriteLine("请输入想要查询的信号所属消息的ID值：");
                    uint messageId = Convert.ToUInt32(Console.ReadLine());
                    Console.WriteLine("请输入想要查询的信号名：");
                    string signalName = Console.ReadLine();

                    double value = intfCANSignal.GetSignalByNameToApp(messageId, System.Text.Encoding.Default.GetBytes(signalName));
                    Console.WriteLine(Convert.ToString(value));

                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                return;     //添加日志，报错信息
            }

        }

    }
}
