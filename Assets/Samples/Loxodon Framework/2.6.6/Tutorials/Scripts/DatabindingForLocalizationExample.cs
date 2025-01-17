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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class DatabindingForLocalizationViewModel : ViewModelBase
    {
        Localization localization;

        public DatabindingForLocalizationViewModel(Localization localization) => this.localization = localization;

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

    public class DatabindingForLocalizationExample : MonoBehaviour
    {
        public Dropdown dropdown;

        public Text text;

        Localization localization;

        void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            localization = Localization.Current;
            var cultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
            localization.CultureInfo = cultureInfo;
            //this.localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
            localization.AddDataProvider(new DefaultLocalizationSourceDataProvider("LocalizationTutorials", "LocalizationModule.asset"));
        }

        void Start()
        {
            var bindingSet = this.CreateBindingSet(new DatabindingForLocalizationViewModel(localization));
            bindingSet.Bind(dropdown).For(v => v.onValueChanged).To<int>(vm => vm.OnValueChanged);
            bindingSet.Build();

            var staticBindingSet = this.CreateBindingSet();
            //staticBindingSet.Bind(this.text).For(v => v.text).To(() => Res.localization_tutorials_content).OneWay();
            staticBindingSet.Bind(text).For(v => v.text).ToValue(localization.GetValue("localization.tutorials.content")).OneWay();
            staticBindingSet.Build();
        }
    }
}