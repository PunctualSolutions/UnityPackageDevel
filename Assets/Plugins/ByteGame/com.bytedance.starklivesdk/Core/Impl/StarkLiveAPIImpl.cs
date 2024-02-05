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
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace StarkLive
{
    internal class StarkLiveAPIImpl : IStarkLiveAPI
    {
        private static readonly string TAG = "StarkLiveAPIImpl";
        private static readonly string LogColor = "green";
        public ILiveObj liveObj = null;
        public void SetLiveCallbacks(ILiveObj obj)
        {
            if (liveObj != null)
            {
                RpcRegister.UnRegisterMethodObj(liveObj);
            }
            RegisterLiveObj(obj);
        }
        private void RegisterLiveObj(ILiveObj obj)
        {
            liveObj = obj == null ? new LiveObj() : obj;
            RpcRegister.RegisterMethodObj(liveObj);
        }

        // 启动任务
        public void StartLiveTask(LiveEventType msgType, Action<int, string, object> callback = null)
        {
            StarkLiveSDK.PrintLog(TAG, $"StartLiveTask do nothing", LogColor);
        }
        // 结束任务
        public void StopLiveTask(LiveEventType msgType, Action<int, string, object> callback = null)
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
        }
        // 查询任务状态
        public void QueryStatus(LiveEventType msgType, Action<int, string, LiveTaskState> callback)
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
        }
        // 设置礼物置顶
        public void SetGiftTop(string[] giftIds, Action<int, string, string[]> callback = null)
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
        }
        // 查询丢失的礼物
        public void QueryFailedGift(int pageNum, int pageSize, Action<int, string, QueryFailedGiftPageData> callback = null)
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
        }
        // 读排行榜
        public void GetRankData(int rankId, RankTimeType rankTimeType, int pageIdx, int pageSize, Action<int, string, RankData[]> callback)
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

            int reqId = Utils.GetRankReqId();
            StarkLiveSDK.PrintLog(TAG, $"GetRankData reqId: {reqId}, rankId: {rankId}, rankTimeType: {rankTimeType}, pageIdx: {pageIdx}, pageSize: {pageSize}", LogColor);
            liveObj.SetReadRankCallback(callback);
            SendRequest(RequestType.READ_RANK, callback, reqId, rankId, (int)rankTimeType, pageIdx, pageSize);
        }
        // 根据openidlist读排行榜
        public void GetRankDataWithUserIds(int rankId, RankTimeType rankTimeType, string[] openIdList, Action<int, string, RankData[]> callback)
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

            int reqId = Utils.GetRankReqId();
            string json = JsonConvert.SerializeObject(openIdList);
            StarkLiveSDK.PrintLog(TAG, $"GetRankDataWithOpenId reqId: {reqId}, rankId: {rankId}, rankTimeType: {rankTimeType}, openIdList: {json}", LogColor);
            liveObj.SetReadRankCallback(callback);
            SendRequest(RequestType.READ_RANK_WITH_OPENID, callback, reqId, rankId, (int)rankTimeType, json);
        }
        // 写排行榜
        public void SetRankDataList(int rankId, List<WriteRankData> list, Action<int, string, int> callback)
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

            int reqId = Utils.GetRankReqId();
            string json = Utils.GetWriteRankDataJson(list);
            StarkLiveSDK.PrintLog(TAG, $"SetRankDataList reqId: {reqId}, json: {json}", LogColor);
            liveObj.SetWriteRankCallback(callback);
            SendRequest(RequestType.WRITE_RANK, callback, reqId, rankId, json);
        }
        
        public string GetRoomId()
        {
            return liveObj.GetRoomId();
        }
        
        public void SendHeartbeatServerRpc(ulong tick)
        {
            Rpc.CallServerMethod(Consts.HEARTBEAT_SERVER_RPC, tick);
        }

        public bool IsInitialized()
        {
            return NetworkController.NetworkState == ClientState.RUNNING && liveObj.GetInitState();
        }
        
        public bool Init(string appId, Action<int, string, ulong> callback = null)
        {
            StarkLiveReport.ReportInitStart(appId);
            if (string.IsNullOrEmpty(appId))
            {
                callback?.Invoke(Consts.APPID_EMPTY_ERROR_CODE, Consts.APPID_EMPTY_ERROR_MESSAGE, 0);
                StarkLiveReport.ReportInitResult(appId, "", Consts.APPID_EMPTY_ERROR_CODE, Consts.APPID_EMPTY_ERROR_MESSAGE, ErrorSourceType.CLIENT);
                return false;
            }
            if (StarkLiveSDK.API.IsInitialized())
            { // 已初始化
                callback?.Invoke(Consts.ALREADY_INITIALIZE_ERROR_CODE, Consts.ALREADY_INITIALIZE_ERROR_MESSAGE, 0);
                StarkLiveReport.ReportInitResult(appId, "", Consts.ALREADY_INITIALIZE_ERROR_CODE, Consts.ALREADY_INITIALIZE_ERROR_MESSAGE, ErrorSourceType.CLIENT);
                return false;
            }
            if (NetworkHelper.Instance.AlreadyRequested(RequestType.INIT) || NetworkHelper.Instance.AlreadyRequested(RequestType.SET_APP_INFO))
            { // 未初始化并且已有初始化请求
                callback?.Invoke(Consts.ALREADY_REQUEST_INIT_ERROR_CODE, Consts.ALREADY_REQUEST_INIT_ERROR_MESSAGE, 0);
                StarkLiveReport.ReportInitResult(appId, "", Consts.ALREADY_REQUEST_INIT_ERROR_CODE, Consts.ALREADY_REQUEST_INIT_ERROR_MESSAGE, ErrorSourceType.CLIENT);
                return false;
            }

            Action<int, string, ulong> callbackWithReport = (errorCode, errorMsg, uid) =>
            {
                callback(errorCode, errorMsg, uid);
                StarkLiveReport.ReportInitResult(appId, uid.ToString(), errorCode, errorMsg, ErrorSourceType.SERVER);
            };
            StarkLiveSDK.PrintLog(TAG, $"Init ip: {StarkLiveSDK.IP}, port: {StarkLiveSDK.Port}", LogColor);
            liveObj.SetAppInfo(appId);
            liveObj.SetInitCallback(callbackWithReport);
            NetworkHelper.Instance.RecordRequest(RequestType.INIT, callbackWithReport);
            
            string token = $"{appId}_{SystemInfo.deviceUniqueIdentifier}";
            StarkLiveSDK.PrintLog(TAG, $"token: {token}", LogColor);
            bool r = NetworkController.ConnectUsingSettings(new ConnectOption().SetToken(token).SetServer(StarkLiveSDK.IP, StarkLiveSDK.Port).SetIsFake(true));
            Rpc.RegisterCustomSimpleMethod(Consts.INIT_INFO_CLIENT_RPC, new string[]{"uint64"});
            Rpc.RegisterCustomSimpleMethod(Consts.APP_INFO_CONFIRM_CLIENT_RPC, new string[]{"uint64", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.BEGIN_ROOM_RSP_CLIENT_RPC, new string[]{"int", "string", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.ROOM_EVENT_CLIENT_RPC, new string[]{"string", "uint64", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.HEARTBEAT_CLIENT_RPC, new string[]{"uint64"});
            Rpc.RegisterCustomSimpleMethod(Consts.END_ROOM_CLIENT_RPC, new string[]{"int", "string", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.QUERY_ROOM_STATUS_CLIENT_RPC, new string[]{"int", "string", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.QUERY_EVENT_OF_PAGE_CLIENT_RPC, new string[]{"int", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.TOP_GIFT_CLIENT_RPC, new string[]{"int", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.NOTIFY_LOST_GIFT_CLIENT_RPC, new string[]{"int", "string", "uint64", "uint64", "string", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.SET_RANK_LIST_RSP_CLIENT_RPC, new string[]{"int", "string", "int", "int"});
            Rpc.RegisterCustomSimpleMethod(Consts.GET_RANK_RSP_CLIENT_RPC, new string[]{"int", "string", "int", "int", "int", "int", "int", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.NOTIFY_SERVER_ERROR_CLIENT_RPC, new string[]{"int", "string"});
            Rpc.RegisterCustomSimpleMethod(Consts.GET_RANK_DATA_LIST_RSP_CLIENT_RPC, new string[]{"int", "string", "int", "int", "int", "string"});
            return r;
        }
        
        public bool UnInit()
        {
            liveObj.Uninit();
            NetworkHelper.Instance.ClearRequestRecords();
            return NetworkController.CloseConnection();
        }

        public void SetReceiveCommentCallback(Action<Comment[]> callback)
        {
            liveObj.SetReceiveCommentCallback(callback);
        }

        public void SetReceiveGiftCallback(Action<Gift[]> callback)
        {
            liveObj.SetReceiveGiftCallback(callback);
        }

        public void SetReceiveLikeCallback(Action<Like[]> callback)
        {
            liveObj.SetReceiveLikeCallback(callback);
        }
        

        public void QueryNetDisconnectedFailedGifts()
        {
            var roomId = GetRoomId();
            StarkLiveSDK.PrintLog(TAG, $"QueryNetDisconnectedFailedGifts roomId: {GetRoomId()}, pushIds: {liveObj.GetLostGiftsPushIds()}", LogColor);
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }
            Rpc.CallServerMethod(Consts.QUERY_LOST_GIFT_SERVER_RPC, GetRoomId(), liveObj.GetLostGiftsPushIds());
        }

        public void ExecuteInitFailedCallback(int errorCode, string errorMessage)
        {
            liveObj.ExecuteInitFailedCallback(errorCode, errorMessage);
        }

        public void NetResumeCheckTask(bool markStartTask)
        {
        }
        
        protected void SendRequest<T>(RequestType type, Action<int, string, T> callback, params object[] args)
        {
            // 记录请求
            NetworkHelper.Instance.RecordRequest(type, callback);
            // 发出请求
            Rpc.CallServerMethod(Consts.RpcCallDatas[type].ServerMethodName, args);
        }
    }
}