using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNBridgeLib.LitJson;
using System.Linq;

namespace StarkLive
{
    internal class StarkLiveReport
    {
        internal static TeaSdk s_StarkTea = new TeaSdk();
        internal static bool bInit = false;
        public static void CheckInit()
        {
            if (!bInit)
            {
                bInit = true;
                s_StarkTea.Init(4243, SystemInfo.deviceUniqueIdentifier, "StarkLive", Application.version);
            }
        }
        
        private static string reportAppId = String.Empty;
        private static string reportUid = String.Empty;
        // 初始化开始
        public static async void ReportInitStart(string appId)
        {
            CheckInit();
            StarkLog.Debug("StarkLiveReport", $"ReportInitStart appId: {appId}, enging: {Application.unityVersion}, launchType: {StarkLiveSDK.GetLaunchType()}");
            JsonData tmpDic = new JsonData();
            tmpDic["appid"] = appId;
            tmpDic["engine"] = "Unity " + Application.unityVersion;
            tmpDic["launch_type"] = StarkLiveSDK.GetLaunchType().ToString();
            tmpDic["sdk_version"] = StarkLiveSDK.GetSDKVersion();

            await s_StarkTea.Collect("sl_init_start", tmpDic.ToJson(), Debug.isDebugBuild ? new StarkTeaDebugProvider() : null);
        }

        // 初始化结果
        public static async void ReportInitResult(string appId, string uid, int errCode, string errMsg, ErrorSourceType errorSourceType)
        {
            CheckInit();
            StarkLog.Debug("StarkLiveReport", $"ReportInitResult appId: {appId}, uid: {uid}, errCode: {errCode}, errMsg: {errMsg}, enging: {Application.unityVersion}, launchType: {StarkLiveSDK.GetLaunchType()}");
            JsonData tmpDic = new JsonData();
            tmpDic["appid"] = appId;
            tmpDic["uid"] = uid;
            tmpDic["error_code"] = errCode;
            tmpDic["error_msg"] = errMsg;
            tmpDic["engine"] = "Unity " + Application.unityVersion;
            tmpDic["launch_type"] = StarkLiveSDK.GetLaunchType().ToString();
            tmpDic["sdk_version"] = StarkLiveSDK.GetSDKVersion();
            tmpDic["error_type"] = Enum.GetName(typeof(ErrorSourceType), errorSourceType);

            await s_StarkTea.Collect("sl_init_result", tmpDic.ToJson(), Debug.isDebugBuild ? new StarkTeaDebugProvider() : null);
        }
        // 开启任务开始
        public static async void ReportStartTask(string appId, string uid, string msgType)
        {
            CheckInit();
            StarkLog.Debug("StarkLiveReport", $"ReportStartTask appId: {appId}, uid: {uid}, msgType: {msgType}, enging: {Application.unityVersion}, launchType: {StarkLiveSDK.GetLaunchType()}");
            JsonData tmpDic = new JsonData();
            tmpDic["appid"] = string.IsNullOrEmpty(appId) ? reportAppId : appId;
            tmpDic["uid"] = string.IsNullOrEmpty(uid) ? reportAppId : uid;
            tmpDic["msg_type"] = msgType; 
            tmpDic["engine"] = "Unity " + Application.unityVersion;
            tmpDic["launch_type"] = StarkLiveSDK.GetLaunchType().ToString();
            tmpDic["sdk_version"] = StarkLiveSDK.GetSDKVersion();

            await s_StarkTea.Collect("sl_task_start", tmpDic.ToJson(), Debug.isDebugBuild ? new StarkTeaDebugProvider() : null);
        }
        // 开启任务结果
        public static async void ReportStartTaskResult(string appId, string uid, string msgType, string roomId, int errCode, string errMsg, ErrorSourceType errorSourceType)
        {
            CheckInit();
            StarkLog.Debug("StarkLiveReport", $"ReportStartTaskResult appId: {appId}, uid: {uid}, msgType: {msgType}, roomId: {roomId}, errCode: {errCode}, errMsg: {errMsg}, " +
                                              $"engine: {Application.unityVersion}, launch_type: {StarkLiveSDK.GetLaunchType()}");
            JsonData tmpDic = new JsonData();
            tmpDic["appid"] = appId;
            tmpDic["uid"] = uid;
            tmpDic["msg_type"] = msgType; 
            tmpDic["roomid"] = roomId; 
            tmpDic["error_code"] = errCode;
            tmpDic["error_msg"] = errMsg;
            tmpDic["engine"] = "Unity " + Application.unityVersion;
            tmpDic["launch_type"] = StarkLiveSDK.GetLaunchType().ToString();
            tmpDic["sdk_version"] = StarkLiveSDK.GetSDKVersion();
            tmpDic["error_type"] = Enum.GetName(typeof(ErrorSourceType), errorSourceType);

            await s_StarkTea.Collect("sl_task_start_result", tmpDic.ToJson(), Debug.isDebugBuild ? new StarkTeaDebugProvider() : null);
        }

        private class StarkTeaDebugProvider : ITeaDataProvider
        {
            public string TestDeviceId => "StarkTeaDebugProvider";
            private Dictionary<string, object> m_CustomValues = new Dictionary<string, object>();
            public Dictionary<string, object> CustomValues => m_CustomValues;
        }
    }
}
