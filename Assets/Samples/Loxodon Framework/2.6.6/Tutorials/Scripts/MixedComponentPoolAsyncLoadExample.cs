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
using System.Threading.Tasks;
using UnityEngine;
using Loxodon.Framework.Asynchronous;
using Object = UnityEngine.Object;

namespace Loxodon.Framework.Examples
{
    public class MixedComponentPoolAsyncLoadExample : MonoBehaviour
    {
        IMixedObjectPool<PooledCube>         pool;
        Dictionary<string, List<PooledCube>> dict;

        void Start()
        {
            /**
             * This is an example of loading an object using an asynchronous method.
             */
            var factory = new CubeMixedObjectFactory(transform);
            pool = new MixedObjectPool<PooledCube>(factory, 5);
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

            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(red)")) _   = Add("red",   1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(green)")) _ = Add("green", 1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Add(blue)")) _  = Add("blue",  1);

            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(red)")) Delete("red",     1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(green)")) Delete("green", 1);
            if (GUI.Button(new(x, y + i++ * (height + padding), width, height), "Delete(blue)")) Delete("blue",   1);
        }

        protected async Task Add(string typeName, int count)
        {
            List<PooledCube> list;
            if (!dict.TryGetValue(typeName, out list))
            {
                list = new();
                dict.Add(typeName, list);
            }

            for (var i = 0; i < count; i++)
            {
                var cube = pool.Allocate(typeName);
                if (!cube.IsLoaded)
                    await cube.LoadAsync();

                cube.gameObject.transform.position = GetPosition();
                cube.gameObject.name               = string.Format("Cube {0}-{1}", typeName, list.Count);
                cube.gameObject.SetActive(true);
                list.Add(cube);
            }
        }

        protected void Delete(string typeName, int count)
        {
            List<PooledCube> list;
            if (!dict.TryGetValue(typeName, out list) || list.Count <= 0)
                return;

            for (var i = 0; i < count; i++)
            {
                if (list.Count <= 0)
                    return;

                var index = list.Count - 1;
                var cube  = list[index];
                list.RemoveAt(index);

                //this.pool.Free(cube);
                //or
                cube.Free();
            }
        }

        protected Vector3 GetPosition()
        {
            float x = UnityEngine.Random.Range(-10, 10);
            float y = UnityEngine.Random.Range(-5,  5);
            float z = UnityEngine.Random.Range(-10, 10);
            return new(x, y, z);
        }

        public class PooledCube : MonoBehaviour, IPooledObject
        {
            public Color                        color;
            public IMixedObjectPool<PooledCube> pool;
            public string                       typeName;
            public bool                         IsLoaded = false;
            GameObject                          child;

            public async Task<GameObject> LoadAsync()
            {
                if (IsLoaded)
                    return child;

                var go = (GameObject)await Resources.LoadAsync<GameObject>("Cube/Cube");
                child                                             = Instantiate(go, transform);
                child.GetComponent<MeshRenderer>().material.color = color;
                IsLoaded                                          = true;
                return child;
            }

            public void Free()
            {
                if (pool != null)
                    pool.Free(typeName, this);
            }
        }

        public class CubeMixedObjectFactory : IMixedObjectFactory<PooledCube>
        {
            Transform parent;
            public CubeMixedObjectFactory(Transform parent) => this.parent = parent;

            public PooledCube Create(IMixedObjectPool<PooledCube> pool, string typeName)
            {
                Debug.LogFormat("Create a cube.");
                var go = new GameObject(string.Format("Cube {0}-Idle", typeName)); // GameObject.Instantiate(this.template, parent);
                go.transform.SetParent(parent);
                var cube = go.AddComponent<PooledCube>();
                cube.pool     = pool;
                cube.typeName = typeName;
                cube.color    = GetColor(typeName);
                return cube;
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

            public virtual void Reset(string typeName, PooledCube obj)
            {
                obj.gameObject.SetActive(false);
                obj.gameObject.name               = string.Format("Cube {0}-Idle", typeName);
                obj.gameObject.transform.position = Vector3.zero;
                obj.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            public virtual void Destroy(string typeName, PooledCube obj)
            {
                Object.Destroy(obj.gameObject);
                Debug.LogFormat("Destroy a cube.");
            }

            public virtual bool Validate(string typeName, PooledCube obj) => true;
        }
    }
}