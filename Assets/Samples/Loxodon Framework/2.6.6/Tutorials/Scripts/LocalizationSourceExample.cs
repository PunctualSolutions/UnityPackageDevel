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

using Loxodon.Framework.Localizations;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class LocalizationSourceExample : MonoBehaviour
    {
        public Dropdown dropdown;

        Localization localization;

        void Awake()
        {
            localization             = Localization.Current;
            localization.CultureInfo = new("en-CA");
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));

            dropdown.value = 0;
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        void OnValueChanged(int value)
        {
            switch (value)
            {
                case 0:
                    localization.CultureInfo = new("en-CA");
                    break;
                case 1:
                    localization.CultureInfo = new("zh-CN");
                    break;
                case 2:
                    localization.CultureInfo = new("ko-KR");
                    break;
                case 3:
                    localization.CultureInfo = new("ja-JP");
                    break;
                default:
                    localization.CultureInfo = new("zh-CN");
                    break;
            }
        }

        void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}