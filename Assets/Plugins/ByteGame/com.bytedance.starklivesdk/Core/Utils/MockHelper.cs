using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarkNetwork;
using StarkNetwork.Stark_Network.Scripts.RPC;
using UnityEngine;

namespace StarkLive
{
    public class MockHelper : StarkSingletonBehaviour<MockHelper>
    {
        private static readonly string LogColor = "cyan";
        private static readonly string TAG = "MockHelper";
        
        private void PrintLog(string log)
        {
            StarkLiveSDK.PrintLog(TAG, log, LogColor);
        }

        public void MockInitRpc(Action action)
        {
            StartCoroutine(MockInitRpcCor(action));
        }
        IEnumerator MockInitRpcCor(Action action)
        {
            yield return null;
            action();
        }

        private Dictionary<LiveEventType, IEnumerator> mockRoomEvent = new Dictionary<LiveEventType, IEnumerator>()
        {
            [LiveEventType.GIFT] = null,
            [LiveEventType.LIKE] = null,
            [LiveEventType.COMMENT] = null
        };
        public void MockRoomEventStart(LiveEventType type, Action action)
        {
            if (mockRoomEvent[type] != null)
            {
                StopCoroutine(mockRoomEvent[type]);
            }
            mockRoomEvent[type] = MockRoomEventCor(action);
            StartCoroutine(mockRoomEvent[type]);
        }

        public void MockRoomEventEnd(LiveEventType type)
        {
            if (mockRoomEvent[type] != null)
            {
                StopCoroutine(mockRoomEvent[type]);
                mockRoomEvent[type] = null;
            }
        }
        IEnumerator MockRoomEventCor(Action action)
        {
            while (true)
            {
                yield return new WaitForSeconds(3);
                action();
            }
        }
        
    }
}