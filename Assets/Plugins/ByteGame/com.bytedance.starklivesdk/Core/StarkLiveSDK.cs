using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using StarkNetwork;
using UnityEngine;

namespace StarkLive
{
    public class StarkLiveSDK
    {
        public static string IP = Consts.GAME_LIVE_SERVER;
        public static int Port = Consts.GAME_LIVE_PORT;
        private static Action<string> debugAction = null;
        private static readonly string TAG = "StarkLiveSDK";
        private static IStarkLiveAPI s_API;
        private static bool isLogEnable = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnableServicesInitializationAsync()
        {
            var networkHelper = NetworkHelper.Instance;
        }

        private static bool isClientMock = false;
        public static bool IsClientMock
        {
            private set { isClientMock = value; }
            get { return isClientMock; }
        }

        internal enum LaunchType
        {
            PC,
            PC_CLOUD,
            ANDROID_CLOUD,
            EDITOR
        }

        private static LaunchType launchType;

        internal static LaunchType GetLaunchType()
        {
            return launchType;
        }
        public static IStarkLiveAPI API
        {
            get
            {
                if (null == s_API)
                {
                    launchType = LaunchType.PC;
#if UNITY_EDITOR
                    launchType = LaunchType.EDITOR;
#if STARK_LIVE_SERVER_MOCK
                    s_API = new StarkLiveAPIPCImpl();
                    PrintLog(TAG, $"start from editor use StarkLiveAPIPCImpl");
#else
                    s_API = new StarkLiveAPIMockImpl();
                    IsClientMock = true;
                    PrintLog(TAG, $"start from editor use StarkLiveAPIMockImpl");

#endif
#else
                    if (IsStartFromPC())
                    {
                        if (!IsStartFromCloud())
                        {
                            // 1.pc伴侣端启动-exe
                            launchType = LaunchType.PC;
                            s_API = new StarkLiveAPIPCImpl();
                            StarkLog.Debug("StarkSDK", "start from pc exe");
                            PrintLog(TAG, $"API eventType: start from pc exe");
                        }
                        else
                        {
                            // 2.pc伴侣端启动-云游戏
                            launchType = LaunchType.PC_CLOUD;
                            s_API = new StarkLiveAPIPCCloudImpl();
                            StarkLog.Debug("StarkSDK", "start from pc cloud");
                            PrintLog(TAG, $"API eventType: start from pc cloud");
                        }
                    }
                    else
                    {
                        // 3.移动端 启动云游戏
                        launchType = LaunchType.ANDROID_CLOUD;
                        s_API = new StarkLiveAPIMobileCloudImpl();
                        StarkLog.Debug("StarkSDK", "start from mobile cloud");
                        PrintLog(TAG, $"API start from mobile cloud");
                    }
#endif
                }

                return s_API;
            }
        }

        public static void SetInitConfig(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        private static bool IsStartFromPC()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-token="))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStartFromCloud()
        {
            return false;
        }

        public static void PrintLog(string tag, string log, string color = "write")
        {
            if (isLogEnable)
            {
                string logStr = $"<color={color}>[{tag}]</color> {log}";
                StarkLog.Debug("StarkLiveSDK", logStr);
                debugAction?.Invoke(logStr);
            }
        }

        public static void SetDebugAction(Action<string> action)
        {
            debugAction = action;
        }

        public static void SetStarkLiveLogEnable(bool enable)
        {
            isLogEnable = enable;
            NetworkDebugger.ActivateNativeDebug();
            NetworkDebugger.SetDebuggerType(NetworkDebuggerType.CLIENT, isLogEnable);
            NetworkDebugger.SetDebuggerType(NetworkDebuggerType.NATIVE, isLogEnable);
        }

        private static string version = "1.0.27";
        public static string GetSDKVersion()
        {
            return version;
        }
    }
}