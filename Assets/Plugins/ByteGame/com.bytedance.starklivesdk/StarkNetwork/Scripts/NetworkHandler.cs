// Copyright @ www.bytedance.com
// Time: 2022-08-17
// Author: wuwenbin@bytedance.com
// Description: 基于 MonoBehaviour 的更新函数，驱动整个系统持续运行，控制事件发送接收频次

using System;
using UnityEngine;

namespace StarkNetwork
{
    internal class NetworkHandler: MonoBehaviour
    {
        private static NetworkHandler _instance;

        internal static NetworkHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<NetworkHandler>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("Network Handler");
                        _instance = obj.AddComponent<NetworkHandler>();
                        DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
        }

        private float _sendFrequency;
        private int _sendInterval = 30;
        private float _readFrequency;
        private int _readInterval = 30;

        private int _frameSendCount = Int32.MaxValue;
        private int _frameReadCount = Int32.MaxValue;

        private int _nextSendTime;
        private int _nextReceiveTime;

        private NetworkHandler()
        {
            
        }
        
        public void SetSendFrequency(float frequency)
        {
            _sendFrequency = frequency;
            _sendInterval = Mathf.FloorToInt(1000 / frequency);
        }

        public void SetReadFrequency(float frequency)
        {
            _readFrequency = frequency;
            _readInterval = Mathf.FloorToInt(1000 / frequency);
        }

        public void SetFrameSendCount(int count)
        {
            _frameSendCount = count;
        }

        public void SetFrameReadCount(int count)
        {
            _frameReadCount = count;
        }

        private void LateUpdate()
        {
            var crtTime = (int)(Time.realtimeSinceStartup * 1000);
            if (_nextSendTime < crtTime && NetworkController.HasAnySending)
            {
                NetworkController.SendMessages(_frameSendCount);
                _nextSendTime = crtTime + _sendInterval;
            }

            // if (NetworkController.NetworkState != ClientState.STOP)
            // {
            //     if (_nextReceiveTime < crtTime && NetworkController.NetworkState != ClientState.STOP)
            //     {
            //         NetworkController.ReadMessages(_frameReadCount);
            //         // 在非连接状态会减少消息解析频率
            //         _nextReceiveTime = crtTime + _readInterval;
            //     }
            // }
            NetworkController.ReadMessages(_frameReadCount);
        }

        private void OnApplicationQuit()
        {
            NetworkController.CloseConnection();
            NetworkController.SendAll();
        }
    }
}