using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace StarkLive
{
    [Serializable]
    [DataContract]
    public class Gift
    {
        [DataMember(Name = "avatar_url")]
        public string AvatarUrl;
        
        [DataMember(Name = "nickname")]
        public string NickName;
        
        [DataMember(Name = "sec_gift_id")]
        public string GiftId;
        
        [DataMember(Name = "gift_num")]
        public int Num;
        
        [DataMember(Name = "gift_value")]
        public int Value;
        
        [DataMember(Name = "timestamp")]
        public ulong Timestamp;
        // public DateTime Time;
        
        [DataMember(Name = "sec_openid")]
        public string OpenId;
        
        [DataMember(Name = "msg_id")]
        public string MsgId;
    }
}