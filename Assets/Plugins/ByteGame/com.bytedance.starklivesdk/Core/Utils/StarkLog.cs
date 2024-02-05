using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace StarkLive
{
    public class StarkLog : StarkSingletonBehaviour<StarkLog>
    {
        private enum LogType
        {
            DEBUG = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            EXCEPTION = 4,
            ASSERT = 5,
            TOAST = 6
        }

        [Flags]
        private enum PrintTarget
        {
            None = 0,
            UI = 1,
            FILE = 2,
            WWW = 4
        }

        private static PrintTarget m_PrintTarget = PrintTarget.None;
        private static int m_Level = 0;

        // 等级控制
        public static int LogLevel
        {
            get => m_Level;
            set
            {
                if (value <= (int)LogType.ERROR)
                {
                    // UNBridgeLib.LogUtils.DEBUG = true;
                }
                else
                {
                    // UNBridgeLib.LogUtils.DEBUG = false;
                }

                m_Level = value;
            }
        }

        private void Init() {
            InitLogStacktrace();

            LogLevel = 0;                                   // 默认等级为0，所有log全部显示

            // SwitchPrintTarget(PrintTarget.UI, false);       // 默认关闭打印至屏幕，待开发
            SwitchPrintTarget(PrintTarget.FILE, true);     // 默认关闭打印至文件
            // SwitchPrintTarget(PrintTarget.WWW, false);      // 默认关闭日志上传，待开发
            UnityEngine.Debug.Log("StarkLog:Init");
        }

        private void Awake()
        {
            UnityEngine.Debug.Log("StarkLog:Awake");
            Init();
        }

        private void OnEnable()
        {
            StarkPrintFile.BeginRecord();
        }

        private void OnDisable()
        {
            StarkPrintFile.EndRecord();
        }

        private static void InitLogStacktrace()
        {
            UnityEngine.Application.SetStackTraceLogType(UnityEngine.LogType.Log, UnityEngine.StackTraceLogType.ScriptOnly);
            UnityEngine.Application.SetStackTraceLogType(UnityEngine.LogType.Warning, UnityEngine.StackTraceLogType.ScriptOnly);
            UnityEngine.Application.SetStackTraceLogType(UnityEngine.LogType.Assert, UnityEngine.StackTraceLogType.ScriptOnly);
            UnityEngine.Application.SetStackTraceLogType(UnityEngine.LogType.Error, UnityEngine.StackTraceLogType.ScriptOnly);
            UnityEngine.Application.SetStackTraceLogType(UnityEngine.LogType.Exception, UnityEngine.StackTraceLogType.ScriptOnly);
        }

        // private static string FormatPrintableJson(JsonData json)
        // {
        //     if (null == json)
        //     {
        //         return "";
        //     }
        //
        //     return json.ToString().Replace("{", "{{").Replace("}", "}}");
        // }

        internal static void Debug(string tag, string message, params object[] args)
        {
            if (!CanLog(LogType.DEBUG)) return;
            string msg = GetFormatMessageWithTAG(tag, LogType.DEBUG, message, args);

            UnityEngine.Debug.Log(msg);
        }

        internal static void Info(string tag, string message, params object[] args)
        {
            if (!CanLog(LogType.INFO)) return;

            string msg = GetFormatMessageWithTAG(tag, LogType.INFO, message, args);

            UnityEngine.Debug.Log(msg);
        }

        internal static void Warning(string tag, string message, params object[] args)
        {
            if (!CanLog(LogType.WARNING)) return;

            string msg = GetFormatMessageWithTAG(tag, LogType.WARNING, message, args);

            UnityEngine.Debug.LogWarning(msg);
        }

        internal static void Error(string tag, string message, params object[] args)
        {
            if (!CanLog(LogType.ERROR)) return;

            string msg = GetFormatMessageWithTAG(tag, LogType.ERROR, message, args);

            UnityEngine.Debug.LogError(msg);
        }

        internal static void Exception(string tag, System.Exception exception)
        {
            if (!CanLog(LogType.EXCEPTION)) return;

            bool exNotNull = true;
            if (exception == null)
            {
                exception = new System.Exception("Empty exception");
                exNotNull = false;
            }

            string msg = GetFormatMessageWithTAG(tag, LogType.EXCEPTION, exception.Message);

            if (exNotNull)
            {
                msg = msg + "\nStackTrace:\n" + exception.StackTrace;
            }

            UnityEngine.Debug.LogError(msg);
        }

        internal static void Assert(bool condition, string tag, string message, params object[] args)
        {
            if (!CanLog(LogType.ASSERT)) return;

            string msg = GetFormatMessageWithTAG(tag, LogType.ASSERT, message, args);
            UnityEngine.Debug.Assert(condition, msg);
        }

        internal static void ShowDebugToast(string msg)
        {
            if (!CanLog(LogType.TOAST)) return;

            if (msg == null || msg == "")
            {
                return;
            }
            // if (StarkSDK.EnableStarkSDKDebugToast)
            // {
            //     AndroidUIManager.ShowToastLong(msg);
            // }
        }

        private static string GetFormatMessageWithTAG(string tag, LogType logType, string message, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (message == null)
            {
                message = "";
            }
            sb.Append(logType.ToString()).Append("/").Append(tag).Append(": ");
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            { // WebGL下可能不允许抛出异常，直接会abort
                sb.Append(message);
                if (args != null && args.Length > 0)
                {
                    var s = string.Join(",", args);
                    sb.Append(" " + s);
                }
            }
            else
            {
                try
                {
                    sb.Append(string.Format(message, args));
                }
                catch (FormatException e)
                {
                    sb.Append(message);
                    // UnityEngine.Debug.LogError(e.ToString());
                }
            }
 
            return sb.ToString();
        }

        /// <summary>
        /// 是否可以使用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CanLog(LogType type)
        {
            if ((int)type >= LogLevel)
            {
                var _ = StarkLog.Instance; // 初始化日志
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void SwitchPrintTarget(PrintTarget printTarget, bool open)
        {
            if (open)
            {
                m_PrintTarget |= printTarget;
            }
            else
            {
                m_PrintTarget &= (~printTarget);
            }
        }

        /// <summary>
        /// 打印到文件
        /// </summary>
        private static class StarkPrintFile
        {
            private static object m_Locker = new object();
            private static StreamWriter m_SWLog = null;
            private static string m_LogPath = "";//@"/StarkLog/";
            private static string m_SavePath
            {
                get
                {
                    return Application.dataPath;
                    // if (RuntimePlatform.WindowsPlayer == Application.platform)
                    // {
                    //     UnityEngine.Debug.Log("aaaaaaa");
                    //     return "";
                    // }
                    // return Application.temporaryCachePath;
                }
            }
            private static string m_CutLogFilePath = null;

            /// <summary>
            /// 初始化打印文件的变量
            /// </summary>
            public static void BeginRecord()
            {
                if (!m_PrintTarget.HasFlag(PrintTarget.FILE)) 
                    return;

                string dirPath = string.Concat(m_SavePath, m_LogPath);
                if (!string.IsNullOrEmpty(dirPath) && !System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                System.IO.FileInfo fi = new System.IO.FileInfo(GetFullPath());
                if (fi.Exists)
                {
                    m_SWLog = fi.AppendText();
                }
                else
                {
                    m_SWLog = fi.CreateText();
                }
                Application.logMessageReceivedThreaded += RecordLog;
            }

            /// <summary>
            /// 结束打印
            /// </summary>
            public static void EndRecord() 
            {
                Application.logMessageReceivedThreaded -= RecordLog;
                if (m_SWLog != null) 
                {
                    m_SWLog.Close();
                    m_SWLog.Dispose();
                    m_SWLog = null;
                }
            }

            private static void RecordLog(string condition, string stackTrace, UnityEngine.LogType type)
            {
                lock (m_Locker)
                {
                    try
                    {
                        m_SWLog.Write(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff "));
                        m_SWLog.WriteLine(type.ToString() + ":" + condition);
                        if (NeedLogStackTrace(type))
                        {
                            m_SWLog.WriteLine(stackTrace);
                        }
                        m_SWLog.Flush();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            }

            /// <summary>
            /// 文件名称
            /// </summary>
            /// <returns></returns>
            private static string GetFullPath()
            {
                if (string.IsNullOrEmpty(m_CutLogFilePath))
                {
                    string timespan = DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
                    m_CutLogFilePath = timespan + ".txt";
                }
                return string.Concat(m_SavePath, m_LogPath, m_CutLogFilePath);
            }

            /// <summary>
            /// 是否需要打印栈信息
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private static bool NeedLogStackTrace(UnityEngine.LogType type)
            {
                return UnityEngine.LogType.Exception == type || UnityEngine.LogType.Error == type || UnityEngine.LogType.Assert == type || UnityEngine.LogType.Warning == type;
            }
        }
    }
}
