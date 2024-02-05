using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarkLive
{
    /// <summary>
    /// 通用Unity单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StarkSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        GameObject go = new GameObject();
                        instance = go.AddComponent<T>();
                        go.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(go);
                    }
                }

                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return instance != null;
            }
        }
    }
}