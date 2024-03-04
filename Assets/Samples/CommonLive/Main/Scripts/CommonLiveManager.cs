using System;
using System.IO;
using NativeWebSocket;
using UnityEngine;
using ZhengDianWaiBao.Tool;

namespace ZhengDianWaiBao.CommonLive.Sample
{
    public class CommonLiveManager : MonoBehaviour
    {
        private LiveManager _liveManager;

        private void OnDestroy()
        {
            _liveManager.Close();
        }

        private async void Start()
        {
            var config = (await File.ReadAllTextAsync(Path.Combine(CommonDirectoryTool.GetConfig(), "CommonLive.json")))
                .DeserializeObject<LiveConfig>();
            _liveManager = new(config.AccessKeySecret, config.AccessKeyId, config.Code, config.AppId);
            var initData = await _liveManager.Init();
            if (!initData.Successes)
            {
                print($"link fail:{initData.ErrorMessage}");
                return;
            }

            print("link successes");
            Print();
            return;

            void Print()
            {
                GuardBuy();
                Gift();
                Commentaries();
                AdvancedComments();
                return;

                async void GuardBuy()
                {
                    while (_liveManager.State == WebSocketState.Connecting)
                    {
                        var buy = await _liveManager.WaitGuardBuy();
                        print($"guard buy:{buy.userInfo.uid}");
                    }
                }

                async void Gift()
                {
                    while (_liveManager.State == WebSocketState.Connecting)
                    {
                        var gift = await _liveManager.WaitGift();
                        print($"gift: userid:{gift.UserId},name:{gift.Name},number:{gift.Number}");
                    }
                }

                async void Commentaries()
                {
                    while (_liveManager.State == WebSocketState.Connecting)
                    {
                        var commentaries = await _liveManager.WaitCommentaries();
                        print($"commentaries: userid:{commentaries.UserId},content:{commentaries.Content}");
                    }
                }

                async void AdvancedComments()
                {
                    while (_liveManager.State == WebSocketState.Connecting)
                    {
                        var comments = await _liveManager.WaitAdvancedComments();
                        print($"commentaries: userid:{comments.UserId},content:{comments.Message}");
                    }
                }
            }
        }
    }
}