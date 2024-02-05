using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkLive
{
    public class LiveObj : ILiveObj
    {
        private enum ReconnectStartTaskState
        {
            NONE,
            HAVE_SEND_REQUEST,
            HAVE_RECEIVE_RESPONSE
        }
        private class ReconnectStartTaskResolver
        {
            public ReconnectStartTaskState State = ReconnectStartTaskState.NONE;
            public int TaskNum = 0;
        }
        private static readonly string LogColor = "yellow";
        private string roomId = string.Empty;
        private static readonly string TAG = "LiveObj";
        private string appId = string.Empty;
        private string uid = String.Empty;
        private Dictionary<LiveEventType, bool> taskStateDic = new Dictionary<LiveEventType, bool>()
        {
            [LiveEventType.GIFT] = false,
            [LiveEventType.LIKE] = false,
            [LiveEventType.COMMENT] = false,
        };
        private Dictionary<LiveEventType, Action<int, string, object>> startTaskCallbackDic = new Dictionary<LiveEventType, Action<int, string, object>>();
        private Dictionary<LiveEventType, Action<int, string, object>> stopTaskCallbackDic = new Dictionary<LiveEventType, Action<int, string, object>>();
        private Dictionary<LiveEventType, Action<int, string, LiveTaskState>> queryTaskStatusCallbackDic = new Dictionary<LiveEventType, Action<int, string, LiveTaskState>>();
        private Action<int, string, ulong> initCallback = null;
        private Action<Comment[]> receiveCommentCallback = null;
        private Action<Gift[]> receiveGiftCallback = null;
        private Action<Like[]> receiveLikeCallback = null;
        private Action<int, string, string[]> setGiftTopCallback = null;
        private Action<int, string, QueryFailedGiftPageData> queryFailedGiftCallback = null;
        private Dictionary<ulong, ulong> lostGiftsPushIdsDic = new Dictionary<ulong, ulong>();
        private List<ulong> latestEventMsgIds = new List<ulong>();
        private List<ulong> latestQueryLostGiftMsgIds = new List<ulong>();
        private Action<int, string, int> writeRankCallback = null;
        private Action<int, string, RankData[]> readRankCallback = null;
        private ReconnectStartTaskResolver reconnectStartTaskResolver = new ReconnectStartTaskResolver()
        {
            State = ReconnectStartTaskState.NONE,
            TaskNum = 0
        };

        private bool init = false;

        private void SetInitState(bool state)
        {
            init = state;
        }

        public override bool GetInitState()
        {
            return init;
        }
        
        public override void Uninit()
        {
            SetInitState(false);
            SetRoomId(string.Empty);
            SetUid(0);
            // reset task state
            foreach (var type in Enum.GetValues(typeof(LiveEventType)))
            {
                taskStateDic[(LiveEventType)type] = false;
            }
        }

        private bool IsTaskHaveStart(LiveEventType type)
        {
            return taskStateDic[type];
        }

        public override void NetResumeCheckTask(string token, string roomId, bool markStartTask)
        {
            foreach (LiveEventType eventType in Enum.GetValues(typeof(LiveEventType)))
            {
                if (IsTaskHaveStart(eventType))
                {
                    var requestType = Utils.GetStartTaskRequestType(eventType);
                    StarkLiveSDK.PrintLog(TAG, $"NetResumeCheckTask eventType: {eventType}, requestType: {requestType}, ServerMethodName: {Consts.RpcCallDatas[requestType].ServerMethodName}", LogColor);
                    string[] args = { token, roomId, Consts.LiveEventMsgStr[(int)eventType] };
                    NetworkHelper.Instance.RecordRequest(requestType, startTaskCallbackDic[eventType]);
                    Rpc.CallServerMethod(Consts.RpcCallDatas[requestType].ServerMethodName, args);
                    if (markStartTask)
                    {
                        reconnectStartTaskResolver.State = ReconnectStartTaskState.HAVE_SEND_REQUEST;
                        reconnectStartTaskResolver.TaskNum++;
                    }
                }
            }
        }

        public override void ExecuteInitFailedCallback(int errorCode, string errorMessage)
        {
            initCallback?.Invoke(errorCode, errorMessage, 0);
        }
        // 初始化
        public override void InitInfoClientRpc(ulong uid)
        {
            SetRoomId(string.Empty);
            SetUid(uid);
            StarkLiveSDK.PrintLog(TAG, $"InitInfoClientRpc uid: {uid}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.INIT);
            
            NetworkHelper.Instance.RecordRequest(RequestType.SET_APP_INFO, initCallback);
            Rpc.CallServerMethod(Consts.REPORT_APP_INFO_SERVER_RPC, appId, "");
            StarkLiveSDK.PrintLog(TAG, $"NetworkController.NetworkState= {NetworkController.NetworkState}");
        }
        
        // set app info
        public override void AppInfoConfirmClientRpc(ulong uid, string appid)
        {
            SetInitState(true);
            SetRoomId(string.Empty);
            SetUid(uid);
            StarkLiveSDK.PrintLog(TAG, $"AppInfoConfirmClientRpc uid: {uid}, appid: {appid}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.SET_APP_INFO);
            // 执行回调
            initCallback?.Invoke(0, "", uid);
            // 解决服务器因意外重启，但表现上==重连的问题
            // 如果因为断连请求过start task，并已经收到了回包（回包会报错，并且该回包的报错不会返回给开发者），重新请求start task
            if (reconnectStartTaskResolver.State == ReconnectStartTaskState.HAVE_RECEIVE_RESPONSE)
            {
                reconnectStartTaskResolver.State = ReconnectStartTaskState.NONE;
                StarkLiveSDK.API.NetResumeCheckTask(false);
            }
        }
        public override void SetInitCallback(Action<int, string, ulong> callback)
        {
            initCallback = callback;
        }

        public override void SetAppInfo(string appId)
        {
            this.appId = appId;
        }

        public override string GetAppId()
        {
            return appId;
        }

        private void SetUid(ulong uid)
        {
            this.uid = uid.ToString();
        }

        private void SetRoomId(string roomId)
        {
            this.roomId = roomId;
        }
        public override string GetUid()
        {
            return uid;
        }
        // 开始任务
        public override void BeginRoomRspClientRpc(int errorCode, string errorMessage, string roomId, string msgType)
        {
            StarkLiveSDK.PrintLog(TAG, $"BeginRoomRspClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, roomId: {roomId}, msgType: {msgType}", LogColor);
            NetworkHelper.Instance.ClearRecord(Utils.GetStartTaskRequestType(Consts.LiveEventMsgStrDic[msgType]));
            SetRoomId(roomId);
            
            if (reconnectStartTaskResolver.TaskNum > 0)
            {
                if (errorCode == Consts.MART_CHECK_TASK_STARK_SERVER_ERRORCODE)
                {
                    reconnectStartTaskResolver.TaskNum--;
                    if (reconnectStartTaskResolver.TaskNum == 0)
                    {
                        reconnectStartTaskResolver.State = ReconnectStartTaskState.HAVE_RECEIVE_RESPONSE;
                        StarkLiveSDK.PrintLog(LiveObj.TAG, $"BeginRoomRspClientRpc markStartTask && errorCode == {Consts.MART_CHECK_TASK_STARK_SERVER_ERRORCODE}, wait for init", LiveObj.LogColor);
                    }
                }
                else
                {
                    StarkLiveSDK.PrintLog(LiveObj.TAG, "BeginRoomRspClientRpc Serious, not in line with expected results", LiveObj.LogColor);
                }
                return;
            }
            
            var type = Consts.LiveEventMsgStrDic[msgType];
            if (errorCode == 0)
            {
                taskStateDic[type] = true;
            }
            if (startTaskCallbackDic.ContainsKey(type) && startTaskCallbackDic[type] != null)
            {
                startTaskCallbackDic[type](errorCode, errorMessage, null);
            }

            if (type == LiveEventType.GIFT)
            {
                // 查询因为断连产生的丢失的礼物，并进行event的补发 
                StarkLiveSDK.API.QueryNetDisconnectedFailedGifts();
            }
        }
        // 结束任务
        public override void EndRoomClientRpc(int errorCode, string errorMessage, string payload, string msgType)
        {
            StarkLiveSDK.PrintLog(TAG, $"EndRoomClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, payload: {payload}", LogColor);
            NetworkHelper.Instance.ClearRecord(Utils.GetStopTaskRequestType(Consts.LiveEventMsgStrDic[msgType]));
            
            var type = Consts.LiveEventMsgStrDic[msgType];
            if (errorCode == 0)
            {
                taskStateDic[type] = false;
            }
            if (stopTaskCallbackDic.ContainsKey(type) && stopTaskCallbackDic[type] != null)
            {
                if (errorCode == 0)
                {
                    EndTaskResponse data = JsonUtility.FromJson<EndTaskResponse>(payload);
                    stopTaskCallbackDic[type](data.err_no, data.err_msg, data.data);
                }
                else
                {
                    stopTaskCallbackDic[type](errorCode, errorMessage, null);
                }
            }
        }
        // 查询任务状态
        public override void QueryRoomStatusClientRpc(int errorCode, string errorMessage, string payload, string msgType)
        {
            StarkLiveSDK.PrintLog(TAG, $"QueryRoomStatusClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, payload: {payload}, msgType: {msgType}", LogColor);
            NetworkHelper.Instance.ClearRecord(Utils.GetQueryTaskStateRequestType(Consts.LiveEventMsgStrDic[msgType]));
            
            var type = Consts.LiveEventMsgStrDic[msgType];
            if (queryTaskStatusCallbackDic.ContainsKey(type) && queryTaskStatusCallbackDic[type] != null)
            {
                if (errorCode == 0)
                {
                    QueryStatusResponse data = JsonUtility.FromJson<QueryStatusResponse>(payload);
                    // StarkLiveSDK.PrintLog(TAG, $"QueryRoomStatusClientRpc logid: {data.logid}, status: {data.data.status}", LogColor);
                    queryTaskStatusCallbackDic[type](data.err_no, data.err_msg, (LiveTaskState)data.data.status);
                }
                else
                {
                    queryTaskStatusCallbackDic[type](errorCode, errorMessage, LiveTaskState.INVALID);
                }
            }
        }
        // 礼物置顶
        public override void TopGiftClientRpc(int errorCode, string errorMessage, string payload)
        {
            StarkLiveSDK.PrintLog(TAG, $"TopGiftClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, payload: {payload}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.TOP_GIFT);
            
            if (errorCode == 0)
            {
                SetTopGiftResponse data = JsonUtility.FromJson<SetTopGiftResponse>(payload);
                setGiftTopCallback?.Invoke(errorCode, errorMessage, data.data.success_top_gift_id_list);
            }
            else
            {
                setGiftTopCallback?.Invoke(errorCode, errorMessage, new string[]{});
            }
        }
        // 查询丢失的礼物
        public override void QueryEventOfPageClientRpc(int errorCode, string errorMessage, string payload)
        {
            StarkLiveSDK.PrintLog(TAG, $"QueryEventOfPageClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, payload: {payload}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.QUERY_FAILED_GIFT);
            
            if (errorCode == 0)
            {
                QueryFailedGiftResponse data = JsonConvert.DeserializeObject<QueryFailedGiftResponse>(payload);
                // QueryFailedGiftResponse data = JsonUtility.FromJson<QueryFailedGiftResponse>(payload);
                if (data.err_no == 0)
                {
                    queryFailedGiftCallback?.Invoke(data.err_no, data.err_msg, data.data);
                }
                else
                {
                    queryFailedGiftCallback?.Invoke(data.err_no, data.err_msg, new QueryFailedGiftPageData());
                }
            }
            else
            {
                queryFailedGiftCallback?.Invoke(errorCode, errorMessage, new QueryFailedGiftPageData());
            }
        }
        // 读排行榜
        public override void GetRankRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId, int timeType, int pageIdx, int pageSize, string rankData)
        {
            StarkLiveSDK.PrintLog(TAG, $"GetRankRspClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, reqId: {reqId}, " +
                                       $"timeType: {timeType}, pageIdx: {pageIdx}, pageSize: {pageSize}, rankData: {rankData}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.READ_RANK);
            GetRankRsp(errorCode, errorMessage, rankData);
        }
        // 写排行榜
        public override void SetRankListRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId)
        {
            StarkLiveSDK.PrintLog(TAG, $"SetRankListRspClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, reqId: {reqId}, rankId: {rankId}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.WRITE_RANK);
            writeRankCallback?.Invoke(errorCode, errorMessage, rankId);
        }
        // 读指定openid的排行榜
        public override void GetRankDataListRspClientRpc(int errorCode, string errorMessage, int reqId, int rankId,
            int timeType, string rankData)
        {
            StarkLiveSDK.PrintLog(TAG, $"GetRankDataListRspClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, reqId: {reqId}, " +
                                       $"timeType: {timeType}, rankData: {rankData}", LogColor);
            NetworkHelper.Instance.ClearRecord(RequestType.READ_RANK_WITH_OPENID);
            GetRankRsp(errorCode, errorMessage, rankData);
        }

        private void GetRankRsp(int errorCode, string errorMessage, string rankData)
        {
            if (errorCode == 0)
            {
                try
                {
                    RankDataServer[] datas = JsonConvert.DeserializeObject<RankDataServer[]>(rankData);
                    List<RankData> rankDatas = new List<RankData>();
                    for (int i = 0; i < datas.Length; i++)
                    {
                        rankDatas.Add(Utils.GetRankData(datas[i]));
                    }
                    readRankCallback?.Invoke(errorCode, errorMessage, rankDatas.ToArray());
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"GetRankRsp Exception e: {e}");
                    readRankCallback?.Invoke(errorCode, errorMessage, new RankData[]{});
                }
            }
            else
            {
                readRankCallback?.Invoke(errorCode, errorMessage, null);
            }
        }
        
        // 设置开始任务回调
        public override void SetStartTaskCallback(LiveEventType msgType, Action<int, string, object> callback)
        {
            if (startTaskCallbackDic.ContainsKey(msgType))
            {
                startTaskCallbackDic[msgType] = callback;
            }
            else
            {
                startTaskCallbackDic.Add(msgType, callback);
            }
        }
        // 设置结束任务回调
        public override void SetStopTaskCallback(LiveEventType msgType, Action<int, string, object> callback)
        {
            if (stopTaskCallbackDic.ContainsKey(msgType))
            {
                stopTaskCallbackDic[msgType] = callback;
            }
            else
            {
                stopTaskCallbackDic.Add(msgType, callback);
            }
        }
        // 设置查询任务状态回调
        public override void SetQueryTaskStatusCallback(LiveEventType msgType,
            Action<int, string, LiveTaskState> callback)
        {
            if (queryTaskStatusCallbackDic.ContainsKey(msgType))
            {
                queryTaskStatusCallbackDic[msgType] = callback;
            }
            else
            {
                queryTaskStatusCallbackDic.Add(msgType, callback);
            }
        }
        // 设置礼物置顶回调
        public override void SetSetGiftTopCallback(Action<int, string, string[]> callback)
        {
            setGiftTopCallback = callback;
        }
        // 设置查询丢失礼物回调
        public override void SetQueryFailedGiftCallback(Action<int, string, QueryFailedGiftPageData> callback)
        {
            queryFailedGiftCallback = callback;
        }
        // 设置读排行榜回调
        public override void SetReadRankCallback(Action<int, string, RankData[]> callback)
        {
            readRankCallback = callback;
        }
        // 设置写排行榜回调
        public override void SetWriteRankCallback(Action<int, string, int> callback)
        {
            writeRankCallback = callback;
        }
        // 服务器内部错误通知
        public override void NotifyServerErrorClientRpc(int errorCode, string errorMessage)
        {
            StarkLiveSDK.PrintLog(TAG, $"NotifyServerErrorClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}", LogColor);
            Uninit();
            initCallback?.Invoke(errorCode, errorMessage, 0);
        }
        // 丢失礼物
        public override void NotifyLostGiftClientRpc(int errorCode, string errorMessage, ulong msgId, ulong roomId, string payloads,
            string msgType)
        {
            StarkLiveSDK.PrintLog(TAG, $"NotifyLostGiftClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}, payloads: {payloads}" +
                                       $", msgId: {msgId}, msgType: {msgType}, roomId: {roomId}", LogColor);
            if (CheckAndRemoveDuplicateQueryLostGiftMsgIds(msgId))
            {
                // 确认收到
                SaveLatestQueryLostGiftMsgId(msgId);
                Rpc.CallServerMethod(Consts.NOTIFY_LOST_GIFT_CONFIRM_SERVER_RPC, msgId);
                return;
            }
            if (errorCode == 0)
            {
                QueryNetDisconnectedFailedGiftResponse[] datas = JsonConvert.DeserializeObject<QueryNetDisconnectedFailedGiftResponse[]>(payloads);
                for (int i = 0; i < datas.Length; i++)
                {
                    if (CheckAndRemoveDuplicateEventMsg(datas[i].msgid))
                    {
                        StarkLiveSDK.PrintLog(TAG, $"NotifyLostGiftClientRpc pushId: {msgId} is duplicate, ignore", LogColor);
                    }
                    else
                    {
                        receiveGiftCallback?.Invoke(datas[i].GetGiftArray());
                    }
                }
            }
            else
            {
                StarkLiveSDK.PrintLog(TAG, $"NotifyLostGiftClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}", LogColor);
                Debug.LogError($"NotifyLostGiftClientRpc errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
            // 确认收到
            SaveLatestQueryLostGiftMsgId(msgId);
            Rpc.CallServerMethod(Consts.NOTIFY_LOST_GIFT_CONFIRM_SERVER_RPC, msgId);
        }
        // 点赞、评论、送礼事件推送
        public override void RoomEventClientRpc(string msgType, ulong pushId, string payload)
        {
            StarkLiveSDK.PrintLog(TAG, $"RoomEventClientRpc eventType: {msgType}, pushId: {pushId}, payload: {payload}", LogColor);
            LiveEventType type = Consts.LiveEventMsgStrDic[msgType];
            switch (type)
            {
                case LiveEventType.COMMENT: 
                    Comment[] comments = JsonConvert.DeserializeObject<Comment[]>(payload);
                    PlayerDataMgr.Instance.SavePlayerData(comments);
                    receiveCommentCallback?.Invoke(comments);
                    break;
                case LiveEventType.GIFT:
                    StarkLiveSDK.PrintLog(TAG, $"RoomEventClientRpc eventType: {msgType}, pushId: {pushId}, payload: {payload}", LogColor);
                    Gift[] gifts = JsonConvert.DeserializeObject<Gift[]>(payload);
                    PlayerDataMgr.Instance.SavePlayerData(gifts);
                    // 执行回调
                    receiveGiftCallback?.Invoke(gifts);
                    // 缓存当前event发送的进度，为了查询stark live server丢失的部分
                    UpdateGiftPushId(pushId);
                    // 缓存最近的event msgId，为补发去重
                    SaveLatestEventMsgId(pushId);
                    // 缓存confirm信息
                    NetworkHelper.Instance.CacheGiftEvent(GetRoomId(), pushId);
                    // 检查是否需要发送确认
                    NetworkHelper.Instance.CheckSendEventConfirm();
                    break;
                case LiveEventType.LIKE: 
                    Like[] likes = JsonConvert.DeserializeObject<Like[]>(payload);
                    PlayerDataMgr.Instance.SavePlayerData(likes);
                    receiveLikeCallback?.Invoke(likes);
                    break;
                default: break;
            }
        }
        
        public override void SetReceiveCommentCallback(Action<Comment[]> callback)
        {
            receiveCommentCallback = callback;
        }
        public override void SetReceiveGiftCallback(Action<Gift[]> callback)
        {
            receiveGiftCallback = callback;
        }
        public override void SetReceiveLikeCallback(Action<Like[]> callback)
        {
            receiveLikeCallback = callback;
        }
        public override void HeartbeatClientRpc(ulong tick)
        {
            // StarkLiveSDK.PrintLog(TAG, $"HeartbeatClientRpc tick: {tick}", LogColor);
            StarkLiveSDK.API.SendHeartbeatServerRpc(tick);
        }
        public override string GetRoomId()
        {
            return roomId;
        }

        public override string GetLostGiftsPushIds()
        {
            string json = "";
            foreach (var item in lostGiftsPushIdsDic)
            {
                json += $"\"{item.Key * Consts.LOST_GIFT_NUM_EACH_ENTITY + item.Value}\",";
            }

            if (!string.IsNullOrEmpty(json))
            {
                json = json.Substring(0, json.Length - 1);
            }

            return $"[{json}]";
        }

        private void UpdateGiftPushId(ulong pushId)
        {
            var entityId = pushId / Consts.LOST_GIFT_NUM_EACH_ENTITY;
            var idInEntity = pushId % Consts.LOST_GIFT_NUM_EACH_ENTITY;
            if (lostGiftsPushIdsDic.ContainsKey(entityId))
            {
                lostGiftsPushIdsDic[entityId] = idInEntity;
            }
            else
            {
                lostGiftsPushIdsDic.Add(entityId, idInEntity);
            }
        }

        // 缓存最近的event msgId，为补发去重
        private void SaveLatestEventMsgId(ulong msgId)
        {
            if (latestEventMsgIds.Count > Consts.CACHE_EVENT_PUSHID_MAX_COUNT)
                latestEventMsgIds.RemoveAt(0);
            latestEventMsgIds.Add(msgId);
        }

        private bool CheckAndRemoveDuplicateEventMsg(ulong id)
        {
            if (latestEventMsgIds.Contains(id))
            {
                latestEventMsgIds.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }
        // 缓存查询stark live server丢失的礼物的msgId，为去重
        private void SaveLatestQueryLostGiftMsgId(ulong pushId)
        {
            if (latestQueryLostGiftMsgIds.Count > Consts.CACHE_QUERY_LOST_GIFTS_ID_MAX_COUNT)
                latestQueryLostGiftMsgIds.RemoveAt(0);
            latestQueryLostGiftMsgIds.Add(pushId);
        }

        private bool CheckAndRemoveDuplicateQueryLostGiftMsgIds(ulong id)
        {
            if (latestQueryLostGiftMsgIds.Contains(id))
            {
                latestQueryLostGiftMsgIds.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}