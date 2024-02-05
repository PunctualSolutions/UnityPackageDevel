using System;
using System.Collections.Generic;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkLive
{
    public abstract class ILiveObj : RpcBase
    {
        /// <summary>
        /// 初始化服务器调用，收到这个必成功，不成功的情况通过net连接失败返回
        /// </summary>
        /// <param name="uid"></param>
        public abstract void InitInfoClientRpc(ulong uid);
        /// <summary>
        /// 设置初始化的自定义回调，将会在执行到InitInfoClientRpc后执行initCallback，或初始化失败（网络问题）时返回
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="appId"></param>
        public abstract void SetInitCallback(Action<int, string, ulong> callback);
        /// <summary>
        /// 设置appId
        /// </summary>
        /// <param name="appId"></param>
        public abstract void SetAppInfo(string appId);
        /// <summary>
        /// 启动任务服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="roomId">直播间id</param>
        /// <param name="msgType">启动的任务的类型：送礼/评论/点赞</param>
        public abstract void BeginRoomRspClientRpc(int errorCode, string errorMessage, string roomId, string msgType);
        /// <summary>
        /// 设置启动任务的自定义回调，将会在执行到BeginRoomRspClientRpc后执行startTaskCallback
        /// </summary>
        /// <param name="msgType">启动任务的类型</param>
        /// <param name="startTaskCallback">开发者自定义回调</param>
        public abstract void SetStartTaskCallback(LiveEventType msgType, Action<int, string, object> startTaskCallback);
        /// <summary>
        /// 礼物置顶服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="payload"></param>
        public abstract void TopGiftClientRpc(int errorCode, string errorMessage, string payload);
        /// <summary>
        /// 设置礼物置顶的自定义回调，将会在置顶到TopGiftClientRpc后置顶setGiftTopCallback
        /// </summary>
        /// <param name="callback"></param>
        public abstract void SetSetGiftTopCallback(Action<int, string, string[]> callback);
        /// <summary>
        /// event推送服务器调用
        /// </summary>
        /// <param name="msgType">推送消息的类型：送礼/评论/点赞</param>
        /// <param name="pushId">推送id，用于向服务器发送确认</param>
        /// <param name="payload">消息信息</param>
        public abstract void RoomEventClientRpc(string msgType, ulong pushId, string payload);

        public abstract string GetAppId();
        public abstract string GetUid();

        public abstract void GetRankRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId,
            int timeType, int pageIdx, int pageSize, string rankData);
        public abstract void SetRankListRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId);
        public abstract void SetReadRankCallback(Action<int, string, RankData[]> callback);
        public abstract void SetWriteRankCallback(Action<int, string, int> callback);

        public abstract void GetRankDataListRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId,
            int timeType, string rankData);
        
        /// <summary>
        /// 设置收到评论的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public abstract void SetReceiveCommentCallback(Action<Comment[]> callback);
        /// <summary>
        /// 设置收到礼物的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public abstract void SetReceiveGiftCallback(Action<Gift[]> callback);
        /// <summary>
        /// 设置收到点赞的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public abstract void SetReceiveLikeCallback(Action<Like[]> callback);
        /// <summary>
        /// 设置app info服务器调用
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="appid"></param>
        public abstract void AppInfoConfirmClientRpc(ulong uid, string appid);
        /// <summary>
        /// 结束任务服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="payload"></param>
        /// <param name="msgType">结束任务类型：送礼/评论/点赞</param>
        public abstract void EndRoomClientRpc(int errorCode, string errorMessage, string payload, string msgType);
        /// <summary>
        /// 设置停止任务的回调函数
        /// </summary>
        /// <param name="msgType">结束任务类型：送礼/评论/点赞</param>
        /// <param name="callback"></param>
        public abstract void SetStopTaskCallback(LiveEventType msgType, Action<int, string, object> callback);
        /// <summary>
        /// 查询任务状态服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="payload"></param>
        /// <param name="msgType">结束任务类型：送礼/评论/点赞</param>
        public abstract void QueryRoomStatusClientRpc(int errorCode, string errorMessage, string payload, string msgType);
        /// <summary>
        /// 设置查询任务状态的回调函数
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="callback"></param>
        public abstract void SetQueryTaskStatusCallback(LiveEventType msgType, Action<int, string, LiveTaskState> callback);
        /// <summary>
        /// 查询失败礼物服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="payload"></param>
        public abstract void QueryEventOfPageClientRpc(int errorCode, string errorMessage, string payload);
        /// <summary>
        /// 设置查询丢失礼物的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public abstract void SetQueryFailedGiftCallback(Action<int, string, QueryFailedGiftPageData> callback);
        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="tick"></param>
        public abstract void HeartbeatClientRpc(ulong tick);
        /// <summary>
        /// 获取RoomId
        /// </summary>
        /// <returns></returns>
        public abstract string GetRoomId();
        /// <summary>
        /// 获取初始化状态
        /// </summary>
        /// <returns></returns>
        public abstract bool GetInitState();
        /// <summary>
        /// 获取丢失的礼物的pushIds，格式：json： ["id1", "id2"]
        /// </summary>
        /// <returns></returns>
        public abstract string GetLostGiftsPushIds();
        /// <summary>
        /// 查询stark live server丢失的礼物的信息服务器调用
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="msgId"></param>
        /// <param name="payloads">补发的丢失push通知payload</param>
        /// <param name="msgType">事件类型</param>
        public abstract void NotifyLostGiftClientRpc(int errorCode, string errorMessage, ulong msgId, ulong roomId, string payloads,
            string msgType);

        /// <summary>
        /// 服务器内部错误通知
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        public abstract void NotifyServerErrorClientRpc(int errorCode, string errorMessage);
        /// <summary>
        /// 检查任务状态并启动
        /// </summary>
        /// <param name="token"></param>
        /// <param name="roomId"></param>
        /// <typeparam name="T"></typeparam>
        public abstract void NetResumeCheckTask(string token, string roomId, bool markStartTask);
        /// <summary>
        /// 初始化失败执行回调
        /// </summary>
        public abstract void ExecuteInitFailedCallback(int errorCode, string errorMessage);
        /// <summary>
        /// 反初始化重置状态
        /// </summary>
        public abstract void Uninit();
    }
}