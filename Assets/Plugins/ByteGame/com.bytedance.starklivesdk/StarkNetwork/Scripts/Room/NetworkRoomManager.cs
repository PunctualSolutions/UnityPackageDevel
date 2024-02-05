// Copyright @ www.bytedance.com
// Time: 2022-08-22
// Author: wuwenbin@bytedance.com
// Description: 用于管理网络房间的回调和状态

using System.Collections.Generic;

namespace StarkNetwork
{
    public class NetworkRoomManager: IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ISyncMessageCallbacks
    {
        internal NetworkRoomManager()
        {
            
        }

        private readonly Dictionary<ulong, List<INetworkRoom>> _roomDic = new Dictionary<ulong, List<INetworkRoom>>();

        private readonly Dictionary<ulong, List<INetworkRoom>>
            _requestDic = new Dictionary<ulong, List<INetworkRoom>>();

        public List<INetworkRoom> GetRoomsById(ulong roomId) => _roomDic[roomId];

        /// <summary>
        /// 注册 NetworkRoom 加入到字典中，若房间仅有 RequestId，则表示还在匹配中，先存入 Request 字典，待获取到具体的房间信息后再存入字典
        /// </summary>
        /// <param name="room"></param>
        public void Add(INetworkRoom room)
        {
            var dic = (room.RoomId != 0)?_roomDic: _requestDic;
            var id = (room.RoomId != 0) ? room.RoomId : room.RequestId;
            if (dic.TryGetValue(id, out var list))
            {
                list.Add(room);
            }
            else
            {
                dic.Add(id, new List<INetworkRoom>(){room});
            }
        }

        /// <summary>
        /// 从字典中注销 NetworkRoom
        /// </summary>
        /// <param name="room"></param>
        public void Remove(INetworkRoom room)
        {
            var dic = (room.RoomId != 0)?_roomDic: _requestDic;
            var id = (room.RoomId != 0) ? room.RoomId : room.RequestId;
            if (dic.TryGetValue(id, out var list))
            {
                list.Remove(room);
                if (list.Count == 0)
                {
                    dic.Remove(id);
                }
            }
        }

        public void OnConnected(SerializedConnectResult result)
        {
            
        }

        public void OnConnectFailed(SerializedConnectFailedResult result)
        {
            foreach (var kvp in _roomDic)
            {
                foreach (var room in kvp.Value)
                {
                    SafeCaller.Call(() => room.OnLeftRoom(new SerializedRoomLeaveResult() { roomId = room.RoomId }));
                }
            }

            _roomDic.Clear();
        }

        public void OnMatched(SerializedRoomInfoMsg msg)
        {
            RequestTransform(msg.requestId, msg.roomId);
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnMatched(msg));
                }
            }
        }

        public void OnMatchFailed(SerializedErrorInfo msg)
        {
            
        }

        public void OnCreatedRoom(SerializedRoomInfoMsg msg)
        {
            RequestTransform(msg.requestId, msg.roomId);
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnCreatedRoom(msg));
                }
            }
        }

        public void OnCreateRoomFailed(SerializedErrorInfo msg)
        {
            
        }

        public void OnJoinedRoom(SerializedRoomInfoMsg msg)
        {
            RequestTransform(msg.requestId, msg.roomId);
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnJoinedRoom(msg));
                }
            }
        }

        public void OnJoinRoomFailed(SerializedErrorInfo msg)
        {
            var joinRoomOp = NetworkController.FindOperationRequest(msg.requestId);
            if (joinRoomOp == null || joinRoomOp.type != SendingOperationType.JOIN)
            {
                return;
            }

            var opData = (JoinRoomOperationData)joinRoomOp.data;
            if (_roomDic.TryGetValue(opData.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnJoinRoomFailed(msg));
                }
            }
        }

        public void OnLeftRoom(SerializedRoomLeaveResult msg)
        {
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnLeftRoom(msg));
                }
            }
        }

        public void OnRoomPropertiesUpdate(SerializedRoomMetaDataUpdateResult msg)
        {
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnRoomPropertiesUpdate(msg));
                }
            }
        }

        public void OnPlayerEnter(SerializedUserEnterOrLeaveInfo newPlayer)
        {
            if (_roomDic.TryGetValue(newPlayer.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnPlayerEnter(newPlayer));
                }
            }
        }

        public void OnPlayerLeft(SerializedUserEnterOrLeaveInfo leavePlayer)
        {
            if (_roomDic.TryGetValue(leavePlayer.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnPlayerLeft(leavePlayer));
                }
            }
        }

        public void OnPlayerPropertiesUpdate()
        {
            // todo
        }

        public void OnMasterClientSwitched(SerializedRoomOwnerUpdateInfo info)
        {
            if (_roomDic.TryGetValue(info.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnMasterClientSwitched(info));
                }
            }
        }

        public void OnSwitchRoomToken(SerializedSwitchRoomRst switchInfo)
        {
            if (_roomDic.TryGetValue(switchInfo.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnSwitchRoomToken(switchInfo));
                }
            }
        }

        public void OnSyncMessage(SerializedSyncMsgInfo msg)
        {
            if (_roomDic.TryGetValue(msg.roomId, out var list))
            {
                foreach (var room in list)
                {
                    SafeCaller.Call(()=>room.OnSyncMessage(msg));
                }
            }
        }

        public void OnDisconnected()
        {
            
        }

        public void OnConnectClosed(SerializedConnectCloseMessage msg)
        {
            foreach (var kvp in _roomDic)
            {
                foreach (var room in kvp.Value)
                {
                    SafeCaller.Call(()=>room.OnLeftRoom(new SerializedRoomLeaveResult(){roomId = room.RoomId}));
                }
            }
            _roomDic.Clear();
        }

        public void OnPlayerInfoGot(SerializedPlayerCurrentInfo info)
        {
            foreach (var crtRoom in info.roomList)
            {
                if (_roomDic.TryGetValue(crtRoom.roomId, out var list))
                {
                    foreach (var room in list)
                    {
                        SafeCaller.Call(()=>room.SetInfo(crtRoom));
                    }
                }
            }
        }

        /// <summary>
        /// 在收到回复后，将原本由请求 id 创建的房间转化为具有房间 id 的房间
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="roomId"></param>
        private void RequestTransform(ulong requestId, ulong roomId)
        {
            if (_requestDic.TryGetValue(requestId, out var list))
            {
                foreach (var room in list)
                {
                    room.SetRoomId(roomId);
                }
                
                if (_roomDic.TryGetValue(roomId, out var roomList))
                {
                    roomList.AddRange(list);
                    list.Clear();
                }
                else
                {
                    _roomDic.Add(roomId, list);
                }
                _requestDic.Remove(requestId);
            }
        }
    }
}