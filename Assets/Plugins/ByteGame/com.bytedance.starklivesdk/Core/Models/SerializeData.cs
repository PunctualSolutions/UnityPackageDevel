using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace StarkLive
{
    [Serializable]
    public class AccessTokenRequestData
    {
        public string appid;
        public string secret;
        public string grant_type;

        public AccessTokenRequestData(string appid, string secret, string grantType)
        {
            this.appid = appid;
            this.secret = secret;
            this.grant_type = grantType;
        }
    }

    [Serializable]
    public class GetAccessTokenResult
    {
        public int err_no;
        public string err_tips;
        public GetAccessTokenResultData data;
    }

    [Serializable]
    public class GetAccessTokenResultData
    {
        public string access_token;
        public int expires_in;
    }

    [Serializable]
    public class GetRoomInfoRequestData
    {
        public string token;

        public GetRoomInfoRequestData(string token)
        {
            this.token = token;
        }
    }

    [Serializable]
    public class GetRoomInfoResult
    {
        public GetRoomInfoResultData data;
    }

    [Serializable]
    public class GetRoomInfoResultData
    {
        public GetRoomInfoResultInfo info;
    }

    [Serializable]
    public class GetRoomInfoResultInfo
    {
        public ulong room_id;
    }

    [Serializable]
    public class GiftIdList
    {
        public string[] data;
    }

    

    
    
    [Serializable]
    public class SetTopGiftResponse
    {
        public TopGiftList data;
    }
    [Serializable]
    public class TopGiftList
    {
        public string[] success_top_gift_id_list;
    }
    [Serializable]
    public class EndTaskResponse
    {
        public string logid;
        public object data;
        public string err_msg;
        public int err_no;
    }
    [Serializable]
    public class QueryStatusResponse
    {
        public string logid;
        public TaskStatus data;
        public string err_msg;
        public int err_no;
    }
    [Serializable]
    public class TaskStatus
    {
        public int status;
    }
    [Serializable]
    public class QueryFailedGiftResponse
    {
        public string logid;
        public QueryFailedGiftPageData data;
        public string err_msg;
        public int err_no;
    }
    [Serializable]
    [DataContract]
    public class QueryNetDisconnectedFailedGiftResponse
    {
        [DataMember(Name = "msgid")] 
        public ulong msgid;
        [DataMember(Name = "payload")] 
        private string giftArrayStr;
        public string GiftArrayStr
        {
            get { return giftArrayStr; }
            set { giftArrayStr = GiftArrayStr.Replace('\\', '\0'); }
        }
        public Gift[] GetGiftArray()
        {
            return JsonConvert.DeserializeObject<Gift[]>(GiftArrayStr);
        }
    }
}