using System;
using PunctualSolutions.Tool.Area;
using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    public class TestArea : MonoBehaviour
    {
        [SerializeField] AreaBase _areaBase;

        void Start()
        {
            var point = _areaBase.GenerateRandomPoints(1f, 30);
            foreach (var vector3 in point)
                new GameObject("point").transform.position = vector3;
        }
    }
}