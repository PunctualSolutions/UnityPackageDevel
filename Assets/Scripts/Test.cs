using System;
using Newtonsoft.Json;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEngine;

public class Test : MonoBehaviour
{
    private WebSocketBLiveClient _client;
    private readonly string _accessKeySecret = "PKhF6NgLMKOZyMijyGD6bKhU";
    private readonly string _accessKeyId = "FQ1us1hwUzIoZs1kGnUZW4qBkmpL9S";
    private readonly string _code = "284815";
    private long _appId;
    private string _gameId;
    private async void LinkStart(string code)
    {
        //测试的密钥
        SignUtility.accessKeySecret = _accessKeySecret;
        //测试的ID
        SignUtility.accessKeyId = _accessKeyId;
        //房间号Code，对应项目Id
        var ret = await BApi.StartInteractivePlay(code, _appId.ToString());
        //打印到控制台日志
        var gameIdResObj = JsonConvert.DeserializeObject<AppStartInfo>(ret);
        //处理开启游戏异常事件
        if (gameIdResObj.Code != 0)
        {
            Debug.LogError(gameIdResObj.Message);
            return;
        }

        //连接长链
        _client = new WebSocketBLiveClient(gameIdResObj.GetWssLink(), gameIdResObj.GetAuthBody());
        _client.OnDanmaku += WebSocketBLiveClientOnDanmaku;
        _client.OnGift += WebSocketBLiveClientOnGift;
        _client.OnGuardBuy += WebSocketBLiveClientOnGuardBuy;
        _client.OnSuperChat += WebSocketBLiveClientOnSuperChat;
        try
        {
            //连接长链  需自己处理重连
            _client.Connect();
            //长链带有重试
            // m_WebSocketBLiveClient.Connect(TimeSpan.FromSeconds(2),10);
            Debug.Log("连接成功");
        }
        catch (Exception ex)
        {
            Debug.Log("连接失败");
            throw;
        }

        //绑定游戏心跳
        var gameId = gameIdResObj.GetGameId();
        var beat = new InteractivePlayHeartBeat(gameId);
        beat.HeartBeatError += json => { };
        beat.HeartBeatSucceed += () => { };
        beat.Start();
    }

    //绑定醒目留言
    private void WebSocketBLiveClientOnSuperChat(SuperChat superChat)
    {
    }

    //绑定大航海信息
    private void WebSocketBLiveClientOnGuardBuy(Guard guard)
    {
    }

    //绑定礼物信息
    private void WebSocketBLiveClientOnGift(SendGift sendGift)
    {
    }

    //绑定弹幕事件
    private void WebSocketBLiveClientOnDanmaku(Dm dm)
    {
    }

    private async void OpenProject()
    {
        //开启并获取项目的场次id，appId需要申请获得
        var ret = await BApi.StartInteractivePlay(_code, _appId.ToString());
        var retObject = JsonConvert.DeserializeObject<AppStartInfo>(ret);
        if (retObject?.Code != 0)
        {
            return;
        }

        _gameId = retObject.Data?.GameInfo?.GameId;
    }

    private async void CloseProject()
    {
        var closeRes = await BApi.EndInteractivePlay(_appId.ToString(), _gameId);
    }
}