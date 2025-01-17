/*
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

using Loxodon.Framework.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loxodon.Framework.Examples
{
    public class MixedObjectPoolExample : MonoBehaviour
    {
        public GameObject template;

        IMixedObjectPool<GameObject>         pool;
        Dictionary<string, List<GameObject>> dict;

        void Start()
        {
            var factory = new CubeMixedObjectFactory(template, transform);
            pool = new MixedObjectPool<GameObject>(factory, 5);
            dict = new();
        }

        void OnDestroy()
        {
            if (pool != null)
            {
                pool.Dispose();
                pool = null;
            }
        }

        void OnGUI()
        {
            var x       = 50;
            var y       = 50;
            var width   = 100;
            var height  = 60;
            var i       = 0;
            var padding = 10;

            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(red)")) Add("red",     1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(green)")) Add("green", 1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(blue)")) Add("blue",   1);

            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(red)")) Delete("red",     1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(green)")) Delete("green", 1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(blue)")) Delete("blue",   1);
        }

        protected void Add(string typeName, int count)
        {
            List<GameObject> list;
            if (!dict.TryGetValue(typeName, out list))
            {
                list = new();
                dict.Add(typeName, list);
            }

            for (var i = 0; i < count; i++)
            {
                var go = pool.Allocate(typeName);
                go.transform.position = GetPosition();
                go.name               = string.Format("Cube {0}-{1}", typeName, list.Count);
                go.SetActive(true);
                list.Add(go);
            }
        }

        protected void Delete(string typeName, int count)
        {
            List<GameObject> list;
            if (!dict.TryGetValue(typeName, out list) || list.Count <= 0)
                return;

            for (var i = 0; i < count; i++)
            {
                if (list.Count <= 0)
                    return;

                var index = list.Count - 1;
                var go    = list[index];
                list.RemoveAt(index);

                //this.pool.Free(go);
                //or
                var freeable = go.GetComponent<IPooledObject>();
                freeable.Free();
            }
        }

        protected Vector3 GetPosition()
        {
            float x = UnityEngine.Random.Range(-10, 10);
            float y = UnityEngine.Random.Range(-5,  5);
            float z = UnityEngine.Random.Range(-10, 10);
            return new(x, y, z);
        }

        public class CubeMixedObjectFactory : UnityMixedGameObjectFactoryBase
        {
            GameObject template;
            Transform  parent;

            public CubeMixedObjectFactory(GameObject template, Transform parent)
            {
                this.template = template;
                this.parent   = parent;
            }

            protected override GameObject Create(string typeName)
            {
                Debug.LogFormat("Create a cube.");
                var go = Instantiate(template, parent);
                go.GetComponent<MeshRenderer>().material.color = GetColor(typeName);
                return go;
            }

            protected Color GetColor(string typeName)
            {
                if (typeName.Equals("red"))
                    return Color.red;
                if (typeName.Equals("green"))
                    return Color.green;
                if (typeName.Equals("blue"))
                    return Color.blue;

                throw new NotSupportedException("Unsupported type:" + typeName);
            }

            public override void Reset(string typeName, GameObject obj)
            {
                obj.SetActive(false);
                obj.name               = string.Format("Cube {0}-Idle", typeName);
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            public override void Destroy(string typeName, GameObject obj)
            {
                base.Destroy(typeName, obj);
                Debug.LogFormat("Destroy a cube.");
            }
        }
    }
}