// Copyright @ www.bytedance.com
// Time: 2022-08-16
// Author: wuwenbin@bytedance.com
// Description: 负责处理消息发送

using System;
using System.Collections.Generic;
using StarkMatchmaking;
using UnityEngine;

namespace StarkNetwork
{
    public partial class NetworkClient
    {
        
        private readonly Queue<NetworkOperation> _sendingQueue = new Queue<NetworkOperation>();
        /// <summary>
        /// 用于登陆成功前的消息
        /// </summary>
        private readonly Queue<NetworkOperation> _cacheQueue = new Queue<NetworkOperation>();

        private readonly Queue<NetworkOperation> _tempQueue = new Queue<NetworkOperation>();
        public bool HasAnySending => _sendingQueue.Count > 0;

        internal bool EnqueueSending(NetworkOperation operation)
        {
            BeforeOperation(operation);
            _sendingQueue.Enqueue(operation);
            return true;
        }

        /// <summary>
        /// 将 cacheQueue 顺序插入到 sendingQueue 前
        /// </summary>
        private void ResendCache()
        {
            if (_cacheQueue.Count == 0)
            {
                return;
            }
            while (_sendingQueue.Count > 0)
            {
                var temp = _sendingQueue.Dequeue();
                _tempQueue.Enqueue(temp);
            }
            while (_cacheQueue.Count > 0)
            {
                var cache = _cacheQueue.Dequeue();
                _sendingQueue.Enqueue(cache);
            }
            while (_tempQueue.Count > 0)
            {
                var temp = _tempQueue.Dequeue();
                _sendingQueue.Enqueue(temp);
            }
        }

        private void BeforeOperation(NetworkOperation operation)
        {
            // tbd
        }

        private void AfterOperation(NetworkOperation operation)
        {
            // tbd
        }

        internal void SendMessages(int count)
        {
            var mCount = 0;
            while (_sendingQueue.Count > 0 && mCount < count)
            {
                var operation = _sendingQueue.Dequeue();
                if (_strictMode && !CheckOperationValid(operation))
                {
                    continue;
                }
                SendOperation(operation);
                AfterOperation(operation);
                mCount++;
            }
        }

        private void SendOperation(NetworkOperation operation)
        {
            if (operation.type != SendingOperationType.MESSAGE)
            {
                NetworkDebugger.Log($"<color=#40E0D0>[{operation.type}]</color> Send.");
                NetworkDebugger.Log($"<color=#FFFFFF>[{operation.type}]</color> ==> <color=#40E0D0>{JsonUtility.ToJson(operation.data)}</color>");
            }
            ulong requestId = 0;
            switch (operation.type)
            {
                case SendingOperationType.CONNECT:
                    var connectOperationData = (ConnectOperationData)operation.data;
                    if (connectOperationData.isFake)
                    {
                        stark_matchmaking.TestConnect(connectOperationData.host, connectOperationData.port, connectOperationData.token, connectOperationData.appId);
                    }
                    else
                    {
                        stark_matchmaking.Connect(connectOperationData.host, connectOperationData.port, connectOperationData.token, connectOperationData.appId);
                    }
                    _state = ClientState.CONNECTING;
                    break;
                case SendingOperationType.CLOSE:
                    stark_matchmaking.Disconnect();
                    _state = ClientState.STOP;
                    _connectionCallbackTargets.OnConnectClosed(new SerializedConnectCloseMessage
                    {
                        msgType = (int)MessageType.ConnectClosed,
                        requestId = 0,
                        errorInfo = null,
                        closeReason = ConnectCloseReason.Initiative
                    });
                    break;
                case SendingOperationType.GET_CURRENT_STATE:
                    stark_matchmaking.GetCurrentState();
                    break;
                case SendingOperationType.MATCH:
                    var matchOperationData = (MatchOperationData)operation.data;
                    var matchmakingOptions = TransformToMatchMakingOption(matchOperationData);
                    requestId = stark_matchmaking.MatchmakingEnqueue(matchmakingOptions);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.MATCH_CANCEL:
                    stark_matchmaking.MatchmakingCancel();
                    break;
                case SendingOperationType.CREATE:
                    var createRoomOperationData = (CreateRoomOperationData)operation.data;
                    var createRoomOption= TransformToCreateRoomOption(createRoomOperationData);
                    requestId = stark_matchmaking.CreateRoom(createRoomOption);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.JOIN:
                    var joinRoomOperationData = (JoinRoomOperationData)operation.data;
                    requestId = stark_matchmaking.JoinRoom(joinRoomOperationData.roomId);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.LEAVE:
                    var leaveRoomOperationData = (LeaveRoomOperationData)operation.data;
                    requestId = stark_matchmaking.LeaveRoom(leaveRoomOperationData.roomId);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.UPDATE_ROOM_META_DATA:
                    var updateRoomMetaDataOperationData = (UpdateRoomMetaDataOperationData)operation.data;
                    var metaData = new MetaData();
                    foreach (var kvp in updateRoomMetaDataOperationData.metaData)
                    {
                        metaData.meta_data.Add(kvp.Key, kvp.Value);
                    }
                    requestId = stark_matchmaking.AddOrUpdateRoomMetaData(updateRoomMetaDataOperationData.roomId, metaData);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.KICK:
                    var kickOutUserOperationData = (KickOutUserOperationData)operation.data;
                    requestId = stark_matchmaking.KickoutUser(kickOutUserOperationData.roomId, kickOutUserOperationData.userId);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.HANDOVER:
                    var handoverOperationData = (HandOverOperationData)operation.data;
                    requestId = stark_matchmaking.HandOverOwner(handoverOperationData.roomId, handoverOperationData.userId);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.SWITCH_ROOM:
                    var switchroom = (SwitchRoomOperationData)operation.data;
                    requestId = stark_matchmaking.SwitchRoom(switchroom.roomId);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.JOIN_ROOM_WITH_TOKEN:
                    var joinRoomWithToken = (JoinRoomWithTokenOperationData)operation.data;
                    requestId = stark_matchmaking.JoinRoomWithToken(joinRoomWithToken.token);
                    operation.requestIdReceiver?.Invoke(requestId);
                    break;
                case SendingOperationType.MESSAGE:
                    var syncMessageOperationData = (SyncMessageOperationData)operation.data;
                    stark_matchmaking.SendSyncMsgInRoom(syncMessageOperationData.msg, syncMessageOperationData.userId, syncMessageOperationData.roomId);
                    break;
                default:
                    break;
            }

            // 避免接收 requestId 函数引用其他变量导致的内存无法释放
            operation.requestIdReceiver = null;

            _operationCallbackTargets.OnOperation(operation, requestId);

            if (requestId != 0)
            {
                RecordOperationRequest(requestId, operation);
                NetworkDebugger.Log($"<color=#FFFFFF>[Request Id]</color> {requestId}");
            }
        }

        private MatchmakingOptions TransformToMatchMakingOption(MatchOperationData data)
        {
            var option = new MatchmakingOptions();
            option.custom_matchmaking_policy_name = data.policyName;
            option.createNewOne = data.createNew;
            option.matchAndjoinImmediately = data.joinImmediately;
            option.need_ds = data.needDs;
            if (data.filters != null)
            {
                foreach (var kvp in data.filters)
                {
                    option.filters.Add(kvp.Key, kvp.Value);
                }
            }
            if (data.createNew)
            {
                var createRoomOption = TransformToCreateRoomOption(data.createRoomOption);
                option.createRoomOption = createRoomOption;
            }

            return option;
        }

        private CreateRoomOption TransformToCreateRoomOption(CreateRoomOperationData data)
        {
            var option = new CreateRoomOption();
            option.max_player_num = data.maxPlayerCount;
            if (data.metaData != null)
            {
                foreach (var kvp in data.metaData)
                {
                    option.meta_data.Add(kvp.Key, kvp.Value);
                }
            }
            return option;
        }

        private bool CheckOperationValid(NetworkOperation operation)
        {
            switch (operation.type)
            {
                case SendingOperationType.CONNECT:
                    return _state == ClientState.STOP;   // 未连接直接连接，已连接重连，连接过程中不宜重复连接
                case SendingOperationType.CLOSE:
                    return _state != ClientState.STOP;
                case SendingOperationType.GET_CURRENT_STATE:
                case SendingOperationType.MATCH:
                case SendingOperationType.CREATE:
                case SendingOperationType.JOIN:
                case SendingOperationType.SWITCH_ROOM:
                case SendingOperationType.JOIN_ROOM_WITH_TOKEN:
                case SendingOperationType.LEAVE:
                case SendingOperationType.UPDATE_ROOM_META_DATA:
                case SendingOperationType.KICK:
                case SendingOperationType.HANDOVER:
                case SendingOperationType.MESSAGE:
                    if (_state == ClientState.CONNECTING)
                    {
                        _cacheQueue.Enqueue(operation);
                    }
                    return _state == ClientState.RUNNING;
                case SendingOperationType.MATCH_CANCEL:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}