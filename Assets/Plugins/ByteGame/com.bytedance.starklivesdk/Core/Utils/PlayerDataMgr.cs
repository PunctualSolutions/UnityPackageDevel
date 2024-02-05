using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkLive
{
    public class PlayerDataMgr
    {
        // private static readonly string LogColor = "cyan";
        // private static readonly string TAG = "PlayerDataMgr";

        private static PlayerDataMgr _instance;

        public static PlayerDataMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerDataMgr();
                }

                return _instance;
            }
        }
        private Dictionary<string, PlayerData> playerDataDic = new Dictionary<string, PlayerData>();
        
        public void SavePlayerDataMgr(string openId, PlayerData data)
        {
            if (!playerDataDic.ContainsKey(openId))
            {
                playerDataDic.Add(openId, data);
            }
            else
            {
                playerDataDic[openId] = data;
            }
        }
        public void SavePlayerData(Comment[] comments)
        {
            for (int i = 0; i < comments.Length; i++)
            {
                var item = comments[i];
                var playerData = new PlayerData()
                {
                    AvatarUrl = item.AvatarUrl,
                    NickName = item.Nickname
                };
                if (!playerDataDic.ContainsKey(item.OpenId))
                {
                    playerDataDic.Add(item.OpenId, playerData);
                }
                else
                {
                    playerDataDic[item.OpenId] = playerData;
                }
            }
        }
        public void SavePlayerData(Like[] likes)
        {
            for (int i = 0; i < likes.Length; i++)
            {
                var item = likes[i];
                var playerData = new PlayerData()
                {
                    AvatarUrl = item.AvatarUrl,
                    NickName = item.NickName
                };
                if (!playerDataDic.ContainsKey(item.OpenId))
                {
                    playerDataDic.Add(item.OpenId, playerData);
                }
                else
                {
                    playerDataDic[item.OpenId] = playerData;
                }
            }
        }
        public void SavePlayerData(Gift[] gifts)
        {
            for (int i = 0; i < gifts.Length; i++)
            {
                var item = gifts[i];
                var playerData = new PlayerData()
                {
                    AvatarUrl = item.AvatarUrl,
                    NickName = item.NickName
                };
                if (!playerDataDic.ContainsKey(item.OpenId))
                {
                    playerDataDic.Add(item.OpenId, playerData);
                }
                else
                {
                    playerDataDic[item.OpenId] = playerData;
                }
            }
        }

        public PlayerData GetPlayerData(string openId)
        {
            if (playerDataDic.ContainsKey(openId))
            {
                return playerDataDic[openId];
            }
            else
            {
                return new PlayerData();
            }
        }
    }
}