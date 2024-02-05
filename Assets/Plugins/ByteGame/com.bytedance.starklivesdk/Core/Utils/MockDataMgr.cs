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
    public class MockTaskData
    {
        // 任务状态
        public LiveTaskState TaskState = LiveTaskState.INVALID;

        // 开启任务
        public int StartTaskErrorCode;

        public string StartTaskErrorMessage;

        // 结束任务
        public int EndTaskErrorCode;
        public string EndTaskErrorMessage;

        public EndTaskResponse EndTaskData
        {
            get
            {
                return new EndTaskResponse()
                {
                    logid = "mock_end_task_logid",
                    data = new TaskStatus() { status = (int)TaskState },
                    err_msg = "",
                    err_no = 0
                };
            }
        }

        // 查询状态
        public int QueryStatusErrorCode;
        public string QueryStatusErrorMessage;

        public QueryStatusResponse QueryStatusData
        {
            get
            {
                return new QueryStatusResponse()
                {
                    logid = "mock_query_task_status_logid",
                    data = new TaskStatus() { status = (int)TaskState },
                    err_msg = "",
                    err_no = 0
                };
            }
        }
    }

    public class MockDataMgr
    {
        // 评论
        public string[] MockComments = new string[]
        {
            "{\"content\":\"test水水水\", \"avatar_url\": \"commentavatar1\", \"nickname\":\"Jack\",\"timestamp\":0, \"sec_openid\":\"mockOpenId1\", \"msg_id\":\"mockMsgId1\"}",
            "{\"content\":\"test火火火\", \"avatar_url\": \"commentavatar2\", \"nickname\":\"Sam\",\"timestamp\":0, \"sec_openid\":\"mockOpenId2\", \"msg_id\":\"mockMsgId2\"}"
        };

        // 礼物
        public string[] MockGifts = new string[]
        {
            "{\"sec_gift_id\":\"1\", \"avatar_url\": \"giftavatar1\", \"nickname\":\"Jack\",\"timestamp\":0, \"sec_openid\":\"mockOpenId1\", \"msg_id\":\"mockMsgId1\"," +
            "\"gift_num\":2,\"gift_value\":10}",
            "{\"sec_gift_id\":\"2\", \"avatar_url\": \"giftavatar2\", \"nickname\":\"Sam\",\"timestamp\":0, \"sec_openid\":\"mockOpenId2\", \"msg_id\":\"mockMsgId2\"," +
            "\"gift_num\":3,\"gift_value\":300}"
        };

        // 点赞
        public string[] MockLikes = new string[]
        {
            "{\"like_num\":2, \"avatar_url\": \"likeavatar1\", \"nickname\":\"Jack\",\"timestamp\":0, \"sec_openid\":\"mockOpenId1\", \"msg_id\":\"mockMsgId1\"}",
            "{\"like_num\":3, \"avatar_url\": \"likeavatar2\", \"nickname\":\"Sam\",\"timestamp\":0, \"sec_openid\":\"mockOpenId2\", \"msg_id\":\"mockMsgId2\"}"
        };

        // 任务相关
        public Dictionary<LiveEventType, MockTaskData> MockTaskDatas = new Dictionary<LiveEventType, MockTaskData>()
        {
            [LiveEventType.GIFT] = new MockTaskData(),
            [LiveEventType.LIKE] = new MockTaskData(),
            [LiveEventType.COMMENT] = new MockTaskData(),
        };


        // 礼物置顶结果
        public int TopGiftErrorCode = 0;
        public string TopGiftErrorMessage = String.Empty;
        public SetTopGiftResponse TopGiftResponse = new SetTopGiftResponse()
        {
            data = new TopGiftList()
            {
                success_top_gift_id_list = new string[] { "1", "2", "3" }
            }
        };

        // 查询丢失礼物结果
        public int QueryFailedGiftErrorCode = 0;
        public string QueryFailedGiftErrorMessage = String.Empty;
        public string GetMockQueryFailedGiftResponse()
        {
            string page1Payload = "[{\\\"msg_id\\\":\\\"1\\\",\\\"sec_openid\\\":\\\"mockOpenId1\\\",\\\"sec_gift_id\\\":\\\"1\\\",\\\"gift_num\\\":1,\\\"gift_value\\\":100,\\\"avatar_url\\\":\\\"mockAvatarUrl\\\",\\\"nickname\\\":\\\"Lucy\\\",\\\"timestamp\\\":1687764017000}]";
            string page2Payload = "[{\\\"msg_id\\\":\\\"2\\\",\\\"sec_openid\\\":\\\"mockOpenId2\\\",\\\"sec_gift_id\\\":\\\"2\\\",\\\"gift_num\\\":2,\\\"gift_value\\\":200,\\\"avatar_url\\\":\\\"mockAvatarUrl\\\",\\\"nickname\\\":\\\"Lily\\\",\\\"timestamp\\\":1687764017000}]";
            string page1 = "{\"room_id\":\"" + MockRoomId + "\", \"msg_type\":\"live_gift\",\"payload\":\"" + page1Payload + "\"}";
            string page2 = "{\"room_id\":\"" + MockRoomId + "\", \"msg_type\":\"live_gift\",\"payload\":\"" + page2Payload + "\"}";
            string pages = "[" + page1 + "," + page2 + "]";
            string data = "{\"page_num\":1,\"total_count\":2,\"data_list\":" + pages + "}";
            string result = "{\"logid\":\"111\", \"err_msg\":\"\",\"err_no\":0,\"data\":" + data + "}";
            return result;
        }

        // 读排行榜结果
        public int ReadRankErrorCode;
        public string ReadRankErrorMessage;
        public int ReadRankRankId;
        public int ReadRankTimeType;
        public int ReadRankPageIndex;
        public int ReadRankPageSize;
        private string[] MockRankData = new string[]
        {
            "{\"score\":100,\"rank\":1,\"extra_data\":\"{\\\"AvatarUrl\\\":\\\"avatar1\\\",\\\"NickName\\\":\\\"Jack\\\"}\",\"member\":\"appid_1_1_openid\"}",
            "{\"score\":99,\"rank\":2,\"extra_data\":\"{\\\"AvatarUrl\\\":\\\"avatar2\\\",\\\"NickName\\\":\\\"Sam\\\"}\",\"member\":\"appid_1_1_openid\"}"
        };
        private string[] MockRankDataWithUserInfo = new string[]
        {
            "{\"score\":100,\"rank\":1,\"extra_data\":\"{\\\"AvatarUrl\\\":\\\"avatar1\\\",\\\"NickName\\\":\\\"Jack\\\"}\",\"member\":\"appid_1_1_openid\", \"user_info\":{\"sec_uid\":\"test_sec_uid1\",\"nickname\":\"Jack1\", \"avatar_url\":\"avatar11\"}}",
            "{\"score\":99,\"rank\":2,\"extra_data\":\"{\\\"AvatarUrl\\\":\\\"avatar2\\\",\\\"NickName\\\":\\\"Sam\\\"}\",\"member\":\"appid_1_1_openid\", \"user_info\":{\"sec_uid\":\"test_sec_uid2\",\"nickname\":\"Sam1\", \"avatar_url\":\"avatar22\"}}"
        };

        // 写排行榜结果
        public int WriteRankErrorCode;
        public string WriteRankErrorMessage;
        public int WriteRankRankId;
        
        

        
        public string GetMockRoomEventPayload(LiveEventType type)
        {
            string payload = string.Empty;
            switch (type)
            {
                case LiveEventType.GIFT:
                    for (int i = 0; i < MockGifts.Length; i++)
                    {
                        payload += MockGifts[i] + ",";
                    }

                    break;
                case LiveEventType.LIKE:
                    for (int i = 0; i < MockLikes.Length; i++)
                    {
                        payload += MockLikes[i] + ",";
                    }

                    break;
                case LiveEventType.COMMENT:
                    for (int i = 0; i < MockComments.Length; i++)
                    {
                        payload += MockComments[i] + ",";
                    }

                    break;
            }

            payload = payload.Substring(0, payload.Length - 1);
            return $"[{payload}]";
        }
        public ulong GetRoomEventPushId()
        {
            roomEventPushId++;
            return roomEventPushId;
        }

        private void PrintLog(string log)
        {
            StarkLiveSDK.PrintLog(TAG, log, LogColor);
        }
        public string GetRankDataPayload(bool withUserInfo = false)
        {
            string payload = string.Empty;
            var rankData = withUserInfo ? MockRankDataWithUserInfo : MockRankData;
            for (int i = 0; i < rankData.Length; i++)
            {
                payload += rankData[i] + ",";
            }
            payload = payload.Substring(0, payload.Length - 1);
            return $"[{payload}]";
        }
        public const string MockRoomId = "123";
        private static readonly string LogColor = "cyan";
        private static readonly string TAG = "MockDataMgr";
        private ulong roomEventPushId = 100001;
        private static MockDataMgr _instance;
        public static MockDataMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MockDataMgr();
                }

                return _instance;
            }
        }
    }
}