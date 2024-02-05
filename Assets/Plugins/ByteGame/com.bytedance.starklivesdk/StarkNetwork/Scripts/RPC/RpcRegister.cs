using System.Collections.Generic;
using UnityEngine;

namespace StarkNetwork.Stark_Network.Scripts.RPC
{
    public class RpcRegister
    {
        private static readonly  List<object> methodList = new List<object>();

        public static void  RegisterMethodObj(RpcBase obj)
        {
            if (methodList.Contains(obj))
            {
                Debug.LogWarning("同一个RPC对象注册两次，如果不符合预期,请检查注册逻辑");
            }
            methodList.Add(obj);
        }

        public static void UnRegisterMethodObj(RpcBase obj)
        {
            if (methodList.Contains(obj))
            {
                methodList.Remove(obj);
            }
        }

        public static List<object> GetRegisterMethodObjs()
        {
            return methodList;
        }
    }
}