using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnvSwitch : MonoBehaviour
    {
        [SerializeField]
        Material[] _materials;

        void Start()
        {
            SwitchEnv(0);
        }

        public void SwitchEnv(int index)
        {
            if (index < 0 || index >= _materials.Length)
            {
                return;
            }
            //HDRP change skybox
            RenderSettings.skybox = _materials[index];
        }
    }
}