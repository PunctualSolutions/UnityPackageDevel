// Copyright @ www.bytedance.com
// Time: 2022-08-04
// Author: wuwenbin@bytedance.com
// Description: 和 Native 端保持一致的事件枚举定义，用于消息接受

using System;

namespace StarkNetwork
{
    [Obsolete("Use MessageType instead.")]
    enum EventDefine {

        Connected = 10000,          //连接成功
        ConnectFailed,              //连接失败
        Disconnected,               //网络断联(临时，会重试)
        ConnectClosed,              //完全断开

        RoomMatched = 10100,       //匹配到房间时的事件，此事件应该携带房间信息
        RoomMatchFailed,           //没有任何一个可加入的房间
        MatchMakingProcessUpdate,  //当匹配进度更新时的事件，此事件应该包含前面还有多少待匹配的玩家数量信息  暂未实现
        MatchCanceled,

        RoomCreated = 10200,  //房间创建成功的时间，携带房间信息
        RoomCreateFailed,
        RoomEntered,          //当玩家加入到房间时的事件，携带房间信息
        RoomEnteredFailed,    //玩家加入房间失败的事件
        PeopleEnteredRoom,    //当有其他玩家加入到房间时，携带玩家信息
        PeopleLeavedRoom,     //当有玩家离开了房间的时候的事件
        RoomDateUpdate,       //房间相关元数据信息改变时的事件，携带变更的数据信息 暂未实现
        RoomLeaved,           //从房间中离开时收到的回调
        RoomOwnerUpdate,      //房间的房主信息变更时的消息
        RoomKickUser,         //踢人操作结果

        SyncMsgReceived = 10800,  //收到同步的信息，

        OnError = 11000,  //收到错误信息
        RpcServerCall = 50000,  //收到服务端的Rpc请求
    }
}
