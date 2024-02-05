using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace StarkLive
{
    [Serializable]
    internal class RankDataServer
    {
        public long score = 0;
        public int rank = 0;
        public string extra_data = string.Empty;
        public string member = string.Empty;
        
        public RankDataServerUserInfo user_info = new RankDataServerUserInfo();
    }
    
    [Serializable]
    internal class RankDataServerUserInfo
    {
        public string sec_uid = string.Empty;
        public string nickname = string.Empty;
        public string avatar_url = string.Empty;
    }
    
    public class RankData
    {
        public long Score;
        public int Rank;
        public string AvatarUrl;
        public string Nickname;
        public string AppId;
        public int RankId;
        public RankTimeType TimeType;
        public string OpenId;
        
        public string UserSecUid;
    }
}