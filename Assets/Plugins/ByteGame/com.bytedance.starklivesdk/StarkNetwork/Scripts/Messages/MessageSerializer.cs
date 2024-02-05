// Copyright @ www.bytedance.com
// Time: 2022-08-23
// Author: wuwenbin@bytedance.com
// Description: 用于将 Native 返回的各种数据转化为 C# 基础数据

using System;
using System.Collections.Generic;
using StarkMatchmaking;
using UnityEngine;

namespace StarkNetwork
{
    public static class MessageSerializer
    {
        private static readonly Dictionary<Type, Func<ISerializedMessage>> _typeParserDic =
            new Dictionary<Type, Func<ISerializedMessage>>()
            {
                { typeof(MessageBase), () => new SerializedMessageBaseInfo() },
                { typeof(ConnectResult), () => new SerializedConnectResult() },
                { typeof(ConnectFailedResult), () => new SerializedConnectFailedResult() },
                { typeof(ConnectCloseMsg), () => new SerializedConnectCloseMessage() },
                { typeof(PlayerCurrentInfoMsg), () => new SerializedPlayerCurrentInfo() },
                { typeof(RoomInfoMsg), () => new SerializedRoomInfoMsg() },
                { typeof(UserEnterOrLeaveInfo), () => new SerializedUserEnterOrLeaveInfo() },
                { typeof(RoomOwnerUpdateInfo), () => new SerializedRoomOwnerUpdateInfo() },
                { typeof(RoomMeteDataUpdateResult), () => new SerializedRoomMetaDataUpdateResult() },
                { typeof(RoomLeaveResult), () => new SerializedRoomLeaveResult() },
                { typeof(RoomKickoutUserRst), () => new SerializedRoomKickOutUserResult() },
                { typeof(SwitchRoomRst), () => new SerializedSwitchRoomRst() },
                { typeof(SyncMsgInfo), () => new SerializedSyncMsgInfo() },
                { typeof(RpcServerCallInfo), () => new SerializedRpcCallInfo() },
            };

        public static ISerializedMessage GetSerializedMessage<T>(T data) where T: MessageBase
        {
            if (_typeParserDic.TryGetValue(typeof(T), out var parser))
            {
                var d = parser();
                d.Parse(data);
                return d;
            }
            NetworkDebugger.Warning($"Type [{typeof(T)}] not been registered.");
            return null;
        }
    }
}