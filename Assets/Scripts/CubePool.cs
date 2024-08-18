using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace PunctualSolutions.UnityPackageDevel
{
    public class CubePool : Tool.Pool.ObjectPool<Cube>
    {
        event Func<UniTask<Cube>> GetCube;

        public CubePool([NotNull] Func<UniTask<Cube>> getCube) : base(10) => GetCube = getCube;

        protected override UniTask<Cube> OnCreate() => GetCube!.Invoke();

        protected override UniTask OnGet(Cube @object)
        {
            @object.gameObject.SetActive(true);
            return UniTask.NextFrame();
        }

        protected override UniTask OnRelease(Cube @object)
        {
            @object.ClearColor();
            @object.gameObject.SetActive(false);
            return UniTask.NextFrame();
        }

        protected override UniTask OnDestroy(Cube @object)
        {
            Object.Destroy(@object);
            return UniTask.NextFrame();
        }
    }
}