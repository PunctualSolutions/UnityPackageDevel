// Copyright @ www.bytedance.com
// Time: 2022-08-30
// Author: wuwenbin@bytedance.com
// Description: 用于控制网络模块内部的日志显示和隐藏

using System;
using System.Runtime.InteropServices;
using AOT;
using StarkMatchmaking;
using UnityEngine;

namespace StarkNetwork
{
    public enum NetworkDebuggerType
    {
        CLIENT,
        NATIVE,
    }

    public enum NetworkDebuggerLevel
    {
        INFO,
        WARNING,
        ERROR
    }
    
    public class NetworkDebugger
    {
        private static NetworkDebugger _instance = new NetworkDebugger();

        public static void Log(string msg, NetworkDebuggerType debuggerType = NetworkDebuggerType.CLIENT)
        {
            _instance.LogFunc(msg, debuggerType, NetworkDebuggerLevel.INFO);
        }
        
        public static void Warning(string msg, NetworkDebuggerType debuggerType = NetworkDebuggerType.CLIENT)
        {
            _instance.LogFunc(msg, debuggerType, NetworkDebuggerLevel.WARNING);
        }
        
        public static void Error(string msg, NetworkDebuggerType debuggerType = NetworkDebuggerType.CLIENT)
        {
            _instance.LogFunc(msg, debuggerType, NetworkDebuggerLevel.ERROR);
        }
        
        public static void SetDebuggerType(NetworkDebuggerType type, bool isAllowed)
        {
            _instance.SetDebuggerTypeFunc(type, isAllowed);
        }
        
        public static void SetDebuggerLevel(NetworkDebuggerLevel level, bool isAllowed)
        {
            _instance.SetDebugLevelFunc(level, isAllowed);
        }

        /// <summary>
        /// Log 输出时对应类型输出的颜色
        /// </summary>
        private string[] _typeColors = new[] { "#00FF00", "#FFFF00" };
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugDelegate(int level, IntPtr strPtr);
        
        [MonoPInvokeCallback(typeof(DebugDelegate))]
        static void NativeLog(int level, IntPtr nativeLog)
        {
            try
            {
                var logCont = Marshal.PtrToStringAnsi(nativeLog);
                Log($"[Level {level}] {logCont}", NetworkDebuggerType.NATIVE);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
          
        }
        
        
        public static void ActivateNativeDebug()
        {
            DebugDelegate callback_delegate = NativeLog;
            IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
            stark_matchmaking.SetLogDelegate(new SWIGTYPE_p_f_int_p_q_const__char__void(intptr_delegate, true));
        }
        
        private NetworkDebugger()
        {
            
        }

        private int _allowedType = 0;
        private int _allowedLevel = 0b111;

        private void LogFunc(string msg, NetworkDebuggerType type, NetworkDebuggerLevel level)
        {
            if ((_allowedType & (1 << (int)type)) == 0 || (_allowedLevel & (1 << (int)level)) == 0)
            {
                return;
            }
            switch (level)
            {
                case NetworkDebuggerLevel.INFO:
                    Debug.Log($"<color=cyan>[Stark Network]</color>-<color={_typeColors[(int)type]}>[{type.ToString()}]</color>: {msg}");
                    break;
                case NetworkDebuggerLevel.WARNING:
                    Debug.LogWarning($"<color=cyan>[Stark Network]</color>-<color={_typeColors[(int)type]}>[{type.ToString()}]</color>: {msg}");
                    break;
                case NetworkDebuggerLevel.ERROR:
                    Debug.LogError($"<color=cyan>[Stark Network]</color>-<color={_typeColors[(int)type]}>[{type.ToString()}]</color>: {msg}");
                    break;
            }
        }

        private void SetDebuggerTypeFunc(NetworkDebuggerType type, bool isAllowed)
        {
            if (isAllowed)
            {
                _allowedType |= 1 << (int)type;
            }
            else
            {
                _allowedType &= ~(1 << (int)type);
            }
        }

        private void SetDebugLevelFunc(NetworkDebuggerLevel level, bool isAllowed)
        {
            if (isAllowed)
            {
                _allowedLevel |= 1 << (int)level;
            }
            else
            {
                _allowedLevel &= ~(1 << (int)level);
            }
        }

    }
}