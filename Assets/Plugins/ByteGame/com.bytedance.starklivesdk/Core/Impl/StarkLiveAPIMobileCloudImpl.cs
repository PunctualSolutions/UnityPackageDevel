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
using Random = UnityEngine.Random;

namespace StarkLive
{
    internal class StarkLiveAPIMobileCloudImpl : StarkLiveAPIImpl, IStarkLiveAPI
    {
        private static readonly string TAG = "StarkLiveAPIMobileCloudImpl";
        private static readonly string LogColor = "green";
        public StarkLiveAPIMobileCloudImpl(ILiveObj obj = null)
        {
            SetLiveCallbacks(obj);
        }
        

        public new void StartLiveTask(LiveEventType msgType, Action<int, string, object> callback = null)
        {
            StarkLiveReport.ReportStartTask(liveObj.GetAppId(), liveObj.GetUid(), Utils.GetMsgTypeStr(msgType));
            if (!IsInitialized())
            {
                callback?.Invoke(Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, null);
                StarkLiveReport.ReportStartTaskResult(liveObj.GetAppId(), liveObj.GetUid(), Utils.GetMsgTypeStr(msgType), GetRoomId(), Consts.NOT_INITIALIZE_ERROR_CODE, Consts.NOT_INITIALIZE_ERROR_MESSAGE, ErrorSourceType.CLIENT);
                return;
            }
            StarkLiveSDK.PrintLog(TAG, $"begin StartLiveTask......type: {Consts.LiveEventMsgStr[(int)msgType]}", LogColor);
            Action<int, string, object> callbackWithReport = (errorCode, errMsg, o) =>
            {
                callback?.Invoke(errorCode, errMsg, o);
                StarkLiveReport.ReportStartTaskResult(liveObj.GetAppId(), liveObj.GetUid(), Utils.GetMsgTypeStr(msgType), GetRoomId(), errorCode, errMsg, ErrorSourceType.SERVER);
            };
            liveObj.SetStartTaskCallback(msgType, callbackWithReport);
            var type = Utils.GetStartTaskRequestType(msgType);
            SendRequest(type, callbackWithReport, GetStartToken(), "", Consts.LiveEventMsgStr[(int)msgType]);
        }


        private string GetStartToken()
        {
            return String.Empty;
        }
        public new void NetResumeCheckTask(bool markStartTask)
        {
            liveObj.NetResumeCheckTask(GetStartToken(), "", markStartTask);
        }
    }
}