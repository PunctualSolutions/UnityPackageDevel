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
    internal class StarkLiveAPIPCImpl : StarkLiveAPIImpl, IStarkLiveAPI
    {
        private static readonly string TAG = "StarkLiveAPIPCImpl";
        private static readonly string LogColor = "green";
        public StarkLiveAPIPCImpl(ILiveObj obj = null)
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
            string commandline = Environment.CommandLine;
            string[] args = Environment.GetCommandLineArgs();
            StarkLiveSDK.PrintLog(TAG, $"GetStartToken CommandLine: {commandline}", LogColor);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(Consts.PC_STARTTOKEN_KEYWORDS))
                {
                    var startToken = args[i].Substring(args[i].IndexOf(Consts.PC_STARTTOKEN_KEYWORDS) + Consts.PC_STARTTOKEN_KEYWORDS.Length);
                    return startToken;
                }
            }
            return String.Empty;
        }
        
        public new void NetResumeCheckTask(bool markStartTask)
        {
            liveObj.NetResumeCheckTask(GetStartToken(), "", markStartTask);
        }
    }
}