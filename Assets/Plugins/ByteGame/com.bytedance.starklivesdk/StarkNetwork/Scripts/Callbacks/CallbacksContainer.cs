// Copyright @ www.bytedance.com
// Time: 2022-08-18
// Author: wuwenbin@bytedance.com
// Description: 回调目标集合，用于集中处理各类型回调

using System.Collections.Generic;

namespace StarkNetwork
{
    public class ConnectionCallbacksContainer: List<IConnectionCallbacks>, IConnectionCallbacks
    {
        public void OnConnected(SerializedConnectResult result)
        {
            foreach (IConnectionCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnConnected(result));
            }
        }

        public void OnConnectFailed(SerializedConnectFailedResult result)
        {
            foreach (IConnectionCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnConnectFailed(result));
            }
        }

        public void OnDisconnected()
        {
            foreach (IConnectionCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnDisconnected());
            }
        }

        public void OnConnectClosed(SerializedConnectCloseMessage msg)
        {
            foreach (IConnectionCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnConnectClosed(msg));
            }
        }

        public void OnPlayerInfoGot(SerializedPlayerCurrentInfo info)
        {
            foreach (IConnectionCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnPlayerInfoGot(info));
            }
        }
    }

    public class InRoomCallbacksContainer: List<IInRoomCallbacks>, IInRoomCallbacks
    {
        public void OnPlayerEnter(SerializedUserEnterOrLeaveInfo newPlayer)
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnPlayerEnter(newPlayer));
            }
        }

        public void OnPlayerLeft(SerializedUserEnterOrLeaveInfo leavePlayer)
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnPlayerLeft(leavePlayer));
            }
        }

        public void OnRoomPropertiesUpdate(SerializedRoomMetaDataUpdateResult propertiesThatChanged)
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnRoomPropertiesUpdate(propertiesThatChanged));
            }
        }

        public void OnPlayerPropertiesUpdate()
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnPlayerPropertiesUpdate());
            }
        }

        public void OnMasterClientSwitched(SerializedRoomOwnerUpdateInfo info)
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnMasterClientSwitched(info));
            }
        }
        public void OnSwitchRoomToken(SerializedSwitchRoomRst switchInfo)
        {
            foreach (IInRoomCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnSwitchRoomToken(switchInfo));
            }
        }
    }

    public class MatchmakingCallbacksContainer : List<IMatchmakingCallbacks>, IMatchmakingCallbacks
    {
        public void OnMatched(SerializedRoomInfoMsg msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnMatched(msg));
            }
        }

        public void OnMatchFailed(SerializedErrorInfo msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnMatchFailed(msg));
            }
        }

        public void OnCreatedRoom(SerializedRoomInfoMsg msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnCreatedRoom(msg));
            }
        }

        public void OnCreateRoomFailed(SerializedErrorInfo msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnCreateRoomFailed(msg));
            }
        }

        public void OnJoinedRoom(SerializedRoomInfoMsg msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnJoinedRoom(msg));
            }
        }

        public void OnJoinRoomFailed(SerializedErrorInfo msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnJoinRoomFailed(msg));
            }
        }

        public void OnLeftRoom(SerializedRoomLeaveResult msg)
        {
            foreach (IMatchmakingCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnLeftRoom(msg));
            }
        }
    }
    
    public class SyncMessageCallbackContainer : List<ISyncMessageCallbacks>, ISyncMessageCallbacks
    {
        public void OnSyncMessage(SerializedSyncMsgInfo msg)
        {
            foreach (ISyncMessageCallbacks target in this)
            {
                SafeCaller.Call(() => target.OnSyncMessage(msg));
            }
        }
    }

    public class ErrorInfoCallbackContainer : List<IErrorInfoCallback>, IErrorInfoCallback
    {
        public void OnErrorInfo(SerializedErrorInfo errorInfo)
        {
            foreach (IErrorInfoCallback target in this)
            {
                SafeCaller.Call(() => target.OnErrorInfo(errorInfo));
            }
        }
    }
    
    public class OperationCallbackContainer : List<IOperationCallback>, IOperationCallback
    {
        public void OnOperation(NetworkOperation operation, ulong requestId)
        {
            foreach (IOperationCallback target in this)
            {
                SafeCaller.Call(() => target.OnOperation(operation, requestId));
            }
        }
    }
    
}