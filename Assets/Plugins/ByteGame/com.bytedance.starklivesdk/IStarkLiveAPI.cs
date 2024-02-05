using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StarkLive
{
    public interface IStarkLiveAPI
    {
        /// <summary>
        /// 是否已初始化
        /// </summary>
        /// <returns>已初始化返回true，否则返回false</returns>
        bool IsInitialized();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appId">小玩法的AppId</param>
        /// <param name="callback">初始化回调</param>
        /// <returns>始终返回true</returns>
        bool Init(string appId, Action<int, string, ulong> callback = null);
        /// <summary>
        /// 反初始化
        /// </summary>
        /// <returns>始终返回true</returns>
        bool UnInit();
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="msgType">任务类型：点赞/评论/送礼</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，预留暂未使用</param>
        void StartLiveTask(LiveEventType msgType, Action<int, string, object> callback = null);

        /// <summary>
        /// 结束任务 
        /// </summary>
        /// <param name="msgType">任务类型：点赞/评论/送礼</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，预留暂未使用</param>
        void StopLiveTask(LiveEventType msgType, Action<int, string, object> callback = null);
        
        /// <summary>
        /// 查询任务状态 
        /// </summary>
        /// <param name="msgType">任务类型：点赞/评论/送礼</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，任务状态</param>
        void QueryStatus(LiveEventType msgType, Action<int, string, LiveTaskState> callback);

        /// <summary>
        /// 礼物置顶   
        /// </summary>
        /// <param name="giftIds">要指定的礼物Id数组</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，设置成功的礼物id数组</param>
        void SetGiftTop(string[] giftIds, Action<int, string, string[]> callback = null);
        
        /// <summary>
        /// 分页查询（礼物）推送数据
        /// </summary>
        /// <param name="pageNum">页码，从1开始</param>
        /// <param name="pageSize">每页个数，最大不超过100</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，查询结果</param>
        void QueryFailedGift(int pageNum, int pageSize, Action<int, string, QueryFailedGiftPageData> callback = null);
        
        /// <summary>
        /// 设置收到评论回调
        /// </summary>
        /// <param name="callback">回调函数，参数：评论数据数组</param>
        void SetReceiveCommentCallback(Action<Comment[]> callback);
        /// <summary>
        /// 设置收到礼物回调
        /// </summary>
        /// <param name="callback">回调函数，参数：礼物数据数组</param>
        void SetReceiveGiftCallback(Action<Gift[]> callback);
        /// <summary>
        /// 设置收到点赞回调
        /// </summary>
        /// <param name="callback">回调函数，参数：点赞数据数组</param>
        void SetReceiveLikeCallback(Action<Like[]> callback);

        /// <summary>
        /// 读排行榜数据
        /// </summary>
        /// <param name="rankId">排行榜id，你可以有多个排行榜如积分榜，伤害榜，财富榜等等</param>
        /// <param name="rankTimeType">排行榜的时间类型，每个rankid都有日榜、周榜、月榜和总榜</param>
        /// <param name="pageIdx">页码，0为第一页</param>
        /// <param name="pageSize">每页个数，最小值5，最大值100</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，排行榜数据</param>
        void GetRankData(int rankId, RankTimeType rankTimeType, int pageIdx, int pageSize,
            Action<int, string, RankData[]> callback);
        /// <summary>
        /// 根据OpenIdList读排行榜数据
        /// </summary>
        /// <param name="rankId">排行榜id，你可以有多个排行榜如积分榜，伤害榜，财富榜等等</param>
        /// <param name="rankTimeType">排行榜的时间类型，每个rankid都有日榜、周榜、月榜和总榜</param>
        /// <param name="openIdList">openId数组</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，排行榜数据</param>
        void GetRankDataWithUserIds(int rankId, RankTimeType rankTimeType, string[] openIdList, Action<int, string, RankData[]> callback);
        
        /// <summary>
        /// 写排行榜数据
        /// </summary>
        /// <param name="rankId">排行榜id，你可以有多个排行榜如积分榜，伤害榜，财富榜等等</param>
        /// <param name="list">需要写入的数据</param>
        /// <param name="callback">回调函数，参数：errorCode，errorMessage，rankId</param>
        void SetRankDataList(int rankId, List<WriteRankData> list, Action<int, string, int> callback);
        
        // 以下接口开发者无需使用-------------------------------------------------
        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="tick"></param>
        void SendHeartbeatServerRpc(ulong tick);
        
        /// <summary>
        /// 断线重连或登录时查询stark live server丢失的礼物
        /// </summary>
        void QueryNetDisconnectedFailedGifts();

        /// <summary>
        /// 初始化（连接）失败时按失败执行初始化回调
        /// </summary>
        void ExecuteInitFailedCallback(int errorCode, string errorMessage);
        /// <summary>
        /// 重连成功后按重连前各任务启动状态重启任务
        /// </summary>
        void NetResumeCheckTask(bool markStartTask);

        void SetLiveCallbacks(ILiveObj obj);

    }
}