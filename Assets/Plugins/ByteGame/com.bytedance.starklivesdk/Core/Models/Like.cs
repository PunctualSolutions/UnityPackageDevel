using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace StarkLive
{
    [Serializable]
    [DataContract]
    public class Like
    {
        [DataMember(Name = "like_num")]
        public int Num { get; set; }
        
        
        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }
        
        
        [DataMember(Name = "nickname")]
        public string NickName { get; set; }
        
        
        [DataMember(Name = "timestamp")]
        public ulong Timestamp { get; set; }
        // public DateTime Time;
        
        
        [DataMember(Name = "sec_openid")]
        public string OpenId { get; set; }
        
        [DataMember(Name = "msg_id")]
        public string MsgId;
    }
}