using System;

namespace CANSignalLayer
{
    #region 信号层信号结构体

    /// <summary>
    /// value   信号值
    /// strSignalName   信号名
    /// strMessageName  消息名
    /// </summary>
    #endregion
    public struct SignalInfo
    {
        public double value;
        public string strSignalName;
        public UInt32 messageID;
    }

    public interface ICANSignal
    {
        /// <summary>
        /// 向CANSignalLayer层更新来自App层的信号信息，strMessageName: 消息名，strSignalName: 信号名, Value: 信号值,返回值: 标志设置并打包是否成功
        /// </summary>
        /// <param name="strMessageName"></param>
        /// <param name="strSignalName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetSignalByNameFromApp(UInt32 MessageID, byte[] strSignalName, double value);

        #region ...
        /// <summary>
        /// 利用DBC.cs解析本层 更新后 的接收的帧信息，返回为信号信息结构体变量SignalInfo供APP使用
        /// </summary>
        /// <param name="MessageID"></param>
        /// <param name="strSignalName"></param>
        /// <returns></returns>
        #endregion
        double GetSignalByNameToApp(UInt32 MessageID, byte[] strSignalName);

        void StartTimer();

        void StopTimer();

        void Close();


    }
}
