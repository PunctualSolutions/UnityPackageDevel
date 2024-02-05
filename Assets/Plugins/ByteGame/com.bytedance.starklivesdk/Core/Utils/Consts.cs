using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace StarkLive
{
    public class RpcCallData
    {
        public string ServerMethodName;
        public string ClientMethodName;

        public RpcCallData(string serverMethodName, string clientMethodName)
        {
            ServerMethodName = serverMethodName;
            ClientMethodName = clientMethodName;
        }
    }
    public class Consts
    {
        public const int INIT_WAIT_SECONDS = 10;
        public const int CONFIRM_EVENT_SECONDS = 1; // 1s
        public const int CACHE_EVENT_PUSHID_MAX_COUNT = 2000;
        public const int CACHE_QUERY_LOST_GIFTS_ID_MAX_COUNT = 2000;
        public static readonly Dictionary<RequestType, RpcCallData> RpcCallDatas = new Dictionary<RequestType, RpcCallData>()
        {
            // set app info
            [RequestType.SET_APP_INFO] = new RpcCallData(Consts.REPORT_APP_INFO_SERVER_RPC, Consts.APP_INFO_CONFIRM_CLIENT_RPC),
            // 启动任务
            [RequestType.START_TASK_LIKE] = new RpcCallData(Consts.BEGIN_ROOM_REQ_SERVER_RPC, Consts.BEGIN_ROOM_RSP_CLIENT_RPC),
            [RequestType.START_TASK_COMMENT] = new RpcCallData(Consts.BEGIN_ROOM_REQ_SERVER_RPC, Consts.BEGIN_ROOM_RSP_CLIENT_RPC),
            [RequestType.START_TASK_GIFT] = new RpcCallData(Consts.BEGIN_ROOM_REQ_SERVER_RPC, Consts.BEGIN_ROOM_RSP_CLIENT_RPC),
            // 结束任务
            [RequestType.STOP_TASK_LIKE] = new RpcCallData(Consts.END_ROOM_SERVER_RPC, Consts.END_ROOM_CLIENT_RPC),
            [RequestType.STOP_TASK_GIFT] = new RpcCallData(Consts.END_ROOM_SERVER_RPC, Consts.END_ROOM_CLIENT_RPC),
            [RequestType.STOP_TASK_COMMENT] = new RpcCallData(Consts.END_ROOM_SERVER_RPC, Consts.END_ROOM_CLIENT_RPC),
            // 查询任务状态
            [RequestType.QUERY_TASK_STATE_LIKE] = new RpcCallData(Consts.QUERY_ROOM_STATUS_SERVER_RPC, Consts.QUERY_ROOM_STATUS_CLIENT_RPC),
            [RequestType.QUERY_TASK_STATE_GIFT] = new RpcCallData(Consts.QUERY_ROOM_STATUS_SERVER_RPC, Consts.QUERY_ROOM_STATUS_CLIENT_RPC),
            [RequestType.QUERY_TASK_STATE_COMMENT] = new RpcCallData(Consts.QUERY_ROOM_STATUS_SERVER_RPC, Consts.QUERY_ROOM_STATUS_CLIENT_RPC),
            // 礼物置顶
            [RequestType.TOP_GIFT] = new RpcCallData(Consts.TOP_GIFT_SERVER_RPC, Consts.TOP_GIFT_CLIENT_RPC),
            // 查询平台丢失的礼物
            [RequestType.QUERY_FAILED_GIFT] = new RpcCallData(Consts.QUERY_EVENT_OF_PAGE_SERVER_RPC, Consts.QUERY_EVENT_OF_PAGE_CLIENT_RPC),
            // 写排行榜
            [RequestType.WRITE_RANK] = new RpcCallData(Consts.SET_RANK_DATA_LIST_SERVER_RPC, Consts.SET_RANK_LIST_RSP_CLIENT_RPC),
            // 读排行榜
            [RequestType.READ_RANK] = new RpcCallData(Consts.GET_RANK_DATA_SERVER_RPC, Consts.GET_RANK_RSP_CLIENT_RPC),
            [RequestType.READ_RANK_WITH_OPENID] = new RpcCallData(Consts.GET_RANK_DATA_LIST_SERVER_RPC, Consts.GET_RANK_DATA_LIST_RSP_CLIENT_RPC),
        };

        public const int LOST_GIFT_NUM_EACH_ENTITY = 10000;
        public static readonly string[] LiveEventMsgStr = new string[] { "live_comment", "live_gift", "live_like" };

        public static readonly Dictionary<string, LiveEventType> LiveEventMsgStrDic =
            new Dictionary<string, LiveEventType>
            {
                ["live_comment"] = LiveEventType.COMMENT,
                ["live_gift"] = LiveEventType.GIFT,
                ["live_like"] = LiveEventType.LIKE
            };

        public const string INIT_INFO_CLIENT_RPC = "InitInfoClientRpc";
        public const string APP_INFO_CONFIRM_CLIENT_RPC = "AppInfoConfirmClientRpc";
        public const string BEGIN_ROOM_RSP_CLIENT_RPC = "BeginRoomRspClientRpc";
        public const string ROOM_EVENT_CLIENT_RPC = "RoomEventClientRpc";
        public const string END_ROOM_CLIENT_RPC = "EndRoomClientRpc";
        public const string QUERY_ROOM_STATUS_CLIENT_RPC = "QueryRoomStatusClientRpc";
        public const string QUERY_EVENT_OF_PAGE_CLIENT_RPC = "QueryEventOfPageClientRpc";
        public const string TOP_GIFT_CLIENT_RPC = "TopGiftClientRpc";
        public const string HEARTBEAT_CLIENT_RPC = "HeartbeatClientRpc";
        public const string NOTIFY_LOST_GIFT_CLIENT_RPC = "NotifyLostGiftClientRpc";
        public const string GET_RANK_RSP_CLIENT_RPC = "GetRankRspClientRpc";
        public const string SET_RANK_LIST_RSP_CLIENT_RPC = "SetRankListRspClientRpc";
        public const string NOTIFY_SERVER_ERROR_CLIENT_RPC = "NotifyServerErrorClientRpc";
        public const string GET_RANK_DATA_LIST_RSP_CLIENT_RPC = "GetRankDataListRspClientRpc";

        public const string BEGIN_ROOM_REQ_SERVER_RPC = "BeginRoomReqServerRpc";
        public const string END_ROOM_SERVER_RPC = "EndRoomServerRpc";
        public const string HEARTBEAT_SERVER_RPC = "HeartbeatServerRpc";
        public const string PUSH_CONFIRM_SERVER_RPC = "PushConfirmServerRpc";
        public const string QUERY_LOST_GIFT_SERVER_RPC = "QueryLostGiftServerRpc"; // 查询因为和stark live server断连而丢失的礼物
        public const string NOTIFY_LOST_GIFT_CONFIRM_SERVER_RPC = "NotifyLostGiftConfirmServerRpc";
        public const string TOP_GIFT_SERVER_RPC = "TopGiftServerRpc";
        public const string QUERY_ROOM_STATUS_SERVER_RPC = "QueryRoomStatusServerRpc";
        public const string REPORT_APP_INFO_SERVER_RPC = "ReportAppInfoServerRpc";
        public const string QUERY_EVENT_OF_PAGE_SERVER_RPC = "QueryEventOfPageServerRpc";
        public const string GET_RANK_DATA_SERVER_RPC = "GetRankDataServerRpc";
        public const string SET_RANK_DATA_LIST_SERVER_RPC = "SetRankDataListServerRpc";
        public const string GET_RANK_DATA_LIST_SERVER_RPC = "GetRankDataListServerRpc";

        public const string GAME_LIVE_SERVER = "starklivegate.bytedance.com";
        public const int GAME_LIVE_PORT = 8900;

        public const string PC_STARTTOKEN_KEYWORDS = "-token=";

        public const int MAX_TOP_GIFT_COUNT = 6;
        public static readonly Dictionary<string, string> GiftIdDic = new Dictionary<string, string>
        {
            ["仙女棒"] = "n1/Dg1905sj1FyoBlQBvmbaDZFBNaKuKZH6zxHkv8Lg5x2cRfrKUTb8gzMs=",
            ["甜甜圈"] = "PJ0FFeaDzXUreuUBZH6Hs+b56Jh0tQjrq0bIrrlZmv13GSAL9Q1hf59fjGk=",
            ["恶魔炸弹"] = "gx7pmjQfhBaDOG2XkWI2peZ66YFWkCWRjZXpTqb23O/epru+sxWyTV/3Ufs=",
            ["能力药丸"] = "28rYzVFNyXEXFC8HI+f/WG+I7a6lfl3OyZZjUS+CVuwCgYZrPrUdytGHu0c=",
            ["能量电池"] = "IkkadLfz7O/a5UR45p/OOCCG6ewAWVbsuzR/Z+v1v76CBU+mTG/wPjqdpfg=",
            ["神秘空投"] = "pGLo7HKNk1i4djkicmJXf6iWEyd+pfPBjbsHmd3WcX0Ierm2UdnRR7UINvI=",
            ["超级空投"] = "lsEGaeC++k/yZbzTU2ST64EukfpPENQmqEZxaK9v1+7etK+qnCRKOnDyjsE=",
            ["超能喷射"] = "P7zDZzpeO215SpUptB+aURb1+zC14UC9MY1+MHszKoF0p5gzYk8CNEbey60=",
            ["魔法镜"] = "fJs8HKQ0xlPRixn8JAUiL2gFRiLD9S6IFCFdvZODSnhyo9YN8q7xUuVVyZI="
        };
        public const int APPID_EMPTY_ERROR_CODE = 9993;
        public const string APPID_EMPTY_ERROR_MESSAGE = "Input appid is empty.";
        public const int ALREADY_REQUEST_INIT_ERROR_CODE = 9994;
        public const string ALREADY_REQUEST_INIT_ERROR_MESSAGE = "Already request init, please wait.";
        public const int INIT_TIMEOUT_ERROR_CODE = 9995;
        public const string INIT_TIMEOUT_ERROR_MESSAGE = "Init timeout";
        public const int ALREADY_INITIALIZE_ERROR_CODE = 9996;
        public const string ALREADY_INITIALIZE_ERROR_MESSAGE = "StarkLiveSDK has already init";
        public const int NOT_STARTTASK_ERROR_CODE = 9997;
        public const string NOT_STARTTASK_ERROR_MESSAGE = "Need start task first";
        public const int NOT_INITIALIZE_ERROR_CODE = 9998;
        public const string NOT_INITIALIZE_ERROR_MESSAGE = "StarkLiveSDK need init or net is reconnecting";
        public const int NET_DISCONNECTED_ERRORCODE = 9999;
        public const string NET_DISCONNECTED_ERRORMESSAGE = "Net disconnected, callback return error!";
        public const int MART_CHECK_TASK_STARK_SERVER_ERRORCODE = -10010;
    }

    
}