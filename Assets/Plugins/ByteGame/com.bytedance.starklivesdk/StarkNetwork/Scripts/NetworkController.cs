// Copyright @ www.bytedance.com
// Time: 2022-08-19
// Author: wuwenbin@bytedance.com
// Description: StarkNetwork 的总控制台，用于统一管理整个模块

using System;
using StarkMatchmaking;

namespace StarkNetwork
{
    public static partial class NetworkController
    {
        /// <summary>
        /// 基于 Unity 生命周期函数驱动驱动
        /// </summary>
        private static NetworkHandler Handler = NetworkHandler.Instance;

        /// <summary>
        /// 负责和平台无关的消息发送、接收、处理逻辑
        /// </summary>
        private static NetworkClient Client = new NetworkClient();

        #region CLIENT

        public static ulong UserId => Client.UserId;
        public static bool HasAnySending => Client.HasAnySending;
        public static ClientState NetworkState => Client.State;

        /// <summary>
        /// 将实现回调的目标实例注册到服务中，接收回调
        /// </summary>
        /// <param name="target"></param>
        public static void AddCallbackTarget(object target)
        {
            Client.AddCallbackTarget(target);
        }

        /// <summary>
        /// 将实现回调的目标实例从服务中移除，不在接收回调
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveCallbackTarget(object target)
        {
            Client.RemoveCallbackTarget(target);
        }
        
        /// <summary>
        /// 一次将所有消息全部发送出去，不受频次和数量影响
        /// </summary>
        public static void SendAll()
        {
            Client.SendMessages(Int32.MaxValue);
        }

        
        /// <summary>
        /// 一次获取全部消息，不受频次和数量影响
        /// </summary>
        public static void ReadAll()
        {
            Client.ReadMessages(Int32.MaxValue);
        }
        
        internal static void SendMessages(int count)
        {
            Client.SendMessages(count);
        }

        internal static void ReadMessages(int count)
        {
            Client.ReadMessages(count);
        }

        #endregion

        #region HANDLER


        /// <summary>
        /// 设置消息发送频次
        /// </summary>
        /// <param name="frequency">n 次/秒</param>
        public static void SetSendFrequency(float frequency)
        {
            Handler.SetSendFrequency(frequency);
        }
        
        /// <summary>
        /// 设置消息接收频次
        /// </summary>
        /// <param name="frequency">n 次/秒</param>
        public static void SetReadFrequency(float frequency)
        {
            Handler.SetReadFrequency(frequency);
        }
        
        /// <summary>
        /// 设置单次最大发送消息数量
        /// </summary>
        /// <param name="count"></param>
        public static void SetFrameSendCount(int count)
        {
            Handler.SetFrameSendCount(count);
        }
        
        /// <summary>
        /// 设置单次最大接收消息数量
        /// </summary>
        /// <param name="count"></param>
        public static void SetFrameReadCount(int count)
        {
            Handler.SetFrameReadCount(count);
        }

        #endregion
    }
}