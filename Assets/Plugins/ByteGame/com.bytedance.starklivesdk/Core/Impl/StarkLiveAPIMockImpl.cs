using System;
using System.Text;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace StarkLive
{
    
    internal class StarkLiveAPIMockImpl : StarkLiveAPIImpl, IStarkLiveAPI
    {
        private static readonly string TAG = "StarkLiveAPIMockImpl";
        private static readonly string LogColor = "green";
        private string fakeToken = string.Empty;
        public StarkLiveAPIMockImpl(ILiveObj obj = null)
        {
            SetLiveCallbacks(obj);
        }
        
        private string GetStartToken()
        {
            return fakeToken;
        }

        public void SetStartToken(string token)
        {
            fakeToken = token;
        }
        
        ///////////////////////////////////////////////////////
        ///
        // 启动任务
        public new void StartLiveTask(LiveEventType msgType, Action<int, string, object> callback = null)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, "StarkLiveSDK need init", LogColor);
                return;
            }
            StarkLiveSDK.PrintLog(TAG, $"begin StartLiveTask......type: {Consts.LiveEventMsgStr[(int)msgType]}", LogColor);
            liveObj.SetStartTaskCallback(msgType, callback);
            var type = Utils.GetStartTaskRequestType(msgType);
            SendRequest(type, callback, GetStartToken(), "", Consts.LiveEventMsgStr[(int)msgType]);
            
            ExecuteMockClientRpc(type, msgType);
        }
        // 结束任务
        public new void StopLiveTask(LiveEventType msgType, Action<int, string, object> callback = null)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, null);
                return;
            }
            StarkLiveSDK.PrintLog(TAG, $"StopLiveTask roomId: {roomId}, type: {Consts.LiveEventMsgStr[(int)msgType]}", LogColor);
            liveObj.SetStopTaskCallback(msgType, callback);
            var type = Utils.GetStopTaskRequestType(msgType);
            SendRequest(type, callback, roomId, Consts.LiveEventMsgStr[(int)msgType]);

            ExecuteMockClientRpc(type, msgType);
        }
        // 查询任务状态
        public new void QueryStatus(LiveEventType msgType, Action<int, string, LiveTaskState> callback)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LiveTaskState.INVALID);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }

            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, LiveTaskState.INVALID);
                return;
            }
            StarkLiveSDK.PrintLog(TAG, $"QueryStatus roomId: {roomId}, type: {Consts.LiveEventMsgStr[(int)msgType]}", LogColor);
            liveObj.SetQueryTaskStatusCallback(msgType, callback);
            var type = Utils.GetQueryTaskStateRequestType(msgType);
            SendRequest(type, callback, roomId, Consts.LiveEventMsgStr[(int)msgType]);

            ExecuteMockClientRpc(type, msgType);
        }
        // 设置礼物置顶
        public new void SetGiftTop(string[] giftIds, Action<int, string, string[]> callback = null)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, null);
                return;
            }
            string json = JsonConvert.SerializeObject(giftIds);
            StarkLiveSDK.PrintLog(TAG, $"SetGiftTop roomId: {roomId}, json: {json}", LogColor);

            liveObj.SetSetGiftTopCallback(callback);
            SendRequest(RequestType.TOP_GIFT, callback, roomId, json);

            ExecuteMockClientRpc(RequestType.TOP_GIFT);
        }
        // 查询丢失的礼物
        public new void QueryFailedGift(int pageNum, int pageSize, Action<int, string, QueryFailedGiftPageData> callback = null)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, null);
                return;
            }
            StarkLiveSDK.PrintLog(TAG, $"QueryFailedGift roomId: {roomId}, type: {Consts.LiveEventMsgStr[(int)LiveEventType.GIFT]}, pageNum: {pageNum}, pageSize: {pageSize}", LogColor);
            liveObj.SetQueryFailedGiftCallback(callback);
            SendRequest(RequestType.QUERY_FAILED_GIFT, callback, roomId, Consts.LiveEventMsgStr[(int)LiveEventType.GIFT], pageNum, pageSize);

            ExecuteMockClientRpc(RequestType.QUERY_FAILED_GIFT); 
        }
        // 读排行榜
        public new void GetRankData(int rankId, RankTimeType rankTimeType, int pageIdx, int pageSize, Action<int, string, RankData[]> callback)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, null);
                return;
            }

            int reqId = 0;
            StarkLiveSDK.PrintLog(TAG, $"GetRankData reqId: {reqId}, rankId: {rankId}, rankTimeType: {rankTimeType}, pageIdx: {pageIdx}, pageSize: {pageSize}", LogColor);
            liveObj.SetReadRankCallback(callback);
            SendRequest(RequestType.READ_RANK, callback, reqId, rankId, (int)rankTimeType, pageIdx, pageSize);

            ExecuteMockClientRpc(RequestType.READ_RANK); 
        }
        public new void GetRankDataWithUserIds(int rankId, RankTimeType rankTimeType, string[] openIdList, Action<int, string, RankData[]> callback)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, null);
                return;
            }

            int reqId = 0;
            string json = JsonConvert.SerializeObject(openIdList);
            StarkLiveSDK.PrintLog(TAG, $"GetRankDataWithOpenId reqId: {reqId}, rankId: {rankId}, rankTimeType: {rankTimeType}, idsStr: {json}", LogColor);
            liveObj.SetReadRankCallback(callback);
            SendRequest(RequestType.READ_RANK_WITH_OPENID, callback, reqId, rankId, (int)rankTimeType, json);
            ExecuteMockClientRpc(RequestType.READ_RANK_WITH_OPENID); 
        }
        // 写排行榜
        public new void SetRankDataList(int rankId, List<WriteRankData> list, Action<int, string, int> callback)
        {
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, rankId);
                StarkLiveSDK.PrintLog(TAG, Consts.NOT_INITIALIZE_ERROR_MESSAGE, LogColor);
                return;
            }
            var roomId = GetRoomId();
            if (string.IsNullOrEmpty(roomId))
            {
                callback?.Invoke(Consts.NOT_STARTTASK_ERROR_CODE, Consts.NOT_STARTTASK_ERROR_MESSAGE, rankId);
                return;
            }

            int reqId = 0;
            string json = Utils.GetWriteRankDataJson(list);
            StarkLiveSDK.PrintLog(TAG, $"SetRankDataList reqId: {reqId}, json: {json}", LogColor);
            liveObj.SetWriteRankCallback(callback);
            SendRequest(RequestType.WRITE_RANK, callback, reqId, rankId, json);
            
            ExecuteMockClientRpc(RequestType.WRITE_RANK); 
        }
        
        
        
        public new bool IsInitialized()
        {
            return _init;
        }

        private bool _init = false;
        public new bool Init(string appId, Action<int, string, ulong> callback = null)
        {
            if (string.IsNullOrEmpty(appId))
            {
                callback?.Invoke(Consts.APPID_EMPTY_ERROR_CODE, Consts.APPID_EMPTY_ERROR_MESSAGE, 0);
                return false;
            }
            if (StarkLiveSDK.API.IsInitialized())
            {
                callback?.Invoke(Consts.ALREADY_INITIALIZE_ERROR_CODE, Consts.ALREADY_INITIALIZE_ERROR_MESSAGE, 0);
                return false;
            }
            StarkLiveSDK.PrintLog(TAG, $"Init mock", LogColor);
            liveObj.SetAppInfo(appId);
            liveObj.SetInitCallback(callback);
            NetworkHelper.Instance.RecordRequest(RequestType.INIT, callback);

            MockHelper.Instance.MockInitRpc(() =>
            {
                ulong uid = 123456;
                
                StarkLiveSDK.PrintLog(TAG, $"InitInfoClientRpc uid: {uid}", LogColor);
                NetworkHelper.Instance.ClearRecord(RequestType.INIT);
                NetworkHelper.Instance.RecordRequest(RequestType.SET_APP_INFO, callback);
                
                NetworkHelper.Instance.ClearRecord(RequestType.SET_APP_INFO);

                _init = true;
                callback?.Invoke(0, "", uid);
            });
            return true;
        }
        
        public new bool UnInit()
        {
            liveObj.Uninit();
            _init = false;
            // 关闭自动发消息
            MockHelper.Instance.MockRoomEventEnd(LiveEventType.GIFT);
            MockHelper.Instance.MockRoomEventEnd(LiveEventType.COMMENT);
            MockHelper.Instance.MockRoomEventEnd(LiveEventType.LIKE);
            StarkLiveSDK.PrintLog(TAG, $"UnInit mock", LogColor);
            return true;
        }

        public new void QueryNetDisconnectedFailedGifts()
        {
            // mock不考虑因断网丢礼物问题 liveObj.NotifyLostGiftClientRpc
            
            // var roomId = GetRoomId();
            // StarkLiveSDK.PrintLog(TAG, $"QueryNetDisconnectedFailedGifts roomId: {GetRoomId()}, pushIds: {liveObj.GetLostGiftsPushIds()}", LogColor);
            // if (string.IsNullOrEmpty(roomId))
            // {
            //     return;
            // }
            // Rpc.CallServerMethod(Consts.QUERY_LOST_GIFT_SERVER_RPC, GetRoomId(), liveObj.GetLostGiftsPushIds()); 
        }

        public new void ExecuteInitFailedCallback(int errorCode, string errorMessage)
        {
            liveObj.ExecuteInitFailedCallback(errorCode, errorMessage);
        }

        public new void NetResumeCheckTask(bool markStartTask)
        {
            liveObj.NetResumeCheckTask(GetStartToken(), "", markStartTask);
        }

        protected new void SendRequest<T>(RequestType type, Action<int, string, T> callback, params object[] args)
        {
            NetworkHelper.Instance.RecordRequest(type, callback);
            // Rpc.CallServerMethod(Consts.RpcCallDatas[type].ServerMethodName, args);
        }

        
        
        private void ExecuteMockClientRpc(RequestType type, LiveEventType msgType = LiveEventType.GIFT)
        {
            var data = MockDataMgr.Instance.MockTaskDatas[msgType];
            var msgTypeStr = Utils.GetMsgTypeStr(msgType);
            switch (type)
            {
                case RequestType.START_TASK_GIFT:
                case RequestType.START_TASK_LIKE:
                case RequestType.START_TASK_COMMENT:
                    data.TaskState = LiveTaskState.RUNNING;
                    liveObj.BeginRoomRspClientRpc(data.StartTaskErrorCode, data.StartTaskErrorMessage, MockDataMgr.MockRoomId, msgTypeStr);
                    // 开启自动发消息
                    MockHelper.Instance.MockRoomEventStart(msgType, () =>
                    {
                        liveObj.RoomEventClientRpc(msgTypeStr, MockDataMgr.Instance.GetRoomEventPushId(), MockDataMgr.Instance.GetMockRoomEventPayload(msgType));
                    });
                    break;
                case RequestType.STOP_TASK_GIFT:
                case RequestType.STOP_TASK_LIKE:
                case RequestType.STOP_TASK_COMMENT:
                    data.TaskState = LiveTaskState.NOT_START;
                    liveObj.EndRoomClientRpc(data.EndTaskErrorCode, data.EndTaskErrorMessage, JsonUtility.ToJson(data.EndTaskData), msgTypeStr);
                    // 关闭自动发消息
                    MockHelper.Instance.MockRoomEventEnd(msgType);
                    break;
                case RequestType.QUERY_TASK_STATE_GIFT:
                case RequestType.QUERY_TASK_STATE_LIKE:
                case RequestType.QUERY_TASK_STATE_COMMENT:
                    liveObj.QueryRoomStatusClientRpc(data.QueryStatusErrorCode, data.QueryStatusErrorMessage, JsonUtility.ToJson(data.QueryStatusData), msgTypeStr);
                    break;
                case RequestType.TOP_GIFT:
                    liveObj.TopGiftClientRpc(MockDataMgr.Instance.TopGiftErrorCode, MockDataMgr.Instance.TopGiftErrorMessage, 
                        JsonUtility.ToJson(MockDataMgr.Instance.TopGiftResponse));
                    break;
                case RequestType.QUERY_FAILED_GIFT:
                    liveObj.QueryEventOfPageClientRpc(MockDataMgr.Instance.QueryFailedGiftErrorCode, MockDataMgr.Instance.QueryFailedGiftErrorMessage, 
                        MockDataMgr.Instance.GetMockQueryFailedGiftResponse());
                    break;
                case RequestType.READ_RANK:
                    liveObj.GetRankRspClientRpc(MockDataMgr.Instance.ReadRankErrorCode,
                        MockDataMgr.Instance.ReadRankErrorMessage, 0, MockDataMgr.Instance.ReadRankRankId,
                        MockDataMgr.Instance.ReadRankTimeType, MockDataMgr.Instance.ReadRankPageIndex,
                        MockDataMgr.Instance.ReadRankPageSize, MockDataMgr.Instance.GetRankDataPayload());
                    break;
                case RequestType.WRITE_RANK:
                    liveObj.SetRankListRspClientRpc(MockDataMgr.Instance.WriteRankErrorCode,
                        MockDataMgr.Instance.WriteRankErrorMessage, 0, MockDataMgr.Instance.WriteRankRankId);
                    break;
                case RequestType.READ_RANK_WITH_OPENID:
                    liveObj.GetRankDataListRspClientRpc(MockDataMgr.Instance.ReadRankErrorCode,
                        MockDataMgr.Instance.ReadRankErrorMessage, 0, MockDataMgr.Instance.ReadRankRankId,
                        MockDataMgr.Instance.ReadRankTimeType, MockDataMgr.Instance.GetRankDataPayload(true));
                    break;
                default:
                    StarkLiveSDK.PrintLog(TAG, $"mock下未实现这种类型的请求：{Enum.GetName(typeof(RequestType), type)}", LogColor);
                    break;
            }
        }
    }
}