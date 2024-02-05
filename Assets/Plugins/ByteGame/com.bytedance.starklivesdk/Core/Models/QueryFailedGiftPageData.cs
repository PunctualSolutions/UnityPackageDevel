using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace StarkLive
{
    [Serializable]
    [DataContract]
    public class QueryFailedGiftPageData
    {
        [DataMember(Name = "data_list")]
        public QueryFailedGiftData[] FailedGiftDataArray { get; set; }
        [DataMember(Name = "page_num")]
        public int PageNum { get; set; }
        [DataMember(Name = "total_count")]
        public int TotalCount { get; set; }
    }
    [Serializable]
    [DataContract]
    public class QueryFailedGiftData
    {
        [DataMember(Name = "room_id")]
        public string RoomId { get; set; }
        [DataMember(Name = "msg_type")]
        public string MsgType { get; set; }

        [DataMember(Name = "payload")] 
        private string failedGiftInfosStr;
        public string FailedGiftInfosStr
        {
            get { return failedGiftInfosStr; }
            set { failedGiftInfosStr = FailedGiftInfosStr.Replace('\\', '\0'); }
        }

        // Deprecated. Please use 'GetQueryFailedGifts'
        public QueryFailedGiftInfo[] GetQueryFailedGiftInfoArray()
        {
            return JsonConvert.DeserializeObject<QueryFailedGiftInfo[]>(FailedGiftInfosStr);
        }
        
        public Gift[] GetQueryFailedGifts()
        {
            return JsonConvert.DeserializeObject<Gift[]>(FailedGiftInfosStr);
        }
        
        // for client mock
        public void SetFailedGiftDataArray(string str)
        {
            failedGiftInfosStr = str;
        }
    }
    [Serializable]
    [DataContract]
    public class QueryFailedGiftInfo
    {
        [DataMember(Name = "msg_id")]
        public string MsgId { get; set; }
        [DataMember(Name = "sec_openid")]
        public string OpenId { get; set; }
        [DataMember(Name = "sec_gift_id")]
        public string GiftId { get; set; }
        [DataMember(Name = "gift_num")]
        public int GiftNum { get; set; }
        [DataMember(Name = "gift_value")]
        public int GiftValue { get; set; }
        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }
        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }
        [DataMember(Name = "timestamp")]
        public ulong Timestamp { get; set; }
    }
}