using System;
using System.Collections;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using StarkMatchmaking;
using UnityEngine;
namespace StarkNetwork.Stark_Network.Scripts.RPC
{
    public static class Rpc
    {
        internal static bool PushValToRpcParam(object arg,SWIGTYPE_p_bg__cs_rpc__IBGRpcParams param)
        {
            var name = arg.GetType().Name;
            var x = nameof(System.Int32);
            bool result = false;
            switch (name)
            {
                case nameof(System.Int32):
                    result = stark_matchmaking.PushIntToBGRpcParam(param, (int)arg);
                    break;
                case nameof(System.Int64):
                    result = stark_matchmaking.PushInt64ToBGRpcParam(param, (long)arg);
                    break;
                case nameof(System.UInt64):
                    result = stark_matchmaking.PushUInt64ToBGRpcParam(param, (ulong)arg);
                    break;
                case nameof(System.Double):
                    result = stark_matchmaking.PushDoubleToBGRpcParam(param, (double)arg);
                    break;
                case nameof(System.Single):
                    result = stark_matchmaking.PushDoubleToBGRpcParam(param, (float)arg);
                    break;
                case nameof(System.String):
                    result = stark_matchmaking.PushStringToBGRpcParam(param, (string)arg);
                    break;
                case nameof(System.Boolean):
                    result = stark_matchmaking.PushBooleanToBGRpcParam(param, (bool)arg);
                    break;
            }
            if (result == false)
                Debug.Log("push rpc param invalid "+ arg.GetType() + " " + x);
            return result;
        }

        public static bool CallServerMethod(string method, params object[] args)
        {
            int argNum = args.Length;
            var rpcParam = stark_matchmaking.GetBGRpcParam();
            for(int i=0; i<argNum; ++i)
            {
               var ret = PushValToRpcParam(args[i], rpcParam);
               if (!ret)
               {
                   Debug.LogError(("push 出错"+i));
               }
            }
            stark_matchmaking.CallServerMethod(method, rpcParam);
            return true;
        }

        public static bool CallTargetServerMethod(string method, uint targetHashId, ulong targetId, params object[] args)
        {
            int argNum = args.Length;
            var rpcParam = stark_matchmaking.GetBGRpcParam();
            for(int i=0; i<argNum; ++i)
            {
                var ret = PushValToRpcParam(args[i], rpcParam);
                if (!ret)
                {
                    Debug.LogError(("push 出错"+i));
                }
            }
            stark_matchmaking.CallTargetServerMethod(targetHashId, targetId, method, rpcParam);
            return true;
        }

        public static void RegisterCustomSimpleMethod(string method, string[] paramList)
        {
            Debug.Log($"RegisterCustomSimpleMethod method: {method}");
            Vec_string vecCString = new Vec_string();
            for (int i = 0; i < paramList.Length; i++)
            {
                vecCString.Add(paramList[i]);
                Debug.Log($"RegisterCustomSimpleMethod paramList[{i}]: {paramList[i]}");
            }
            stark_matchmaking.RegisterCustomSimpleMethod(method, vecCString);
        }

        private static RpcCallContext _rpcCallContext = null;
        public static RpcCallContext GetRpcCallContext()
        {
            return _rpcCallContext;
        }
        public static void OnServerRpcCall(SerializedRpcCallInfo callInfo)
        {
            Debug.Log("onRpcServerCall methodName"+callInfo.methodName + " "+ callInfo.paramTypes.Count);
            ArrayList arr = new ArrayList();
            for (int i = 0; i < callInfo.paramTypes.Count; i++)
            {
                Debug.Log("PARAM TYPES = " + callInfo.paramTypes[i]);
                switch (callInfo.paramTypes[i])
                {
                    case "int":
                        arr.Add(callInfo.intArr[0]);
                        callInfo.intArr.RemoveAt(0);
                        break;
                    case "boolean":
                        arr.Add(callInfo.boolArr[0]);
                        callInfo.boolArr.RemoveAt(0);
                        break;
                    case "double":
                        arr.Add(callInfo.doubleArr[0]);
                        callInfo.boolArr.RemoveAt(0);
                        break;
                    case "string":
                        arr.Add(callInfo.strArr[0]);
                        callInfo.strArr.RemoveAt(0);
                        break;
                    case "uint":
                        arr.Add(callInfo.uint32Arr[0]);
                        callInfo.uint32Arr.RemoveAt(0);
                        break;
                    case "uint64":
                        arr.Add(callInfo.uint64_arr[0]);
                        callInfo.uint64_arr.RemoveAt(0);
                        break;
                }
            }
            var founded = false;
            foreach (var methodObj in RpcRegister.GetRegisterMethodObjs())
            {
                _rpcCallContext = callInfo.context;
                // Debug.Log();
                var tgtMethod = methodObj.GetType().GetMethod(callInfo.methodName);
                if (tgtMethod!=null)
                {
                    tgtMethod.Invoke(methodObj, arr.ToArray());
                    founded = true;
                }
            }
            if (!founded)
            {
                Debug.LogWarning($"OnServerRpcCall, method {callInfo.methodName} can not be found");
            }
        }
    }
}