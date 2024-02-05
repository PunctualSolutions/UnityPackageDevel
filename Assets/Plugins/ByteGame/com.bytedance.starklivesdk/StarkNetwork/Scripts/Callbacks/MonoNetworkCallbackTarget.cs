// Copyright @ www.bytedance.com
// Time: 2022-08-18
// Author: wuwenbin@bytedance.com
// Description: 继承自 MonoBehaviour，通用的回调对象基类，虚拟实现了各类 Callback 接口，并提供了注册和注销接口

using UnityEngine;

namespace StarkNetwork
{
    public class MonoNetworkCallbackTarget: MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ISyncMessageCallbacks, IErrorInfoCallback, IOperationCallback
    {
        public void Registry()
        {
            NetworkController.AddCallbackTarget(this);
        }

        public void Unregistry()
        {
            NetworkController.RemoveCallbackTarget(this);
        }

        public virtual void OnConnected(SerializedConnectResult result)
        {
            
        }
        
        public virtual void OnConnectFailed(SerializedConnectFailedResult result)
        {
            
        }

        public virtual void OnDisconnected()
        {
            
        }

        public virtual void OnConnectClosed(SerializedConnectCloseMessage msg)
        {
            
        }

        public void OnPlayerInfoGot(SerializedPlayerCurrentInfo info)
        {
            
        }

        public virtual void OnMatched(SerializedRoomInfoMsg msg)
        {
            
        }

        public virtual void OnMatchFailed(SerializedErrorInfo msg)
        {
            
        }

        public virtual void OnCreatedRoom(SerializedRoomInfoMsg msg)
        {
            
        }

        public virtual void OnCreateRoomFailed(SerializedErrorInfo msg)
        {
            
        }

        public virtual void OnJoinedRoom(SerializedRoomInfoMsg msg)
        {
            
        }

        public virtual void OnJoinRoomFailed(SerializedErrorInfo msg)
        {
            
        }

        public virtual void OnJoinRandomFailed(SerializedErrorInfo msg)
        {
            
        }

        public virtual void OnLeftRoom(SerializedRoomLeaveResult msg)
        {
            
        }

        public virtual void OnPlayerEnter(SerializedUserEnterOrLeaveInfo newPlayer)
        {
            
        }

        public virtual void OnPlayerLeft(SerializedUserEnterOrLeaveInfo leavePlayer)
        {
            
        }

        public virtual void OnRoomPropertiesUpdate(SerializedRoomMetaDataUpdateResult propertiesThatChanged)
        {
            
        }

        public virtual void OnPlayerPropertiesUpdate()
        {
            
        }

        public virtual void OnMasterClientSwitched(SerializedRoomOwnerUpdateInfo info)
        {
            
        }

        public void OnSwitchRoomToken(SerializedSwitchRoomRst switchInfo)
        {
            
        }

        public virtual void OnSyncMessage(SerializedSyncMsgInfo msg)
        {
            
        }

        public virtual void OnErrorInfo(SerializedErrorInfo errorInfo)
        {
            
        }

        public void OnOperation(NetworkOperation operation, ulong requestId)
        {
            
        }
    }
}