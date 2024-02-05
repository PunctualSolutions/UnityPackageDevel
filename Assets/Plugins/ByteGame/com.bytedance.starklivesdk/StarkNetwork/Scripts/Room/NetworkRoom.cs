// Copyright @ www.bytedance.com
// Time: 2022-08-22
// Author: wuwenbin@bytedance.com
// Description: 网络房间逻辑的基类

using System;
using System.Collections.Generic;

namespace StarkNetwork
{
    public class NetworkRoom: INetworkRoom
    {
        private readonly ulong _requestId;
        public ulong RequestId => _requestId;
        
        private ulong _roomId;
        public ulong RoomId => _roomId;

        private RoomState _state = RoomState.OUTER;
        public RoomState State => _state;

        private SerializedRoomInfoMsg _roomInfo;
        public SerializedRoomInfoMsg RoomInfo => _roomInfo;
        public HashSet<ulong> Users => _roomInfo.users;
        public int MaxUserCount => _roomInfo.maxPlayerNum;
        public ulong OwnerId => _roomInfo.ownerId;
        public bool IsOwner => _roomInfo.ownerId == NetworkController.UserId;
        
        public Dictionary<string, string> MetaData =>  _roomInfo.metaData;

        private readonly List<SerializedSyncMsgInfo> _msgs = new List<SerializedSyncMsgInfo>();
        public List<SerializedSyncMsgInfo> HistoryRoomMessages => _msgs;
        

        /// <summary>
        /// 房间创建匹配和进入事件
        /// </summary>
        public event Action<RoomMatchEvent> OnMatchEvent;

        /// <summary>
        /// 房间退出事件
        /// </summary>
        public event Action<RoomLeaveReason> OnLeft;
        
        /// <summary>
        /// 进出房间时的状态改变事件
        /// </summary>
        public event Action<RoomState> OnStateChanged;
        
        /// <summary>
        /// 房间内玩家数量的改变
        /// </summary>
        public event Action<RoomUserChangeEvent, ulong> OnUserChanged;
        
        /// <summary>
        /// 在房间内获取到再次匹配的token
        /// </summary>
        public event Action<SerializedSwitchRoomRst> OnSwitchRoomInfo;
        
        /// <summary>
        /// 房间状态以外的信息改变
        /// </summary>
        public event Action OnInfoChanged;
        
        /// <summary>
        /// 收到房间内信息
        /// </summary>
        public event Action<SerializedSyncMsgInfo> OnRoomMessage;

        /// <summary>
        /// 收到房间内报错
        /// </summary>
        public event Action<SerializedErrorInfo> OnError;

        /// <summary>
        /// 通过房间 id 或是请求 id 标记一个网络房间
        /// </summary>
        /// <param name="roomIdOrRequestId"></param>
        /// <param name="isRequest"></param>
        public NetworkRoom(ulong roomIdOrRequestId, bool isRequest = false)
        {
            if (isRequest)
            {
                _requestId = roomIdOrRequestId;
            }
            else
            {
                _roomId = roomIdOrRequestId;
            }
            NetworkController.AddCallbackTarget(this);
        }

        public void SetRoomId(ulong roomId)
        {
            _roomId = roomId;
        }

        public void SetInfo(SerializedRoomInfoMsg info)
        {
            if (info.roomId == _roomId)
            {
                _roomInfo = info;
                OnInfoChanged?.Invoke();
            }
        }
        
        public void SetInfo(SerializedRoomInfo info)
        {
            if (info.roomId == _roomId)
            {
                _roomInfo.ownerId = info.ownerId;
                _roomInfo.maxPlayerNum = info.maxPlayerNum;
                _roomInfo.users = info.users;
                _roomInfo.metaData = info.metaData;
                OnInfoChanged?.Invoke();
            }
        }

        public void SetState(RoomState state)
        {
            _state = state;
            OnStateChanged?.Invoke(_state);
        }

        public void JoinRoom()
        {
            if (_state == RoomState.OUTER)
            {
                // 断线后会退出房间注销监听，调用此接口重连时需要重新注册监听
                NetworkController.AddCallbackTarget(this);
                NetworkController.JoinRoom(_roomId);
                _state = RoomState.JOINING;
                OnStateChanged?.Invoke(_state);
            }
        }

        public void LeaveRoom()
        {
            if (_state == RoomState.INNER)
            {
                NetworkController.LeaveRoom(_roomId);
                _state = RoomState.QUITING;
                OnStateChanged?.Invoke(_state);
            }
        }

        public void UpdateMetaData(Dictionary<string, string> metaData)
        {
            NetworkController.UpdateRoomMetaData(_roomId, metaData);
        }

        public void KickOutUser(ulong userId)
        {
            NetworkController.KickOutUser(_roomId, userId);
        }
        
        public void HandOverOwner(ulong userId)
        {
            NetworkController.HandOverOwner(_roomId, userId);
        }

        public void SwitchRoom()
        {
            NetworkController.SwitchRoom(_roomId);
        }
        

        public void SyncMessage(string msg, ulong toUserId = 0)
        {
            NetworkController.SyncMessage(msg, Convert.ToUInt64(RoomId), toUserId);
        }

        public virtual void OnMatched(SerializedRoomInfoMsg msg)
        {
            _roomInfo = msg;
            _state = RoomState.OUTER;
            OnInfoChanged?.Invoke();
            OnMatchEvent?.Invoke(RoomMatchEvent.Matched);
        }

        public virtual void OnMatchFailed(SerializedErrorInfo msg)
        {
            
        }

        public virtual void OnCreatedRoom(SerializedRoomInfoMsg msg)
        {
            _roomInfo = msg;
            _state = RoomState.OUTER;
            OnInfoChanged?.Invoke();
            OnMatchEvent?.Invoke(RoomMatchEvent.Created);
        }

        public virtual void OnCreateRoomFailed(SerializedErrorInfo msg)
        {
            
        }
        
        public virtual void OnJoinedRoom(SerializedRoomInfoMsg msg)
        {
            _roomInfo = msg;
            _state = RoomState.INNER;
            OnInfoChanged?.Invoke();
            OnStateChanged?.Invoke(_state);
            OnMatchEvent?.Invoke(RoomMatchEvent.Entered);
        }

        public virtual void OnJoinRoomFailed(SerializedErrorInfo msg)
        {
            // 避免重复加入的 Failed 导致房间状态错误
            if (_state == RoomState.JOINING)
            {
                _state = RoomState.OUTER;
                OnStateChanged?.Invoke(_state);
            }
            OnError?.Invoke(msg);
        }

        public virtual void OnLeftRoom(SerializedRoomLeaveResult msg)
        {
            _state = RoomState.OUTER;
            _roomInfo.users.Remove(NetworkController.UserId);
            OnStateChanged?.Invoke(_state);
            OnLeft?.Invoke((RoomLeaveReason)msg.reason);
            NetworkController.RemoveCallbackTarget(this);
        }

        public virtual void OnPlayerEnter(SerializedUserEnterOrLeaveInfo newPlayer)
        {
            _roomInfo.users.Add(newPlayer.userId);
            OnUserChanged?.Invoke(RoomUserChangeEvent.Add, newPlayer.userId);
        }

        public virtual void OnPlayerLeft(SerializedUserEnterOrLeaveInfo leavePlayer)
        {
            _roomInfo.users.Remove(leavePlayer.userId);
            OnUserChanged?.Invoke(RoomUserChangeEvent.Remove, leavePlayer.userId);
        }

        public virtual void OnPlayerPropertiesUpdate()
        {
            // todo
        }

        public virtual void OnMasterClientSwitched(SerializedRoomOwnerUpdateInfo info)
        {
            // 在改变房主之前先通知新房主 id，这样还能得到旧的房主 id
            OnUserChanged?.Invoke(RoomUserChangeEvent.OwnerChanged, info.newOwnerId);
            _roomInfo.ownerId = info.newOwnerId;
            OnInfoChanged?.Invoke();
        }

        public virtual void OnSwitchRoomToken(SerializedSwitchRoomRst switchInfo)
        {
            OnSwitchRoomInfo?.Invoke(switchInfo);
        }

        public virtual void OnRoomPropertiesUpdate(SerializedRoomMetaDataUpdateResult propertiesThatChanged)
        {
            foreach (var kvp in propertiesThatChanged.metaData)
            {
                _roomInfo.metaData[kvp.Key] = kvp.Value;
            }
            OnInfoChanged?.Invoke();
        }
        
        public virtual void OnSyncMessage(SerializedSyncMsgInfo msg)
        {
            _msgs.Add(msg);
            OnRoomMessage?.Invoke(msg);
        }
    }
}