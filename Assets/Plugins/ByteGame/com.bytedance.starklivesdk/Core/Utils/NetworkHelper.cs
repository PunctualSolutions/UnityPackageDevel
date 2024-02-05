using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkLive
{
    public class RequestRecord
    {
        public RequestType RequestType;
        public Action<int, string> Callback;
        
        public RequestRecord(RequestType requestType, Action<int, string> callback)
        {
            RequestType = requestType;
            Callback = callback;
        }

        public void CallbackInvoke(int errorCode = 0, string errorMessage = "")
        {
            if (errorCode == 0)
            {
                Callback(Consts.NET_DISCONNECTED_ERRORCODE, Consts.NET_DISCONNECTED_ERRORMESSAGE);
            }
            else
            {
                Callback(errorCode, errorMessage);
            }
        }
    }
    
    public class NetworkHelper : StarkSingletonBehaviour<NetworkHelper>, IConnectionCallbacks
    {

        class EventConfirmInfo
        {
            public uint HashId;
            public ulong RoomId;
            public ulong PushId;
        }
        private static readonly string LogColor = "cyan";
        private static readonly string TAG = "NetworkHelper";
        private Dictionary<RequestType, RequestRecord> requestTimerDic = new Dictionary<RequestType, RequestRecord>();
        private long confirmGiftEventTimestamp = 0;
        private Dictionary<ulong, EventConfirmInfo> eventConfirmInfosDic = new Dictionary<ulong, EventConfirmInfo>();
        private IEnumerator confirmEventChecker = null;
        private void Awake()
        {
            NetworkDebugger.SetDebuggerType(NetworkDebuggerType.CLIENT, true);
            NetworkDebugger.SetDebuggerType(NetworkDebuggerType.NATIVE, true);
            NetworkDebugger.ActivateNativeDebug();
            NetworkController.AddCallbackTarget(this);
        }
        private void OnDestroy()
        {
            NetworkController.RemoveCallbackTarget(this);
        }
        private void PrintLog(string log)
        {
            StarkLiveSDK.PrintLog(TAG, log, LogColor);
        }
        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="result"></param>
        public void OnConnected(SerializedConnectResult result)
        {
            PrintLog($"OnConnected......resumeMod: {result.resumeMod}");
            if (result.resumeMod == ConnectResumeMod.ReConnect)
            {
                // 查询因为断连产生的丢失的礼物，并进行event的补发 
                // StarkLiveSDK.API.QueryNetDisconnectedFailedGifts(); // 这里不需要了，因为断线重连也会触发task start
                // task状态检查
                StarkLiveSDK.API.NetResumeCheckTask(true);
            }
            else if (result.resumeMod == ConnectResumeMod.FirstTime)
            {
                if (confirmEventChecker != null)
                {
                    StopCoroutine(confirmEventChecker);
                }

                confirmEventChecker = ConfirmEventChecker();
                StartCoroutine(confirmEventChecker);
            }
        }

        /// <summary>
        /// 连接网络失败
        /// </summary>
        /// <param name="result"></param>
        public void OnConnectFailed(SerializedConnectFailedResult result)
        {
            PrintLog($"OnConnectFailed");
            if (confirmEventChecker != null)
            {
                StopCoroutine(confirmEventChecker);
            }

            StopInitTimer();
            StarkLiveSDK.API.UnInit();
            StarkLiveSDK.API.ExecuteInitFailedCallback(result.errorInfo.errCode, result.errorInfo.errMsg);
        }

        /// <summary>
        /// 网络临时断连，期间 Native 会自动重试
        /// </summary>
        public void OnDisconnected()
        {
            PrintLog($"OnDisconnected");
            // 当前所有请求按失败返回并清除请求
            OnDisconnectedHandleRequestRecords();
        }

        /// <summary>
        /// 和服务的通信完全断开
        /// </summary>
        /// <param name="msg"></param>
        public void OnConnectClosed(SerializedConnectCloseMessage msg)
        {
            PrintLog($"OnConnectClosed");
            if (confirmEventChecker != null)
            {
                StopCoroutine(confirmEventChecker);
            }
            eventConfirmInfosDic.Clear();
            StopInitTimer();
            if (msg.closeReason != ConnectCloseReason.Initiative)
            {
                StarkLiveSDK.API.UnInit();
                StarkLiveSDK.API.ExecuteInitFailedCallback(msg.errorInfo.errCode, msg.errorInfo.errMsg);
            }
        }

        /// <summary>
        /// 主动向服务端查询当前自己的状态后，得到服务端返回结果
        /// </summary>
        /// <param name="info">当前自身状态，主要是当前所在房间信息列表</param>
        public void OnPlayerInfoGot(SerializedPlayerCurrentInfo info)
        {
            
        }
        
        // 记录请求
        private List<RequestRecord> requestRecords = new List<RequestRecord>();
        public void RecordRequest<T>(RequestType type, Action<int, string, T> callback)
        {
            var record = new RequestRecord(type, (errorCode, errorMsg) =>
            {
                callback(errorCode, errorMsg, default(T));
            });
            requestRecords.Add(record);
            if (type == RequestType.INIT)
            {
                StartInitTimer();
            }
        }

        // 发生断线重连时，按失败处理
        private void OnDisconnectedHandleRequestRecords()
        {
            while (requestRecords.Count > 0)
            {
                var item = requestRecords[0];
                requestRecords.RemoveAt(0);
                item.CallbackInvoke();
            }
        }

        internal void ClearRequestRecords()
        {
            requestRecords.Clear();
        }
        // 收到回包时清除record
        public void ClearRecord(RequestType type)
        {
            for (int i = 0; i < requestRecords.Count; i++)
            {
                if (requestRecords[i].RequestType == type)
                {
                    requestRecords.RemoveAt(i);
                    break;
                }
            }
        }

        public bool AlreadyRequested(RequestType type)
        {
            for (int i = 0; i < requestRecords.Count; i++)
            {
                if (requestRecords[i].RequestType == type)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator initTimer;
        private void StartInitTimer()
        {
            if (initTimer != null)
            {
                StopCoroutine(initTimer);
            }

            initTimer = InitTimer();
            StartCoroutine(initTimer);
        }

        private void StopInitTimer()
        {
            if (initTimer != null)
            {
                StopCoroutine(initTimer);
                initTimer = null;
            }
        }
        IEnumerator InitTimer()
        {
            yield return new WaitForSeconds(Consts.INIT_WAIT_SECONDS);
            for (int i = 0; i < requestRecords.Count; i++)
            {
                if (requestRecords[i].RequestType == RequestType.INIT || requestRecords[i].RequestType == RequestType.SET_APP_INFO)
                {
                    if (!StarkLiveSDK.API.IsInitialized())
                    { // 发出了初始化请求，但没有收到初始化回包
                        StarkLiveSDK.API.UnInit();
                        requestRecords[i].CallbackInvoke(Consts.INIT_TIMEOUT_ERROR_CODE, Consts.INIT_TIMEOUT_ERROR_MESSAGE);
                        requestRecords.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        // confirm event检查:1s检查一次
        IEnumerator ConfirmEventChecker()
        {
            while (true)
            {
                CheckSendEventConfirm();
                yield return new WaitForSeconds(Consts.CONFIRM_EVENT_SECONDS);
            }
        }

        public void CheckSendEventConfirm()
        {
            if (CanConfirmEvent())
            {
                confirmGiftEventTimestamp = DateTime.UtcNow.Ticks;
                // 发送确认
                foreach (var item in eventConfirmInfosDic)
                {
                    var value = item.Value;
                    PrintLog($"发送event确认 hashId: {value.HashId}, targetId: {item.Key}, roomId: {value.RoomId}, pushId: {value.PushId}");
                    Rpc.CallTargetServerMethod(Consts.PUSH_CONFIRM_SERVER_RPC, value.HashId, item.Key, value.RoomId, value.PushId);
                }
                eventConfirmInfosDic.Clear();
            }
        }
        private bool CanConfirmEvent()
        {
            return StarkLiveSDK.API.IsInitialized()
                   && eventConfirmInfosDic.Count > 0
                   && (DateTime.UtcNow.Ticks - confirmGiftEventTimestamp) >= Consts.CONFIRM_EVENT_SECONDS * 10000000;
        }
        public void CacheGiftEvent(string roomId, ulong pushId)
        {
            if (StarkLiveSDK.IsClientMock)
                return;
            var context = Rpc.GetRpcCallContext();
            if (eventConfirmInfosDic.ContainsKey(context.src_id))
            {
                if (eventConfirmInfosDic[context.src_id].PushId < pushId)
                {
                    eventConfirmInfosDic[context.src_id].HashId = context.src_hash;
                    eventConfirmInfosDic[context.src_id].RoomId = Convert.ToUInt64(roomId);
                    eventConfirmInfosDic[context.src_id].PushId = pushId;
                }
            }
            else
            {
                eventConfirmInfosDic.Add(context.src_id, new EventConfirmInfo()
                {
                    HashId = context.src_hash,
                    RoomId = Convert.ToUInt64(roomId),
                    PushId = pushId
                });
            }
        }

        public void PrintRequestRecords()
        {
            string log = "PrintRequestRecords\n";
            for (int i = 0; i < requestRecords.Count; i++)
            {
                log += $"[{i}] {Enum.GetName(typeof(RequestType), requestRecords[i].RequestType)}\n";
            }
            
            PrintLog($"{log}");
        }
    }
}