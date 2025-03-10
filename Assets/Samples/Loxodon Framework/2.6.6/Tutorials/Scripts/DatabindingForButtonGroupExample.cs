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
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    public class ButtonGroupViewModel : ViewModelBase
    {
        string                         text;
        readonly SimpleCommand<string> click;
        public ButtonGroupViewModel() => click = new(OnClick);

        public string Text
        {
            get => text;
            set => Set(ref text, value);
        }

        public ICommand Click => click;

        public void OnClick(string buttonText)
        {
            Executors.RunOnCoroutineNoReturn(DoClick(buttonText));
        }

        IEnumerator DoClick(string buttonText)
        {
            click.Enabled = false;
            Text          = string.Format("Click Button:{0}.Restore button status after one second", buttonText);
            Debug.LogFormat("Click Button:{0}", buttonText);

            //Restore button status after one second
            yield return new WaitForSeconds(1f);
            click.Enabled = true;
        }
    }

    public class DatabindingForButtonGroupExample : UIView
    {
        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Text   text;

        protected override void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            var cultureInfo  = Locale.GetCultureInfo();
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
        }

        protected override void Start()
        {
            var viewModel = new ButtonGroupViewModel();

            var bindingContext = this.BindingContext();
            bindingContext.DataContext = viewModel;

            /* databinding */
            var bindingSet = this.CreateBindingSet<DatabindingForButtonGroupExample, ButtonGroupViewModel>();
            bindingSet.Bind(button1).For(v => v.onClick).To(vm => vm.Click).CommandParameter(() => button1.name);
            bindingSet.Bind(button2).For(v => v.onClick).To(vm => vm.Click).CommandParameter(() => button2.name);
            bindingSet.Bind(button3).For(v => v.onClick).To(vm => vm.Click).CommandParameter(() => button3.name);
            bindingSet.Bind(button4).For(v => v.onClick).To(vm => vm.Click).CommandParameter(() => button4.name);
            bindingSet.Bind(button5).For(v => v.onClick).To(vm => vm.Click).CommandParameter(() => button5.name);

            bindingSet.Bind(text).For(v => v.text).To(vm => vm.Text).OneWay();

            bindingSet.Build();
        }
    }
}