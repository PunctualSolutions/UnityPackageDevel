using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StarkLive
{
    public class Utils
    {
        public static RequestType GetStartTaskRequestType(LiveEventType eventType)
        {
            switch (eventType)
            {
                case LiveEventType.GIFT: return RequestType.START_TASK_GIFT;
                case LiveEventType.COMMENT: return RequestType.START_TASK_COMMENT;
                case LiveEventType.LIKE: return RequestType.START_TASK_LIKE;
            }

            return RequestType.START_TASK_LIKE;
        }
        public static RequestType GetStopTaskRequestType(LiveEventType eventType)
        {
            switch (eventType)
            {
                case LiveEventType.GIFT: return RequestType.STOP_TASK_GIFT;
                case LiveEventType.COMMENT: return RequestType.STOP_TASK_COMMENT;
                case LiveEventType.LIKE: return RequestType.STOP_TASK_LIKE;
            }

            return RequestType.STOP_TASK_LIKE;
        }
        public static RequestType GetQueryTaskStateRequestType(LiveEventType eventType)
        {
            switch (eventType)
            {
                case LiveEventType.GIFT: return RequestType.QUERY_TASK_STATE_GIFT;
                case LiveEventType.COMMENT: return RequestType.QUERY_TASK_STATE_COMMENT;
                case LiveEventType.LIKE: return RequestType.QUERY_TASK_STATE_LIKE;
            }

            return RequestType.QUERY_TASK_STATE_LIKE;
        }
        
        public static string GetMsgTypeStr(LiveEventType eventType)
        {
            switch (eventType)
            {
                case LiveEventType.GIFT: return "live_gift";
                case LiveEventType.COMMENT: return "live_comment";
                case LiveEventType.LIKE: return "live_like";
            }

            return "live_like";
        }
        public static RankTimeType GetRankTimeType(string serverTypeStr)
        {
            switch (serverTypeStr)
            {
                case "RankTimeTypeAll": return RankTimeType.TOTAL;
                case "RankTimeTypeMonth": return RankTimeType.MONTH;
                case "RankTimeTypeWeek": return RankTimeType.WEEK;
                case "RankTimeTypeDay": return RankTimeType.DAY;
            }

            return RankTimeType.TOTAL;
        }
        public static LiveEventType GetLiveEventType(string msgType)
        {
            LiveEventType result = LiveEventType.LIKE;
            switch (msgType)
            {
                case "live_gift": result = LiveEventType.GIFT;
                    break;
                case "live_comment": result = LiveEventType.COMMENT;
                    break;
                case "live_like": result = LiveEventType.LIKE;
                    break;
            }

            return result;
        }

        internal static string GetWriteRankDataJson(List<WriteRankData> list)
        {
            string json = "[";
            string jsonContent = "";
            for (int i = 0 ; i < list.Count; i++)
            {
                var item = list[i];
                jsonContent += "{";
                var extraData = GetPlayerDataJson(item.OpenId);
                extraData = extraData.Replace("\"", "\\\"");
                jsonContent += $"\"sec_openid\":\"{item.OpenId}\",\"chg_value\":{item.ChangeValue},\"extra_data\" : \"{extraData}\"";
                jsonContent += "}";
                if (i < list.Count - 1)
                {
                    jsonContent += ", ";
                }
            }
            json = json + jsonContent + "]";
            json = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json));
            Debug.Log($"json: {json}");
            return json;
        }

        internal static string GetPlayerDataJson(string openId)
        {
            var data = PlayerDataMgr.Instance.GetPlayerData(openId);
            return JsonUtility.ToJson(data);
        }

        internal static RankData GetRankData(RankDataServer data)
        {
            RankData rankData = new RankData()
            {
                Score = data.score,
                Rank = data.rank
            };
            try
            {
                var playerData = JsonUtility.FromJson<PlayerData>(data.extra_data);
                rankData.AvatarUrl = playerData.AvatarUrl;
                rankData.Nickname = playerData.NickName;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"StarkLiveSDK GetRankData exception: {e}");
            }
            rankData.AppId = data.member.Substring(0, data.member.IndexOf("_"));
            var str0 = data.member.Substring(data.member.IndexOf("_") + 1);
            rankData.RankId = Convert.ToInt32(str0.Substring(0, str0.IndexOf("_")));
            var str1 = str0.Substring(str0.IndexOf("_") + 1);
            rankData.TimeType = GetRankTimeType(str1.Substring(0, str1.IndexOf("_")));
            rankData.OpenId = str1.Substring(str1.IndexOf("_") + 1);
            
            rankData.UserSecUid = data.user_info.sec_uid;
            // rankData.Nickname = data.user_info.nickname;
            // rankData.AvatarUrl = data.user_info.avatar_url;
            
            return rankData;
        }

        internal static int rankReqId = 0;
        internal static int GetRankReqId()
        {
            if (rankReqId == int.MaxValue)
            {
                rankReqId = 0;
            }
            else
            {
                rankReqId++;
            }
            return rankReqId;
        }
    }
}