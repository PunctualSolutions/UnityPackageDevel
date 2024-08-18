using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using PunctualSolutions.Tool.Addressables;
using PunctualSolutionsTool.Tool;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PunctualSolutions.UnityPackageDevel
{
    public class Test : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI           text;
        CubePool                                   _cubePool;
        [SerializeField] AssetReferenceGameObject  _referenceCube;
        [SerializeField] AssetReferenceTexture2D[] _referenceTexture2DList;

        void Start()
        {
            GameObject g = null;
            print(g.layer);
            In().Forget();
            return;

            async UniTaskVoid In()
            {
                _cubePool = new(GetCube);
                List<Cube> cubes = new();
                await CreateCube(10);
                await 5.Delay();
                foreach (var cube in cubes)
                    _cubePool.Release(cube).Forget();
                cubes.Clear();
                await 5.Delay();
                await CreateCube(50);
                await 5.Delay();
                foreach (var cube in cubes)
                    _cubePool.Release(cube).Forget();
                return;

                async UniTask CreateCube(int number)
                {
                    for (var i = 0; i < number; i++)
                    {
                        var cube = await _cubePool.Get();
                        cube.Set();
                        cubes.Add(cube);
                    }
                }
            }

            async UniTask<Cube> GetCube() => (await InstantiateAsync(await _referenceCube.Get()))[0].GetComponent<Cube>();

            // 重复加载
            async UniTask Bad1()
            {
                var asyncOperationHandle  = await _referenceTexture2DList[0].LoadAssetAsync();
                var asyncOperationHandle1 = await _referenceTexture2DList[0].LoadAssetAsync();
            }

            async UniTask God1()
            {
                await _referenceTexture2DList[0].Get();
                await _referenceTexture2DList[0].Get();
            }

            // 异步加载多个卡顿
            async UniTask Bad2()
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                foreach (var assetReferenceTexture2D in _referenceTexture2DList) assetReferenceTexture2D.LoadAssetAsync();
                stopwatch.Stop();
                print($"bad:{stopwatch.ElapsedMilliseconds}");
            }

            async UniTask God2()
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                foreach (var assetReferenceTexture2D in _referenceTexture2DList) assetReferenceTexture2D.Get().Forget();
                stopwatch.Stop();
                print($"bad:{stopwatch.ElapsedMilliseconds}");
            }
        }
    }
}