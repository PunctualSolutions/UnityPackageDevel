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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class LocalizedDataBinderExample : MonoBehaviour
    {
        public Dropdown dropdown;
        Localization    localization;

        void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            localization = Localization.Current;
            var cultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
            //this.localization.AddDataProvider(new DefaultLocalizationSourceDataProvider("LocalizationTutorials", "LocalizationModule.asset"));

            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        public void OnValueChanged(int value)
        {
            switch (value)
            {
                case 0:
                    localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
                    break;
                case 1:
                    localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.ChineseSimplified);
                    break;
                default:
                    localization.CultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
                    break;
            }
        }
    }
}