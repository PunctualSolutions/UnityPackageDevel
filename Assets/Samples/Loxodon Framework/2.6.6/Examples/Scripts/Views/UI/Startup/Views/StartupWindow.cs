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
using UnityEngine.UI;
using System;
using Loxodon.Log;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views.InteractionActions;
using Loxodon.Framework.Asynchronous;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Loxodon.Framework.Examples
{
    public class StartupWindow : Window
    {
        static readonly ILog log = LogManager.GetLogger(typeof(StartupWindow));

        public Text   progressBarText;
        public Slider progressBarSlider;
        public Text   tipText;
        public Button button;

        StartupViewModel             viewModel;
        IUIViewLocator               viewLocator;
        AsyncWindowInteractionAction loginWindowInteractionAction;
        AsynSceneInteractionAction   sceneInteractionAction;

        protected override void OnCreate(IBundle bundle)
        {
            viewLocator                  = Context.GetApplicationContext().GetService<IUIViewLocator>();
            loginWindowInteractionAction = new("UI/Logins/Login", viewLocator, WindowManager);
            sceneInteractionAction       = new("Prefabs/Cube");
            viewModel                    = new();

            /* databinding, Bound to the ViewModel. */
            var bindingSet = this.CreateBindingSet(viewModel);
            //bindingSet.Bind().For(v => v.OnOpenLoginWindow).To(vm => vm.LoginRequest);
            bindingSet.Bind().For(v => v.loginWindowInteractionAction).To(vm => vm.LoginRequest);
            bindingSet.Bind().For(v => v.OnDismissRequest).To(vm => vm.DismissRequest);
            bindingSet.Bind().For(v => v.sceneInteractionAction).To(vm => vm.LoadSceneRequest);

            bindingSet.Bind(progressBarSlider).For("value", "onValueChanged").To("ProgressBar.Progress").TwoWay();
            //bindingSet.Bind (this.progressBarSlider).For (v => v.value, v => v.onValueChanged).To (vm => vm.ProgressBar.Progress).TwoWay ();

            /* //by the way,You can expand your attributes.
                ProxyFactory proxyFactory = ProxyFactory.Default;
                PropertyInfo info = typeof(GameObject).GetProperty ("activeSelf");
                proxyFactory.Register (new ProxyPropertyInfo<GameObject, bool> (info, go => go.activeSelf, (go, value) => go.SetActive (value)));
            */

            bindingSet.Bind(progressBarSlider.gameObject).For(v => v.activeSelf).To(vm => vm.ProgressBar.Enable).OneWay();
            bindingSet.Bind(progressBarText).For(v => v.text).ToExpression(vm => string.Format("{0}%", Mathf.FloorToInt(vm.ProgressBar.Progress * 100f))).OneWay(); /* expression binding,support only OneWay mode. */
            bindingSet.Bind(tipText).For(v => v.text).To(vm => vm.ProgressBar.Tip).OneWay();

            //bindingSet.Bind(this.button).For(v => v.onClick).To(vm=>vm.OnClick()).OneWay(); //Method binding,only bound to the onClick event.
            bindingSet.Bind(button).For(v => v.onClick).To(vm => vm.Click).OneWay(); //Command binding,bound to the onClick event and interactable property.
            bindingSet.Build();

            viewModel.Unzip();
        }

        protected void OnDismissRequest(object sender, InteractionEventArgs args)
        {
            Dismiss();
        }

        //// Use AsyncWindowInteractionAction instead of this method.
        //protected void OnOpenLoginWindow(object sender, InteractionEventArgs args)
        //{
        //    try
        //    {
        //        LoginWindow loginWindow = viewLocator.LoadWindow<LoginWindow>(this.WindowManager, "UI/Logins/Login");
        //        var callback = args.Callback;
        //        var loginViewModel = args.Context;

        //        if (callback != null)
        //        {
        //            EventHandler handler = null;
        //            handler = (window, e) =>
        //            {
        //                loginWindow.OnDismissed -= handler;
        //                if (callback != null)
        //                    callback();
        //            };
        //            loginWindow.OnDismissed += handler;
        //        }

        //        loginWindow.SetDataContext(loginViewModel);
        //        loginWindow.Create();
        //        loginWindow.Show();
        //    }
        //    catch (Exception e)
        //    {
        //        if (log.IsWarnEnabled)
        //            log.Warn(e);
        //    }
        //}

        //Load game objects in the scene using AsynSceneInteractionAction
        class AsynSceneInteractionAction : AsyncInteractionActionBase<ProgressBar>
        {
            string path;
            public AsynSceneInteractionAction(string path) => this.path = path;

            public override async Task Action(ProgressBar progressBar)
            {
                progressBar.Enable = true;
                progressBar.Tip    = R.startup_progressbar_tip_loading;
                try
                {
                    var request = Resources.LoadAsync<GameObject>(path);
                    while (!request.isDone)
                    {
                        progressBar.Progress = request.progress; /* update progress */
                        await new WaitForSecondsRealtime(0.02f);
                    }

                    var sceneTemplate = (GameObject)request.asset;
                    Instantiate(sceneTemplate);
                }
                finally
                {
                    progressBar.Tip    = "";
                    progressBar.Enable = false;
                }
            }
        }
    }
}