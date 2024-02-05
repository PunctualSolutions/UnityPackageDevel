// Copyright @ www.bytedance.com
// Time: 2022-08-16
// Author: wuwenbin@bytedance.com
// Description: 负责处理消息接收

using System.Collections.Generic;
using StarkMatchmaking;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkNetwork
{
    public partial class NetworkClient
    {

        public void ReadMessages(int count)
        {
            stark_matchmaking.UpdateNetLib();
            var mCount = 0;
            var msg = stark_matchmaking.PopMessage();
            if (msg == null)
            {
                return;
            }
            while (msg != null && mCount < count)
            {
                ParseMessage(msg);
                msg = stark_matchmaking.PopMessage();
                mCount++;
            }
        }

        private void ParseMessage(Message msgInfo)
        {
            UpdateCallbackTargetChanges();
            MessageType eventType = (MessageType)msgInfo.msg_type;
            NetworkDebugger.Log($"<color=#FF8C00>[{eventType}]</color> Received.");
            var ptr = MessageBase.getCPtr(msgInfo.ptr_data).Handle;
            ISerializedMessage msg = null;
            switch (eventType)
            {
                case MessageType.Connected:
                    _state = ClientState.RUNNING;
                    _userId = stark_matchmaking.GetSelfUid();
                    NetworkDebugger.Log($"<color=#FFFFFF>[User Id]</color> {_userId.ToString()}");
                    ResendCache();
                    msg = MessageSerializer.GetSerializedMessage(new ConnectResult(ptr, false));
                    _connectionCallbackTargets.OnConnected((SerializedConnectResult)msg);
                    break;
                case MessageType.ConnectFailed:
                    msg = MessageSerializer.GetSerializedMessage(new ConnectFailedResult(ptr, false));
                    _connectionCallbackTargets.OnConnectFailed((SerializedConnectFailedResult)msg);
                    _state = ClientState.STOP;
                    break;
                case MessageType.Disconnected:
                    _connectionCallbackTargets.OnDisconnected();
                    // 收到 Disconnected 后 Native 会执行重连逻辑
                    _state = ClientState.CONNECTING;
                    break;
                case MessageType.ConnectClosed:
                    msg = MessageSerializer.GetSerializedMessage(new ConnectCloseMsg(ptr, false));
                    _connectionCallbackTargets.OnConnectClosed((SerializedConnectCloseMessage)msg);
                    _state = ClientState.STOP;
                    break;
                case MessageType.PlayerCurrentState:
                    msg = MessageSerializer.GetSerializedMessage(new PlayerCurrentInfoMsg(ptr, false));
                    _connectionCallbackTargets.OnPlayerInfoGot((SerializedPlayerCurrentInfo)msg);
                    //TODO: DO MORE THING IN STARK NETWORK
                    break;
                case MessageType.RoomCreated:
                    msg = MessageSerializer.GetSerializedMessage(new RoomInfoMsg(ptr, false));
                    _matchmakingCallbackTargets.OnCreatedRoom((SerializedRoomInfoMsg)msg);
                    break;
                case MessageType.RoomEntered:
                    msg = MessageSerializer.GetSerializedMessage(new RoomInfoMsg(ptr, false));
                    _matchmakingCallbackTargets.OnJoinedRoom((SerializedRoomInfoMsg)msg);
                    break;
                case MessageType.RoomMatched:
                    msg = MessageSerializer.GetSerializedMessage(new RoomInfoMsg(ptr, false));
                    _matchmakingCallbackTargets.OnMatched((SerializedRoomInfoMsg)msg);
                    break;
                case MessageType.PeopleEnteredRoom:
                    msg = MessageSerializer.GetSerializedMessage(new UserEnterOrLeaveInfo(ptr, false));
                    _inRoomCallbackTargets.OnPlayerEnter((SerializedUserEnterOrLeaveInfo)msg);
                    break;
                case MessageType.PeopleLeavedRoom:
                    msg = MessageSerializer.GetSerializedMessage(new UserEnterOrLeaveInfo(ptr, false));
                    _inRoomCallbackTargets.OnPlayerLeft((SerializedUserEnterOrLeaveInfo)msg);
                    break;
                case MessageType.RoomDateUpdate:
                    msg = MessageSerializer.GetSerializedMessage(new RoomMeteDataUpdateResult(ptr, false));
                    _inRoomCallbackTargets.OnRoomPropertiesUpdate((SerializedRoomMetaDataUpdateResult)msg);
                    break;
                case MessageType.SyncMsgReceived:
                    msg = MessageSerializer.GetSerializedMessage(new SyncMsgInfo(ptr, false));
                    _syncMessageCallbackTargets.OnSyncMessage((SerializedSyncMsgInfo)msg);
                    break;
                case MessageType.RoomMatchFailed:
                    msg = MessageSerializer.GetSerializedMessage(new MessageBase(ptr, false));
                    _matchmakingCallbackTargets.OnMatchFailed(((SerializedMessageBaseInfo)msg).errorInfo);
                    break;
                case MessageType.RoomEnteredFailed:
                    msg = MessageSerializer.GetSerializedMessage(new MessageBase(ptr, false));
                    _matchmakingCallbackTargets.OnJoinRoomFailed(((SerializedMessageBaseInfo)msg).errorInfo);
                    break;
                case MessageType.RoomLeaved:
                    msg = MessageSerializer.GetSerializedMessage(new RoomLeaveResult(ptr, false));
                    _matchmakingCallbackTargets.OnLeftRoom((SerializedRoomLeaveResult)msg);
                    break;
                case MessageType.RoomOwnerUpdate:
                    msg = MessageSerializer.GetSerializedMessage(new RoomOwnerUpdateInfo(ptr, false));
                    _inRoomCallbackTargets.OnMasterClientSwitched((SerializedRoomOwnerUpdateInfo)msg);
                    break;
                case MessageType.RoomKickMember:
                    msg = MessageSerializer.GetSerializedMessage(new RoomKickoutUserRst(ptr, false));
                    break;
                case MessageType.SwitchRoomResult:
                    msg = MessageSerializer.GetSerializedMessage(new SwitchRoomRst(ptr, false));
                    _inRoomCallbackTargets.OnSwitchRoomToken((SerializedSwitchRoomRst)msg);
                    break;
                case MessageType.OnError:
                    msg = MessageSerializer.GetSerializedMessage(new MessageBase(ptr, false));
                    _errorInfoCallbackTargets.OnErrorInfo(((SerializedMessageBaseInfo)msg).errorInfo);
                    break;
                case MessageType.RpcServerCall:
                    msg = MessageSerializer.GetSerializedMessage(new RpcServerCallInfo(ptr, false));
                    Rpc.OnServerRpcCall((SerializedRpcCallInfo)msg);
                    break;
                default:
                    NetworkDebugger.Warning("Unhandled event type: " + eventType);
                    break;

            }
            UpdateCallbackTargetChanges();
            if (msg != null && eventType != MessageType.SyncMsgReceived) NetworkDebugger.Log($"<color=#FFFFFF>[{eventType}]</color> ==> <color=#FF8C00>{JsonUtility.ToJson(msg)}</color>");
        }

        private struct CallbackTargetChange
        {
            public bool isAdd;
            public object target;
        }

        private Queue<CallbackTargetChange> _callbackTargetChanges = new Queue<CallbackTargetChange>();

        // 防止重复注册回调
        private readonly HashSet<object> _callbackTargets = new HashSet<object>();

        private readonly NetworkRoomManager _networkRoomManager = new NetworkRoomManager();
        
        private readonly ConnectionCallbacksContainer _connectionCallbackTargets = new ConnectionCallbacksContainer();
        private readonly MatchmakingCallbacksContainer _matchmakingCallbackTargets = new MatchmakingCallbacksContainer();
        private readonly InRoomCallbacksContainer _inRoomCallbackTargets = new InRoomCallbacksContainer();
        private readonly SyncMessageCallbackContainer _syncMessageCallbackTargets = new SyncMessageCallbackContainer();
        private readonly ErrorInfoCallbackContainer _errorInfoCallbackTargets = new ErrorInfoCallbackContainer();
        private readonly OperationCallbackContainer _operationCallbackTargets = new OperationCallbackContainer();

        public void AddCallbackTarget(object target)
        {
            _callbackTargetChanges.Enqueue(new CallbackTargetChange
            {
                isAdd = true,
                target = target
            });
        }

        public void RemoveCallbackTarget(object target)
        {
            _callbackTargetChanges.Enqueue(new CallbackTargetChange
            {
                isAdd = false,
                target = target
            });
        }

        /// <summary>
        /// 同一更新注册和注销回调对象，避免在回调列表执行过程中增减回调引起得报错
        /// </summary>
        private void UpdateCallbackTargetChanges()
        {
            while (_callbackTargetChanges.Count > 0)
            {
                var change = _callbackTargetChanges.Dequeue();
                if (change.isAdd)
                {
                    if (_callbackTargets.Contains(change.target))
                    {
                        NetworkDebugger.Warning("Callback target has already been registered, add callback target operation will skip this one");
                        continue;
                    }

                    _callbackTargets.Add(change.target);
                }
                else
                {
                    if (!_callbackTargets.Contains(change.target))
                    {
                        NetworkDebugger.Warning("Callback target is not in the pool, remove callback target operation will skip this one");
                        continue;
                    }

                    _callbackTargets.Remove(change.target);
                }
                
                if (change.target is INetworkRoom networkRoom)
                {
                    UpdateNetworkRoomManager(networkRoom, change.isAdd);
                }
                else
                {
                    UpdateCallbackTargetContainer<IConnectionCallbacks>(change.target, _connectionCallbackTargets, change.isAdd);
                    UpdateCallbackTargetContainer<IMatchmakingCallbacks>(change.target, _matchmakingCallbackTargets, change.isAdd);
                    UpdateCallbackTargetContainer<IInRoomCallbacks>(change.target, _inRoomCallbackTargets, change.isAdd);
                    UpdateCallbackTargetContainer<ISyncMessageCallbacks>(change.target, _syncMessageCallbackTargets, change.isAdd);
                    UpdateCallbackTargetContainer<IErrorInfoCallback>(change.target, _errorInfoCallbackTargets, change.isAdd);
                    UpdateCallbackTargetContainer<IOperationCallback>(change.target, _operationCallbackTargets, change.isAdd);
                }
            }
        }

        private void UpdateCallbackTargetContainer<T>(object target, List<T> container, bool isAdd) where T : class
        {
            if (target is T callbackTarget)
            {
                if (isAdd)
                {
                    container.Add(callbackTarget);
                }
                else
                {
                    container.Remove(callbackTarget);
                }
            }
        }

        private void UpdateNetworkRoomManager(INetworkRoom networkRoom, bool isAdd)
        {
            if (isAdd)
            {
                _networkRoomManager.Add(networkRoom);
            }
            else
            {
                _networkRoomManager.Remove(networkRoom);
            }
        }
    }
}