using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PunctualSolutionsTool.Tool;
using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    public class TaskTest : MonoBehaviour
    {
        [SerializeField] BoxCollider _box1;
        [SerializeField] BoxCollider _playerBox;
        [SerializeField] BoxCollider _box2;

        event Action GoBox1;
        event Action GoBox2;

        bool inBox1ing;
        bool inBox2ing;

        void Start()
        {
            // GoBox1 += () => print("go box1");
            // GoBox2 += () => print("go box2");

            // 进入box1
            // 进入box2
            // 进入box1
            // 进入box1,游戏结束

            // GoBox1 += () =>
            // {
            //     print("go box1");
            //     GoBox2 += () =>
            //     {
            //         print("go box2");
            //         GoBox1 += () =>
            //         {
            //             print("go box1");
            //             GoBox1 += () => { print("go box1 ,game end"); };
            //         };
            //     };
            // };

            // GoBox1 += OnGoBox1;
            In().Forget();
            return;

            async UniTask In()
            {
                await WaitGoBox1();
                print("go box1");
                await WaitGoBox2();
                print("go box2");
                await WaitGoBox1();
                print("go box1");
                await WaitGoBox1();
                print("go box1,end game");
            }
        }

        void OnGoBox1()
        {
            print("go box1");
            GoBox1 -= OnGoBox1;
            GoBox2 += OnGoBox2;
        }

        void OnGoBox2()
        {
            print("go box2");
            GoBox1 += OnAction;
            GoBox2 -= OnGoBox2;
        }

        void OnAction()
        {
            print("go box1");
            GoBox1 += OnAction1;
            GoBox1 -= OnAction;
        }

        void OnAction1()
        {
            print("go box1 ,game end");
            GoBox1 -= OnAction1;
        }

        Task WaitGoBox1() => TaskConvertTool.WaitTask(x => GoBox1 += x, x => GoBox1 -= x);
        Task WaitGoBox2() => TaskConvertTool.WaitTask(x => GoBox2 += x, x => GoBox2 -= x);

        void Update()
        {
            if (!inBox1ing && _box1.FullyInclusive(_playerBox))
            {
                inBox1ing = true;
                GoBox1?.Invoke();
            }
            else if (!_box1.FullyInclusive(_playerBox)) inBox1ing = false;

            if (!inBox2ing && _box2.FullyInclusive(_playerBox))
            {
                inBox2ing = true;
                GoBox2?.Invoke();
            }
            else if (!_box2.FullyInclusive(_playerBox)) inBox2ing = false;
        }
    }
}