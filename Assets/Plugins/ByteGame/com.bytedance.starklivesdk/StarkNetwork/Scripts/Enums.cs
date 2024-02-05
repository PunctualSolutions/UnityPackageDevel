// Copyright @ www.bytedance.com
// Time: 2022-08-17
// Author: wuwenbin@bytedance.com
// Description:

namespace StarkNetwork
{
    /// <summary>
    /// Client 当前的连接状态
    /// </summary>
    public enum ClientState
    {
        STOP,       // 关闭状态
        CONNECTING, // 正在连接
        RUNNING,    // 已连接
    }

    public enum ConnectResumeMod
    {
        FirstTime,  // 首次连接
        ReConnect,  // 重连
    }

    public enum ConnectCloseReason
    {
        Initiative = -1,    // 主动断开(客户端自己发送)
        KickedByReLogin,    // 重复登录被顶号踢下线
        KickedByGame,       // 业务逻辑触发踢下线
    }

    /// <summary>
    /// 房间当前的状态
    /// </summary>
    public enum RoomState
    {
        OUTER,      // 不在房间内
        JOINING,    // 正在加入
        INNER,      // 连接状态
        QUITING,    // 正在退出
    }
    
    /// <summary>
    /// 房间用户改变事件
    /// </summary>
    public enum RoomUserChangeEvent
    {
        Add,        // 用户加入
        Remove,     // 用户离开
        OwnerChanged// 房主改变
    }

    /// <summary>
    /// 房间匹配过程中抛出的事件，和 RoomState 不同
    /// </summary>
    public enum RoomMatchEvent
    {
        Created,    // 创建房间事件
        Matched,    // 匹配到房间事件
        Entered,    // 加入房间事件
    }

    /// <summary>
    /// 退出房间的原因
    /// </summary>
    public enum RoomLeaveReason
    {
        Undefined,  // 未知原因
        Initiative, // 主动退出
        BeKickedByOwner,    // 被房主踢出
        RoomNumReachLimited // 当前加入的房间达到上限，自动退出最早的房间
    }

    public enum OperationState
    {
        READY,      // 未发送
        SENDING,    // 已发送
        ANSWERED,   // 收到回复 （成功或失败）
    }
}