using System;
using System.Collections.Generic;

namespace StarkNetwork
{
    /// <summary>
    /// 对网络房间逻辑的闭环封装，相关回调由 NetworkRoomManager 控制，以 event 绑定的方式对外暴露网络消息
    /// </summary>
    public interface INetworkRoom : IInRoomCallbacks, IMatchmakingCallbacks, ISyncMessageCallbacks
    {
        #region Properties

        /// <summary>
        /// 房间相关操作绑定的唯一标识，用于在还没有房间 Id 的时候创建 NetworkRoom
        /// </summary>
        ulong RequestId { get; }

        /// <summary>
        /// 房间 Id
        /// </summary>
        ulong RoomId { get;}

        /// <summary>
        /// 当前房间状态
        /// </summary>
        RoomState State { get; }

        /// <summary>
        /// 当前房间信息
        /// </summary>
        SerializedRoomInfoMsg RoomInfo { get; }

        /// <summary>
        /// 当前房间用户 Id 列表
        /// </summary>
        HashSet<ulong> Users { get; }

        /// <summary>
        /// 当前房间人数上限
        /// </summary>
        int MaxUserCount { get; }

        /// <summary>
        /// 当前房间房主 Id
        /// </summary>
        ulong OwnerId { get; }

        /// <summary>
        /// 当前房间里，是否为房主
        /// </summary>
        bool IsOwner { get; }
        
        /// <summary>
        /// 房间元信息
        /// </summary>
        Dictionary<string, string> MetaData { get; }

        /// <summary>
        /// 当前房间内的历史消息记录
        /// </summary>
        List<SerializedSyncMsgInfo> HistoryRoomMessages { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 内部使用，将 RequestId 创建的 Room 转化为正常的 RoomId
        /// </summary>
        /// <param name="roomId"></param>
        void SetRoomId(ulong roomId);
        void SetInfo(SerializedRoomInfoMsg info);
        void SetInfo(SerializedRoomInfo info);
        void SetState(RoomState state);
        void JoinRoom();
        void LeaveRoom();
        void UpdateMetaData(Dictionary<string, string> metaData);
        void KickOutUser(ulong userId);
        void SyncMessage(string msg, ulong toUserId);

        #endregion

        #region Events

        /// <summary>
        /// 房间创建匹配和进入事件
        /// </summary>
        event Action<RoomMatchEvent> OnMatchEvent;

        /// <summary>
        /// 房间退出事件
        /// </summary>
        event Action<RoomLeaveReason> OnLeft;
        
        /// <summary>
        /// 进出房间时的状态改变事件
        /// </summary>
        event Action<RoomState> OnStateChanged;
        
        /// <summary>
        /// 房间内玩家数量的改变
        /// </summary>
        event Action<RoomUserChangeEvent, ulong> OnUserChanged;
        
        /// <summary>
        /// 房间状态以外的信息改变
        /// </summary>
        event Action OnInfoChanged;
        
        /// <summary>
        /// 收到房间内信息
        /// </summary>
        event Action<SerializedSyncMsgInfo> OnRoomMessage;
        
        /// <summary>
        /// 收到房间内报错
        /// </summary>
        event Action<SerializedErrorInfo> OnError;

        #endregion
    }
}