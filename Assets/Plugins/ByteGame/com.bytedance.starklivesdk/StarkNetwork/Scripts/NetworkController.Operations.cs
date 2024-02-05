// Copyright @ www.bytedance.com
// Time: 2022-08-16
// Author: wuwenbin@bytedance.com
// Description: 

using System;
using System.Collections.Generic;

namespace StarkNetwork
{
    public static partial class NetworkController
    {
        /// <summary>
        /// 设置是否开启调试,设置为true时，即可在Unity中查看C++内的log，仅内部使用
        /// </summary>
        /// <param name="debug"></param>
        public static void SetDebug(bool debug)
        {
            if (debug)
            {
                // InitCSharpDelegate(LogMessageFromCpp);
            }
        }

        /// <summary>
        /// 获取 RTT(Round-Trip Time 往返时延)
        /// </summary>
        /// <returns></returns>
        public static long GetRTT()
        {
            return Client.Rtt;
        }

        /// <summary>
        /// 上线、登录
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool ConnectUsingSettings(IConnectOption option)
        {
            return SendOperation(new NetworkOperation
            {
                type = SendingOperationType.CONNECT,
                data = new ConnectOperationData
                {
                    host = option.Address,
                    port = option.Port,
                    token = option.Token,
                    appId = "test",
                    isFake = option.IsFake,
                }
            });
        }
        
        /// <summary>
        /// 下线、退出
        /// </summary>
        /// <returns></returns>
        public static bool CloseConnection()
        {
            return SendOperation(new NetworkOperation
            {
                type = SendingOperationType.CLOSE,
            });
        }

        /// <summary>
        /// 向服务器查询自身状态（房间信息）
        /// </summary>
        public static void GetCurrentState()
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.GET_CURRENT_STATE,
            });
        }

        /// <summary>
        /// 匹配房间
        /// </summary>
        /// <param name="policyName">匹配策略名，需要服务端支持</param>
        /// <param name="ifNotFoundCreateNew">如果没有合适的房间，直接创建一个新的并加入</param>
        /// <param name="joinImmediately">匹配到房间以后立即加入</param>
        /// <param name="needDs">是否需要 DS 服务器</param>
        /// <param name="filters">匹配的筛选条件，需要服务端支持</param>
        /// <param name="maxPlayerCountIfCreate">如果创建房间，最大的玩家数量</param>
        /// <param name="requestIdReceiver">（可选）接收一个请求 id，可以和后续的结果消息相对应</param>
        public static void MatchRoom(string policyName, bool ifNotFoundCreateNew, bool joinImmediately, Dictionary<string, string> filters, uint maxPlayerCountIfCreate, bool needDs, Action<ulong> requestIdReceiver = null)
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.MATCH,
                data = new MatchOperationData
                {
                    policyName = policyName,
                    createNew = ifNotFoundCreateNew,
                    joinImmediately = joinImmediately,
                    filters = filters,
                    needDs = needDs,
                    createRoomOption = ifNotFoundCreateNew
                        ? new CreateRoomOperationData
                        {
                            maxPlayerCount = maxPlayerCountIfCreate,
                            needDs = needDs,
                            metaData = filters
                        }
                        : default,
                },
                requestIdReceiver = requestIdReceiver
            });
        }

        /// <summary>
        /// 取消匹配
        /// </summary>
        public static void MatchCancel()
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.MATCH_CANCEL,
            });
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="maxPlayerNum">最大玩家数</param>
        /// <param name="needDs">是否需要 DS 服务器</param>
        /// <param name="metaData">房间初始信息，Key Value Pair</param>
        /// <param name="requestIdReceiver">（可选）接收一个请求 id，可以和后续的结果消息相对应</param>
        public static void CreateRoom(uint maxPlayerNum, bool needDs, Dictionary<string, string> metaData = null, Action<ulong> requestIdReceiver = null)
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.CREATE,
                data = new CreateRoomOperationData
                {
                    maxPlayerCount = maxPlayerNum,
                    needDs = needDs,
                    metaData = metaData,
                },
                requestIdReceiver = requestIdReceiver
            });
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="roomId">房间号</param>
        public static void JoinRoom(ulong roomId)
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.JOIN,
                data = new JoinRoomOperationData
                {
                    roomId = roomId,
                }
            });
        }
        
        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="roomId">房间号</param>
        public static void LeaveRoom(ulong roomId)
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.LEAVE,
                data = new LeaveRoomOperationData()
                {
                    roomId = roomId,
                }
            });
        }

        /// <summary>
        /// 更新房间信息
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="changes">需要变更的内容，Key Value Pair</param>
        public static void UpdateRoomMetaData(ulong roomId, Dictionary<string, string> changes)
        {
            SendOperation(new NetworkOperation()
            {
                type = SendingOperationType.UPDATE_ROOM_META_DATA,
                data = new UpdateRoomMetaDataOperationData()
                {
                    roomId = roomId,
                    metaData = changes
                }
            });
        }

        /// <summary>
        /// 将玩家踢出房间，仅房主有权限
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="userId">用户 id</param>
        public static void KickOutUser(ulong roomId, ulong userId)
        {
            SendOperation(new NetworkOperation()
            {
                type = SendingOperationType.KICK,
                data = new KickOutUserOperationData()
                {
                    roomId = roomId,
                    userId = userId
                }
            });
        }
        
        /// <summary>
        /// 将房主转移，仅房主有权限
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="userId">用户 id</param>
        public static void HandOverOwner(ulong roomId, ulong userId)
        {
            SendOperation(new NetworkOperation()
            {
                type = SendingOperationType.HANDOVER,
                data = new HandOverOperationData()
                {
                    roomId = roomId,
                    userId = userId
                }
            });
        }
        
        /// <summary>
        /// 场景切换获取下次匹配token
        /// </summary>
        /// <param name="roomId">房间号</param>
        public static void SwitchRoom(ulong roomId)
        {
            SendOperation(new NetworkOperation()
            {
                type = SendingOperationType.SWITCH_ROOM,
                data = new SwitchRoomOperationData()
                {
                    roomId = roomId,
                }
            });
        }
        
        /// <summary>
        /// 场景切换获取下次匹配token
        /// </summary>
        /// <param name="roomId">房间号</param>
        public static void JoinRoomWithToken(string token, Action<ulong> requestIdReceiver = null)
        {
            SendOperation(new NetworkOperation()
            {
                type = SendingOperationType.JOIN_ROOM_WITH_TOKEN,
                data = new JoinRoomWithTokenOperationData()
                {
                    token = token,
                },
                requestIdReceiver = requestIdReceiver
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="roomId">房间号</param>
        /// <param name="userId">用户 id，如果填 0 则为广播</param>
        public static void SyncMessage(string msg, ulong roomId, ulong userId)
        {
            SendOperation(new NetworkOperation
            {
                type = SendingOperationType.MESSAGE,
                data = new SyncMessageOperationData()
                {
                    roomId = roomId,
                    userId = userId,
                    msg = msg
                }
            });
        }
        
        /// <summary>
        /// 通过 requestId 查找此前的操作，用于错误消息解析
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static NetworkOperation FindOperationRequest(ulong requestId)
        {
            return Client.FindOperationRequest(requestId);
        }

        private static bool SendOperation(NetworkOperation operation)
        {
            // todo: send immediately
            return Client.EnqueueSending(operation);
        }
    }
}