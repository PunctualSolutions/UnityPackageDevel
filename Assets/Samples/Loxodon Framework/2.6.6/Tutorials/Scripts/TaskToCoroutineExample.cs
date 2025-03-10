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
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
using Loxodon.Framework.Asynchronous;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Loxodon.Framework.Tutorials
{
    public class TaskToCoroutineExample : MonoBehaviour
    {
#if NETFX_CORE || NET_STANDARD_2_0 || NET_4_6
        IEnumerator Start()
        {
            var task = Task.Run(() =>
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        Thread.Sleep(200);
                    }
                    catch (Exception)
                    {
                    }

                    Debug.LogFormat("Task ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
                }
            });

            yield return task.AsCoroutine();
            Debug.LogFormat("Task End,Current Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);

            yield return Task.Delay(1000).AsCoroutine();
            Debug.LogFormat("Delay End");
        }
#endif
    }
}