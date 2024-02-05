using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace StarkLive
{
    [Serializable]
    [DataContract]
    public class Comment
    {
        [DataMember(Name = "content")]
        public string Content;
        
        [DataMember(Name = "avatar_url")]
        public string AvatarUrl;
        
        [DataMember(Name = "nickname")]
        public string Nickname;
        
        [DataMember(Name = "timestamp")]
        public ulong Timestamp;
        // public DateTime Time;
        
        [DataMember(Name = "sec_openid")]
        public string OpenId { get; set; }
        
        [DataMember(Name = "msg_id")]
        public string MsgId;
    }
}