// Copyright @ www.bytedance.com
// Time: 2022-08-17
// Author: wuwenbin@bytedance.com
// Description: 各类网络 Callback 的接口汇总

using System.Collections;
using StarkMatchmaking;

namespace StarkNetwork
{
    public interface ICallbackBase
    {

    }

    public interface IConnectionCallbacks: ICallbackBase
    {
        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="result"></param>
        void OnConnected(SerializedConnectResult result);

        /// <summary>
        /// 连接网络失败
        /// </summary>
        /// <param name="result"></param>
        void OnConnectFailed(SerializedConnectFailedResult result);
        
        /// <summary>
        /// 网络临时断连，期间 Native 会自动重试
        /// </summary>
        void OnDisconnected();

        /// <summary>
        /// 和服务的通信完全断开
        /// </summary>
        /// <param name="msg"></param>
        void OnConnectClosed(SerializedConnectCloseMessage msg);

        /// <summary>
        /// 主动向服务端查询当前自己的状态后，得到服务端返回结果
        /// </summary>
        /// <param name="info">当前自身状态，主要是当前所在房间信息列表</param>
        void OnPlayerInfoGot(SerializedPlayerCurrentInfo info);
    }

    public interface IMatchmakingCallbacks: ICallbackBase
    {
        void OnMatched(SerializedRoomInfoMsg msg);
        void OnMatchFailed(SerializedErrorInfo msg);
        void OnCreatedRoom(SerializedRoomInfoMsg msg);
        void OnCreateRoomFailed(SerializedErrorInfo msg);
        void OnJoinedRoom(SerializedRoomInfoMsg msg);
        void OnJoinRoomFailed(SerializedErrorInfo msg);
        void OnLeftRoom(SerializedRoomLeaveResult msg);
    }

    public interface IInRoomCallbacks: ICallbackBase
    {
        void OnPlayerEnter(SerializedUserEnterOrLeaveInfo newPlayer);
        void OnPlayerLeft(SerializedUserEnterOrLeaveInfo leavePlayer);
        void OnRoomPropertiesUpdate(SerializedRoomMetaDataUpdateResult propertiesThatChanged);
        void OnPlayerPropertiesUpdate();
        void OnMasterClientSwitched(SerializedRoomOwnerUpdateInfo info);
        void OnSwitchRoomToken(SerializedSwitchRoomRst switchInfo);
    }

    public interface ISyncMessageCallbacks : ICallbackBase
    {
        void OnSyncMessage(SerializedSyncMsgInfo msg);
    }

    public interface IErrorInfoCallback: ICallbackBase
    {
        void OnErrorInfo(SerializedErrorInfo errorInfo);
    }
    
    /// <summary>
    /// 玩家向服务端发送消息时回调，由客户端自己抛出，用于监控操作行为
    /// </summary>
    public interface IOperationCallback : ICallbackBase
    {
        void OnOperation(NetworkOperation operation, ulong requestId);
    }
}