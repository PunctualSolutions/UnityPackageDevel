// Copyright @ www.bytedance.com
// Time: 2022-08-19
// Author: wuwenbin@bytedance.com
// Description: 负责和 Native 通信，进行各种网络操作的独立模块

using System;
using System.Runtime.InteropServices;
using AOT;
using StarkMatchmaking;

namespace StarkNetwork
{
    public partial class NetworkClient
    {
        internal NetworkClient()
        {
            AddCallbackTarget(_networkRoomManager);
        }

        private ClientState _state;
        /// <summary>
        /// 当前网络服务的连接状态
        /// </summary>
        public ClientState State => _state;

        private ulong _userId;
        /// <summary>
        /// 本次连接的用户 id
        /// </summary>
        public ulong UserId => _userId;
        
        /// <summary>
        /// 是否开启严格模式，开启后将屏蔽无效操作，对可能的操作进行重发
        /// </summary>
        private bool _strictMode = true;

        public long Rtt => _state == ClientState.RUNNING ? stark_matchmaking.GetRTT() : 0;
    }
}