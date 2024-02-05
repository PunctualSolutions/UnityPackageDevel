using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StarkLive;
using StarkNetwork;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
    {
        private static readonly string LogColor = "orange";
        private static readonly string TAG = "Test";
        private string log = string.Empty;
        public InputField IPPortInputField;
        public InputField AppIdInputField;
        public InputField PageIndexInputField;
        public InputField PageSizeInputField;
        public ToggleGroup MsgTypeToggleGroup;
        public ToggleGroup IPPortToggleGroup;
        public Toggle TestNetToggle;
        public Toggle TestLogToggle, APILogToggle, LiveObjLogToggle;
        public Button UninitBtn;
        public Button InitBtn;
        public Button BeginTaskBtn;
        public Button ClearLogBtn;
        public Button SetGiftTopBtn;
        public Button EndTaskBtn;
        public Button QueryStatusBtn;
        public Button QueryFailedGiftBtn;
        public Button TestBeginBtn;
        public Button ShowLogBtn;
        public Button GiftSelectBtn;
        public Button SetRankBtn;
        public Button SetLikeRankBtn;
        public Button GetRankBtn;
        public Button GetRankWithOpenIdsBtn;
        public Button PrintRequestRecordsBtn;
        public Dropdown SelectRankTimetype;
        public GameObject GiftSelectItem;
        public GameObject GiftSelectParent;
        public GameObject GiftSelectListRoot;
        public GameObject LogPanel;
        public InputField RankValue;
        public InputField RankOpenId;
        public InputField RankId;
        public Text LogText;
        private ILiveObj liveObj;
        private List<string> selectTopGiftIds = new List<string>();
        private List<WriteRankData> likeRankDataList = new List<WriteRankData>(); // 统计点赞数据变化

        private Dictionary<LiveEventType, Action<int, string, object>> startTaskCallbackDic =
            new Dictionary<LiveEventType, Action<int, string, object>>();
        
        private Dictionary<LiveEventType, Action<int, string, object>> stopTaskCallbackDic =
            new Dictionary<LiveEventType, Action<int, string, object>>();

        private Dictionary<LiveEventType, Action<int, string, LiveTaskState>> queryTaskStateCallbackDic =
            new Dictionary<LiveEventType, Action<int, string, LiveTaskState>>();

        private void Awake()
        {
            startTaskCallbackDic.Add(LiveEventType.GIFT, OnStartGiftTaskCallback);
            startTaskCallbackDic.Add(LiveEventType.COMMENT, OnStartCommentTaskCallback);
            startTaskCallbackDic.Add(LiveEventType.LIKE, OnStartLikeTaskCallback);
            queryTaskStateCallbackDic.Add(LiveEventType.LIKE,
                (errorCode, errorMessage, state) =>
                {
                    OnQueryTaskStatusCallback("like", errorCode, errorMessage, state);
                });
            queryTaskStateCallbackDic.Add(LiveEventType.COMMENT,
                (errorCode, errorMessage, state) =>
                {
                    OnQueryTaskStatusCallback("comment", errorCode, errorMessage, state);
                });
            queryTaskStateCallbackDic.Add(LiveEventType.GIFT,
                (errorCode, errorMessage, state) =>
                {
                    OnQueryTaskStatusCallback("gift", errorCode, errorMessage, state);
                });
            
            stopTaskCallbackDic.Add(LiveEventType.LIKE,
                (errorCode, errorMessage, obj) =>
                {
                    OnEndTaskCallback("like", errorCode, errorMessage, obj);
                });
            stopTaskCallbackDic.Add(LiveEventType.COMMENT,
                (errorCode, errorMessage, obj) =>
                {
                    OnEndTaskCallback("comment", errorCode, errorMessage, obj);
                });
            stopTaskCallbackDic.Add(LiveEventType.GIFT,
                (errorCode, errorMessage, obj) =>
                {
                    OnEndTaskCallback("gift", errorCode, errorMessage, obj);
                });
            GiftSelectBtn.onClick.AddListener(() =>
            {
                GiftSelectListRoot.SetActive(!GiftSelectListRoot.activeSelf);
            });
            foreach (var item in Consts.GiftIdDic)
            {
                GameObject obj = Instantiate(GiftSelectItem, GiftSelectParent.transform);
                obj.transform.Find("Item Label").GetComponent<Text>().text = $"{item.Key} {item.Value}";
                obj.SetActive(true);
                obj.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        if (selectTopGiftIds.Count >= Consts.MAX_TOP_GIFT_COUNT)
                        {
                            // selectTopGiftIds.RemoveAt(0);
                            obj.GetComponent<Toggle>().isOn = false;
                        }
                        else
                        {
                            selectTopGiftIds.Add(item.Value);
                        }
                    }
                    else
                    {
                        selectTopGiftIds.Remove(item.Value);
                    }
                }); 
            }

            StarkLiveSDK.SetDebugAction(AddLog);
            InitBtnListeners();
            
            PrintLog($"StarkLiveSDK version: {StarkLiveSDK.GetSDKVersion()}");
        }

        IEnumerator DelayRequest(Action action)
        {
            yield return new WaitForSeconds(5);
            action();
        }

        void SendRequest(Action action)
        {
            if (TestNetToggle.isOn)
            {
                StartCoroutine(DelayRequest(action));
            }
            else
            {
                action();
            }
        }
        void InitBtnListeners()
        {
            ShowLogBtn.onClick.AddListener(() =>
            {
                LogPanel.SetActive(!LogPanel.activeSelf);
            });
            // 1. init
            // 2. set app info
            // 3. begin all task
            // 4. set gift top
            TestBeginBtn.onClick.AddListener(() =>
            {
                SetInitConfig();
                bool result = StarkLiveSDK.API.Init(GetAppId(), (errorCode, errorMessage, uid) =>
                {
                    if (errorCode != 0)
                    {
                        PrintLog($"init failed. errorCode: {errorCode}, errorMessage: {errorMessage}");
                    }
                    else
                    {
                        PrintLog($"init success. uid: {uid}");
                        likeRankDataList.Clear();
                        StarkLiveSDK.API.StartLiveTask(LiveEventType.COMMENT, startTaskCallbackDic[LiveEventType.COMMENT]);
                        StarkLiveSDK.API.StartLiveTask(LiveEventType.LIKE, startTaskCallbackDic[LiveEventType.LIKE]);
                        StarkLiveSDK.API.StartLiveTask(LiveEventType.GIFT, startTaskCallbackDic[LiveEventType.GIFT]);
                    }
                });
                
                PrintLog($"StarkLiveSDK.API.Init return {result}");
            });
            ClearLogBtn.onClick.AddListener(() => { ClearLog(); });
            UninitBtn.onClick.AddListener(() =>
            {
                bool result = StarkLiveSDK.API.UnInit();
                PrintLog($"StarkLiveSDK.API.UnInit return {result}");
            });
            BeginTaskBtn.onClick.AddListener(() =>
            {
                SendRequest(() =>
                {
                    StarkLiveSDK.API.StartLiveTask(GetCurSelectEventType(), startTaskCallbackDic[GetCurSelectEventType()]);
                });
            });
            EndTaskBtn.onClick.AddListener(() =>
            {
                SendRequest(() =>
                {
                    StarkLiveSDK.API.StopLiveTask(GetCurSelectEventType(), stopTaskCallbackDic[GetCurSelectEventType()]);
                });
            });
            QueryStatusBtn.onClick.AddListener(() =>
            {
                SendRequest(() =>
                {
                    StarkLiveSDK.API.QueryStatus(GetCurSelectEventType(), queryTaskStateCallbackDic[GetCurSelectEventType()]);
                });
            });
            SetGiftTopBtn.onClick.AddListener(() =>
            {
                SendRequest(() =>
                {
                    StarkLiveSDK.API.SetGiftTop(GetSelectGifts(), OnSetGiftTop);
                });
            });
            InitBtn.onClick.AddListener(() =>
            {
                PrintLog($"click init btn");
                SetInitConfig();
                SendRequest(() =>
                {
                    bool result = StarkLiveSDK.API.Init(GetAppId(), (errorCode, errorMessage, uid) =>
                    {
                        if (errorCode != 0)
                        {
                            PrintLog($"init failed. errorCode: {errorCode}, errorMessage: {errorMessage}");
                        }
                        else
                        {
                            likeRankDataList.Clear();
                            PrintLog($"init success. uid: {uid}");
                        }
                    });
                    if (result == false)
                    {
                        PrintLog($"StarkLiveSDK.API.Init return false");
                    }
                });
            });
            QueryFailedGiftBtn.onClick.AddListener(() =>
            {
                SendRequest(() =>
                {
                    StarkLiveSDK.API.QueryFailedGift(Convert.ToInt32(PageIndexInputField.text), Convert.ToInt32(PageSizeInputField.text),
                        (errorCode, errorMessage, queryFailedGiftPageData) =>
                        {
                            if (errorCode != 0)
                            {
                                PrintLog($"query failed gift failed. errorCode: {errorCode}, errorMessage: {errorMessage}");
                                return;
                            }

                            LogFailedGiftInfo(queryFailedGiftPageData);
                        });
                });
            });
            
            StarkLiveSDK.API.SetReceiveLikeCallback((likes) =>
            {
                for (int i = 0; i < likes.Length; i++)
                {
                    var like = likes[i];
                    PrintLog($"likes[{i}]: \nnickName: {like.NickName}\nlikeNum: {like.Num}\navatarUrl: {like.AvatarUrl}\ntimeStamp: {like.Timestamp}" +
                             $"\nopenId: {like.OpenId}\nmsgId: {like.MsgId}");
                    SaveLikeRankData(like.OpenId, like.Num);
                }
            });
            StarkLiveSDK.API.SetReceiveCommentCallback((comments) =>
            {
                for (int i = 0; i < comments.Length; i++)
                {
                    var comment = comments[i];
                    PrintLog($"comments[{i}]: \nnickName: {comment.Nickname}\ncontent: {comment.Content}\navatarUrl: {comment.AvatarUrl}\ntimeStamp: {comment.Timestamp}" +
                             $"\nopenId: {comment.OpenId}\nmsgId: {comment.MsgId}");
                }
            });
            StarkLiveSDK.API.SetReceiveGiftCallback((gifts) =>
            {
                for (int i = 0; i < gifts.Length; i++)
                {
                    var gift = gifts[i];
                    PrintLog($"gifts[{i}]: \nnickName: {gift.NickName}\ngiftId: {gift.GiftId}\ngiftNum: {gift.Num}\ngiftValue: {gift.Value}\navatarUrl: {gift.AvatarUrl}" +
                             $"\ntimeStamp: {gift.Timestamp}\nopenid: {gift.OpenId}\nmsgId: {gift.MsgId}");
                }
            });
            // 根据输入框内容写排行榜
            SetRankBtn.onClick.AddListener(()=>
            {
                var data = new List<WriteRankData>();
                List<long> changeValues = RankValue.text.Split(',').ToList().ConvertAll(input => Convert.ToInt64(input));
                List<string> openIds = RankOpenId.text.Split(',').ToList();
                for (int i = 0; i < openIds.Count; i++)
                {
                    data.Add(new WriteRankData()
                    {
                        ChangeValue = changeValues[i],
                        OpenId = openIds[i]
                    });
                }
                StarkLiveSDK.API.SetRankDataList(Convert.ToInt32(RankId.text), data, (errorCode, errorMessage, rankId) =>
                {
                    PrintLog($"SetRankDataList errorCode: {errorCode}, errorMessage: {errorMessage}, rankId: {rankId}");
                });
            });
            SetLikeRankBtn.onClick.AddListener(()=>
            {
                var data = GetLikeRankList();
                if (data.Count == 0)
                {
                    PrintLog($"donot have like data, cannot set like rank.");
                    return;
                }
                StarkLiveSDK.API.SetRankDataList(Convert.ToInt32(RankId.text), data, (errorCode, errorMessage, rankId) =>
                {
                    if (errorCode == 0)
                    {
                        likeRankDataList.Clear();
                    }
                    PrintLog($"SetRankDataList errorCode: {errorCode}, errorMessage: {errorMessage}, rankId: {rankId}");
                });
            });
            GetRankBtn.onClick.AddListener(()=>
            {
                StarkLiveSDK.API.GetRankData(Convert.ToInt32(RankId.text), (RankTimeType)(SelectRankTimetype.value + 1), Convert.ToInt32(PageIndexInputField.text), Convert.ToInt32(PageSizeInputField.text)
                    , (errorCode, errorMessage, rankDatas) =>
                {
                    if (errorCode != 0)
                    {
                        PrintLog($"GetRankData errorCode: {errorCode}, errorMessage: {errorMessage}");
                    }
                    else
                    {
                        PrintLog($"GetRankData errorCode: {errorCode}, errorMessage: {errorMessage}, rankDatas:");
                        for (int i = 0; i < rankDatas.Length; i++)
                        {
                            var rankData = rankDatas[i];
                            PrintLog($"[{i}]\nRank: {rankData.Rank}");
                            PrintLog($"Score: {rankData.Score}");
                            PrintLog($"AppId: {rankData.AppId}");
                            PrintLog($"AvatarUrl: {rankData.AvatarUrl}");
                            PrintLog($"Nickname: {rankData.Nickname}");
                            PrintLog($"RankId: {rankData.RankId}");
                            PrintLog($"OpenId: {rankData.OpenId}");
                            PrintLog($"TimeType: {rankData.TimeType}");
                        }
                    }
                });
            });
            GetRankWithOpenIdsBtn.onClick.AddListener(()=>
            {
                StarkLiveSDK.API.GetRankDataWithUserIds(Convert.ToInt32(RankId.text), (RankTimeType)(SelectRankTimetype.value + 1), RankOpenId.text.Split(',')
                    , (errorCode, errorMessage, rankDatas) =>
                    {
                        if (errorCode != 0)
                        {
                            PrintLog($"GetRankWithOpenIds errorCode: {errorCode}, errorMessage: {errorMessage}");
                        }
                        else
                        {
                            PrintLog($"GetRankWithOpenIds errorCode: {errorCode}, errorMessage: {errorMessage}, rankDatas:");
                            for (int i = 0; i < rankDatas.Length; i++)
                            {
                                var rankData = rankDatas[i];
                                PrintLog($"[{i}]\nRank: {rankData.Rank}");
                                PrintLog($"Score: {rankData.Score}");
                                PrintLog($"AppId: {rankData.AppId}");
                                PrintLog($"AvatarUrl: {rankData.AvatarUrl}");
                                PrintLog($"Nickname: {rankData.Nickname}");
                                PrintLog($"RankId: {rankData.RankId}");
                                PrintLog($"OpenId: {rankData.OpenId}");
                                PrintLog($"TimeType: {rankData.TimeType}");
                                PrintLog($"UserSecUid: {rankData.UserSecUid}");
                            }
                        }
                    });
            });
            PrintRequestRecordsBtn.onClick.AddListener(() =>
            {
                NetworkHelper.Instance.PrintRequestRecords();
            });
        }

        private void SaveLikeRankData(string openId, int num)
        {
            likeRankDataList.Add(new WriteRankData()
            {
                ChangeValue = num,
                OpenId = openId
            });
        }

        private List<WriteRankData> GetLikeRankList()
        {
            // 1.use event data test
            for (int i = 0; i < likeRankDataList.Count; i++)
            {
                PrintLog($"GetLikeRankList list[{i}] ChangeValue: {likeRankDataList[i].ChangeValue}, OpenId: {likeRankDataList[i].OpenId}");
            }
            
            // if (likeRankDataList.Count == 0)
            // {
            //     likeRankDataList.Add(new WriteRankData()
            //     {
            //         ChangeValue = 10000,
            //         OpenId = "clientMockWriteRankData"
            //     });
            //     PrintLog($"GetLikeRankList useMockData ChangeValue: 99999, ExtraData: {DateTime.Now}, OpenId: clientMockWriteRankData");
            // }
            
            // 2.use json data test
            // string filePath = Path.Combine(Application.streamingAssetsPath, "setrankdata.json");
            // if (File.Exists(filePath))
            // {
            //     string fileContent = File.ReadAllText(filePath);
            //     WriteRankDataArray json = JsonUtility.FromJson<WriteRankDataArray>(fileContent);
            //     // WriteRankData[] array = JsonConvert.DeserializeObject<WriteRankData[]>(fileContent);
            //     return json.data?.ToList();
            // }
            // else
            // {
            //     Debug.LogError("File not found: " + filePath);
            // }
            
            return likeRankDataList;
        }

        private string[] GetSelectGifts()
        {
            return selectTopGiftIds.ToArray();
        }

        private void LogFailedGiftInfo(QueryFailedGiftPageData data)
        {
            PrintLog($"LogFailedGiftInfo page_num: {data.PageNum}, total_count: {data.TotalCount}");
            QueryFailedGiftData[] dataArray = data.FailedGiftDataArray;
            for (int i = 0; i < dataArray.Length; i++)
            {
                var item = dataArray[i];
                PrintLog($"roomid: {item.RoomId}");
                var gifts = dataArray[i].GetQueryFailedGifts();
                for (int j = 0; j < gifts.Length; j++)
                {
                    var info = gifts[j];
                    PrintLog($" -------dataArray[{i}]----gifts[{j}]--------------");
                    PrintLog($"info avatar_url: {info.AvatarUrl}");
                    PrintLog($"info gift_num: {info.Num}");
                    PrintLog($"info gift_value: {info.Value}");
                    PrintLog($"info nickname: {info.NickName}");
                    PrintLog($"info msg_id: {info.MsgId}");
                    PrintLog($"info sec_gift_id: {info.GiftId}");
                    PrintLog($"info sec_openid: {info.OpenId}");
                    PrintLog($"info timestamp: {info.Timestamp}");
                }
            }
        }
        
        private void OnStartCommentTaskCallback(int errorCode, string errorMessage, object data)
        {
            if (errorCode == 0)
            {
                PrintLog($"start comment task success");
            }
            else
            {
                PrintLog($"start comment task failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }

        private void OnStartLikeTaskCallback(int errorCode, string errorMessage, object data)
        {
            if (errorCode == 0)
            {
                PrintLog($"start like task success");
            }
            else
            {
                PrintLog($"start like task failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }

        private void OnStartGiftTaskCallback(int errorCode, string errorMessage, object data)
        {
            if (errorCode == 0)
            {
                PrintLog($"start gift task success");
                StarkLiveSDK.API.SetGiftTop(GetSelectGifts(), OnSetGiftTop);
            }
            else
            {
                PrintLog($"start gift task failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }

        private void OnSetGiftTop(int errorCode, string errorMessage, string[] topGifts)
        {
            if (errorCode == 0)
            {
                PrintLog($"set gift top success. topGifts.Length: {topGifts.Length}");
                for (int i = 0; i < topGifts.Length; i++)
                {
                    PrintLog($"topGifts[{i}]: {topGifts[i]}");
                }
            }
            else
            {
                PrintLog($"set gift top failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }

        private void OnQueryTaskStatusCallback(string type, int errorCode, string errorMessage, LiveTaskState state)
        {
            if (errorCode == 0)
            {
                PrintLog($"query {type} task status. state: {Enum.GetName(typeof(LiveTaskState), state)}");
            }
            else
            {
                PrintLog($"query {type} task status failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }
        
        private void OnEndTaskCallback(string type, int errorCode, string errorMessage, object obj)
        {
            if (errorCode == 0)
            {
                PrintLog($"end {type} task success. obj: {obj}");
            }
            else
            {
                PrintLog($"end {type} task failed: errorCode: {errorCode}, errorMessage: {errorMessage}");
            }
        }


        LiveEventType GetCurSelectEventType()
        {
            var toggle = MsgTypeToggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle == null)
            {
                AddLog($"[{TAG}] cannot find active toggle");
                return LiveEventType.GIFT;
            }

            string select = toggle.gameObject.name;
            switch (select)
            {
                case "Comment": return LiveEventType.COMMENT;
                case "Like": return LiveEventType.LIKE;
                case "Gift": return LiveEventType.GIFT;
                default:
                    return LiveEventType.GIFT;
            }
        }

        void SetInitConfig()
        {
            if (!string.IsNullOrEmpty(IPPortInputField.text))
            {
                string[] array = IPPortInputField.text.Split(':');
                StarkLiveSDK.SetInitConfig(array[0], Convert.ToInt32(array[1]));
                return;
            }
            
            var toggle = IPPortToggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle == null)
            {
                AddLog($"[{TAG}] SetInitConfig cannot find active toggle");
                return;
            }

            string select = toggle.gameObject.name;
            switch (select)
            {
                case "1":
                    StarkLiveSDK.SetInitConfig(Consts.GAME_LIVE_SERVER, Consts.GAME_LIVE_PORT);
                    break;
            }
        }

        private void PrintLog(string log)
        {
            StarkLiveSDK.PrintLog(TAG, log, LogColor);
        }

        private void AddLog(string inLog)
        {
            if (!TestLogToggle.isOn && inLog.Contains("[Test]")
                || !LiveObjLogToggle.isOn && inLog.Contains("[LiveObj]")
                || !APILogToggle.isOn && inLog.Contains("[StarkLiveAPI"))
            {
                return;
            }
            if (log.Length > 10000)
                log = string.Empty;
            log += inLog + "\n";
            SetLog(log);
        }

        private void SetLog(string inLog)
        {
            LogText.text = inLog;
        }

        private void ClearLog()
        {
            log = string.Empty;
            SetLog(log);
        }

        private string GetAppId()
        {
            // if (string.IsNullOrEmpty(AppIdInputField.text))
            // {
            //     return Consts.APP_ID;
            // }

            return AppIdInputField.text;
        }
    }