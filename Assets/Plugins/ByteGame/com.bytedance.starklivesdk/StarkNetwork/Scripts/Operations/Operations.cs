// Copyright @ www.bytedance.com
// Time: 2022-08-25
// Author: wuwenbin@bytedance.com
// Description: 将各种网络操作封装成对应的数据结构体

using System;
using System.Collections.Generic;

namespace StarkNetwork
{
    public enum SendingOperationType
    {
        CONNECT,    // 登录，连接
        GET_CURRENT_STATE, // 获取自身信息（房间列表）
        CLOSE,      // 下线，断开连接
        MATCH,      // 匹配
        MATCH_CANCEL,   // 取消匹配
        CREATE,     // 创建房间
        JOIN,       // 加入指定房间
        LEAVE,      // 离开房间
        UPDATE_ROOM_META_DATA,  // 更新房间信息
        KICK,       // 将玩家踢出房间
        HANDOVER,   //转移房主
        SWITCH_ROOM, // 获取下一次匹配的token
        JOIN_ROOM_WITH_TOKEN,
        MESSAGE,    // 发送消息
    }

    public class NetworkOperation
    {
        public SendingOperationType type;
        public IOperationData data;
        public Action<ulong> requestIdReceiver;
    }

    public interface IOperationData
    {
        
    }
    
    public struct ConnectOperationData: IOperationData
    {
        public string host;
        public int port;
        public string token;
        public string appId;
        public bool isFake;
    }

    public struct MatchOperationData: IOperationData
    {
        public string policyName;
        public bool createNew;
        public bool joinImmediately;
        public Dictionary<string, string> filters;
        public bool needDs;
        public CreateRoomOperationData createRoomOption;
    }

    public struct CreateRoomOperationData: IOperationData
    {
        public uint maxPlayerCount;
        public bool needDs;
        public Dictionary<string, string> metaData;
    }

    public struct JoinRoomOperationData : IOperationData
    {
        public ulong roomId;
    }
    
    public struct LeaveRoomOperationData : IOperationData
    {
        public ulong roomId;
    }

    public struct UpdateRoomMetaDataOperationData : IOperationData
    {
        public ulong roomId;
        public Dictionary<string, string> metaData;
    }

    public struct KickOutUserOperationData : IOperationData
    {
        public ulong roomId;
        public ulong userId;
    }
    
    public struct HandOverOperationData : IOperationData
    {
        public ulong roomId;
        public ulong userId;
    }
    
    public struct SwitchRoomOperationData : IOperationData
    {
        public ulong roomId;
    }
    public struct JoinRoomWithTokenOperationData : IOperationData
    {
        public string token;
    }

    public struct SyncMessageOperationData : IOperationData
    {
        public ulong roomId;
        public ulong userId;
        public string msg;
    }
}