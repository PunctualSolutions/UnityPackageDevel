﻿/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Text;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials
{
    public class CoroutineExecutorExample : MonoBehaviour
    {
        ICoroutineExecutor executor;

        IEnumerator Start()
        {
            executor = new CoroutineExecutor();

            var r1 = executor.RunOnCoroutine(Task1());
            yield return r1.WaitForDone();

            var r2 = executor.RunOnCoroutine(promise => Task2(promise));
            yield return r2.WaitForDone();

            var r3 = executor.RunOnCoroutine<string>(promise => Task3(promise));
            yield return new WaitForSeconds(0.5f);
            r3.Cancel();
            yield return r3.WaitForDone();
            Debug.LogFormat("Task3 IsCalcelled:{0}", r3.IsCancelled);

            var r4 = executor.RunOnCoroutine<float, string>(promise => Task4(promise));
            while (!r4.IsDone)
            {
                yield return null;
                Debug.LogFormat("Task4 Progress:{0}%", Mathf.FloorToInt(r4.Progress * 100));
            }

            Debug.LogFormat("Task4 Result:{0}", r4.Result);
        }

        IEnumerator Task1()
        {
            Debug.Log("The task1 start");
            yield return null;
            Debug.Log("The task1 end");
        }

        IEnumerator Task2(IPromise promise)
        {
            Debug.Log("The task2 start");
            yield return null;
            promise.SetResult(); /* set a result of the task */
            Debug.Log("The task2 end");
        }

        IEnumerator Task3(IPromise<string> promise)
        {
            Debug.Log("The task3 start");
            var buf = new StringBuilder();
            for (var i = 0; i < 50; i++)
            {
                /* If the task is cancelled, then stop the task */
                if (promise.IsCancellationRequested)
                {
                    promise.SetCancelled();
                    yield break;
                }

                buf.Append(i).Append(" ");
                yield return null;
            }

            promise.SetResult(buf.ToString()); /* set a result of the task */
            Debug.Log("The task3 end");
        }

        IEnumerator Task4(IProgressPromise<float, string> promise)
        {
            Debug.Log("The task4 start");
            var n   = 10;
            var buf = new StringBuilder();
            for (var i = 1; i <= n; i++)
            {
                /* If the task is cancelled, then stop the task */
                if (promise.IsCancellationRequested)
                {
                    promise.SetCancelled();
                    yield break;
                }

                buf.Append(i).Append(" ");

                promise.UpdateProgress(i / (float)n);
                yield return null;
            }

            promise.SetResult(buf.ToString()); /* set a result of the task */
            Debug.Log("The task4 end");
        }
    }
}