using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarkNetwork
{
    internal class SafeCaller
    {
        private static readonly SafeCaller _instance = new SafeCaller();

        public static void Call(Action function, bool isPenetrating = false)
        {
            _instance.CallFunc(function, isPenetrating);
        }
        
        private List<Exception> _callbackExceptions = new List<Exception>();

        private SafeCaller()
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="isPenetrating">是否抛出异常</param>
        private void CallFunc(Action function, bool isPenetrating)
        {
            try
            {
                function.Invoke();
            }
            catch (Exception e)
            {
                _callbackExceptions.Add(e);
                Debug.LogException(e);
                if (isPenetrating)
                {
                    throw;
                }
            }
        }
        
        
    }
}