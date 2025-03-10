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

using System.Collections.Specialized;
using UnityEngine;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Tutorials
{
    public class Item : ObservableObject
    {
        string title;
        string iconPath;
        string content;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string IconPath
        {
            get => iconPath;
            set => Set(ref iconPath, value);
        }

        public string Content
        {
            get => content;
            set => Set(ref content, value);
        }

        public override string ToString() => string.Format("[Item: Title={0}, IconPath={1}, Content={2}]", Title, IconPath, Content);
    }

    public class ObservableListExample : MonoBehaviour
    {
        ObservableList<Item> list;

        protected void Start()
        {
            list                   =  new();
            list.CollectionChanged += OnCollectionChanged;

            list.Add(new() { Title = "title1", IconPath = "xxx/xxx/icon1.png", Content = "this is a test.", });
            list[0] = new() { Title = "title2", IconPath = "xxx/xxx/icon2.png", Content = "this is a test.", };

            list.Clear();
        }

        protected void OnDestroy()
        {
            if (list != null)
            {
                list.CollectionChanged -= OnCollectionChanged;
                list                   =  null;
            }
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Item item in eventArgs.NewItems) Debug.LogFormat("ADD item:{0}", item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Item item in eventArgs.OldItems) Debug.LogFormat("REMOVE item:{0}", item);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (Item item in eventArgs.OldItems) Debug.LogFormat("REPLACE before item:{0}", item);
                    foreach (Item item in eventArgs.NewItems) Debug.LogFormat("REPLACE after item:{0}",  item);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug.LogFormat("RESET");
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }
    }
}