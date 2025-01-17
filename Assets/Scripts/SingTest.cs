using PunctualSolutions.Tool.Singleton;
using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    [SingletonMono(allowAutoDestroy: true)]
    public partial class SingTest : MonoBehaviour
    {
        public void OnSingletonInit()
        {
            Debug.Log(Instance);
        }

        public void Dispose()
        {
        }

        private static void InAwake()
        {
        }
    }
}