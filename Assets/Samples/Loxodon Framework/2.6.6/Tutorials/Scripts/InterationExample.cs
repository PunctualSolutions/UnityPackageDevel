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

using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views.InteractionActions;

namespace Loxodon.Framework.Tutorials
{
    public class InterationViewModel : ViewModelBase
    {
        public readonly InteractionRequest<DialogNotification>      AlertDialogRequest      = new();
        public readonly AsyncInteractionRequest<DialogNotification> AsyncAlertDialogRequest = new();
        public readonly InteractionRequest<ToastNotification>       ToastRequest            = new();
        public readonly InteractionRequest<VisibilityNotification>  LoadingRequest          = new();

        public InterationViewModel()
        {
            OpenAlertDialog = new(() =>
            {
                OpenAlertDialog.Enabled = false;

                var notification = new DialogNotification("Interation Example", "This is a dialog test.", "Yes", "No", true);

                Action<DialogNotification> callback = n =>
                {
                    OpenAlertDialog.Enabled = true;

                    if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                        Debug.LogFormat("Click: Yes");
                    else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE) Debug.LogFormat("Click: No");
                };

                AlertDialogRequest.Raise(notification, callback);
            });

            AsyncOpenAlertDialog = new(async () =>
            {
                AsyncOpenAlertDialog.Enabled = false;
                var notification = new DialogNotification("Interation Example", "This is a dialog test.", "Yes", "No", true);
                await AsyncAlertDialogRequest.Raise(notification);
                AsyncOpenAlertDialog.Enabled = true;
                if (notification.DialogResult == AlertDialog.BUTTON_POSITIVE)
                    Debug.LogFormat("Click: Yes");
                else if (notification.DialogResult == AlertDialog.BUTTON_NEGATIVE) Debug.LogFormat("Click: No");
            });

            ShowToast = new(() =>
            {
                var notification = new ToastNotification("This is a toast test.", 2f);
                ToastRequest.Raise(notification);
            });

            ShowLoading = new(() =>
            {
                var notification = new VisibilityNotification(true);
                LoadingRequest.Raise(notification);
            });

            HideLoading = new(() =>
            {
                var notification = new VisibilityNotification(false);
                LoadingRequest.Raise(notification);
            });
        }

        public SimpleCommand OpenAlertDialog      { get; }
        public SimpleCommand AsyncOpenAlertDialog { get; }
        public SimpleCommand ShowToast            { get; }
        public SimpleCommand ShowLoading          { get; }
        public SimpleCommand HideLoading          { get; }
    }

    public class InterationExample : WindowView
    {
        public Button openAlert;
        public Button asyncOpenAlert;
        public Button showToast;
        public Button showLoading;
        public Button hideLoading;

        List<Loading> list = new();

        LoadingInteractionAction     loadingInteractionAction;
        ToastInteractionAction       toastInteractionAction;
        AsyncDialogInteractionAction dialogInteractionAction;

        protected override void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            /* Initialize the ui view locator and register UIViewLocator */
            var container = context.GetContainer();
            container.Register<IUIViewLocator>(new DefaultUIViewLocator());

            var cultureInfo  = Locale.GetCultureInfo();
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
            container.Register(localization);
        }

        protected override void Start()
        {
            loadingInteractionAction = new();
            toastInteractionAction   = new(this);
            dialogInteractionAction  = new("UI/AlertDialog");

            var viewModel = new InterationViewModel();
            this.SetDataContext(viewModel);

            /* databinding */
            var bindingSet = this.CreateBindingSet<InterationExample, InterationViewModel>();

            /* Bind the method "OnOpenAlert" to an interactive request */
            bindingSet.Bind().For(v => v.OnOpenAlert).To(vm => vm.AlertDialogRequest);

            /* Bind the DialogInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.dialogInteractionAction).To(vm => vm.AsyncAlertDialogRequest);

            /* Bind the ToastInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.toastInteractionAction).To(vm => vm.ToastRequest);
            /* or bind the method "OnShowToast" to an interactive request */
            //bindingSet.Bind().For(v => v.OnShowToast).To(vm => vm.ToastRequest);

            /* Bind the LoadingInteractionAction to an interactive request */
            bindingSet.Bind().For(v => v.loadingInteractionAction).To(vm => vm.LoadingRequest);
            /* or bind the method "OnShowOrHideLoading" to an interactive request */
            //bindingSet.Bind().For(v => v.OnShowOrHideLoading).To(vm => vm.LoadingRequest);

            /* Binding command */
            bindingSet.Bind(openAlert).For(v => v.onClick).To(vm => vm.OpenAlertDialog);
            bindingSet.Bind(asyncOpenAlert).For(v => v.onClick).To(vm => vm.AsyncOpenAlertDialog);
            bindingSet.Bind(showToast).For(v => v.onClick).To(vm => vm.ShowToast);
            bindingSet.Bind(showLoading).For(v => v.onClick).To(vm => vm.ShowLoading);
            bindingSet.Bind(hideLoading).For(v => v.onClick).To(vm => vm.HideLoading);

            bindingSet.Build();
        }

        void OnOpenAlert(object sender, InteractionEventArgs args)
        {
            var notification = args.Context as DialogNotification;
            var callback     = args.Callback;

            if (notification == null)
                return;

            AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
            {
                notification.DialogResult = result;
                callback?.Invoke();
            });
        }

        void OnShowToast(object sender, InteractionEventArgs args)
        {
            var notification = args.Context as ToastNotification;
            if (notification == null)
                return;

            Toast.Show(this, notification.Message, notification.Duration);
        }

        void OnShowOrHideLoading(object sender, InteractionEventArgs args)
        {
            var notification = args.Context as VisibilityNotification;
            if (notification == null)
                return;

            if (notification.Visible)
                list.Add(Loading.Show());
            else
            {
                if (list.Count <= 0)
                    return;

                var loading = list[0];
                loading.Dispose();
                list.RemoveAt(0);
            }
        }
    }
}