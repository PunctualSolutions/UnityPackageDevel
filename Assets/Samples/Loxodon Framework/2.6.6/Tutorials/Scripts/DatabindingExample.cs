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
using System.Globalization;
using System.Text.RegularExpressions;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Builder;

namespace Loxodon.Framework.Tutorials
{
    public class Account : ObservableObject
    {
        int                                 id;
        string                              username;
        string                              password;
        string                              email;
        DateTime                            birthday;
        readonly ObservableProperty<string> address = new();

        public int ID
        {
            get => id;
            set => Set(ref id, value);
        }

        public string Username
        {
            get => username;
            set => Set(ref username, value);
        }

        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        public string Email
        {
            get => email;
            set => Set(ref email, value);
        }

        public DateTime Birthday
        {
            get => birthday;
            set => Set(ref birthday, value);
        }

        public ObservableProperty<string> Address => address;
    }

    public class AccountViewModel : ViewModelBase
    {
        Account                              account;
        bool                                 remember;
        string                               username;
        string                               email;
        ObservableDictionary<string, string> errors = new();

        public Account Account
        {
            get => account;
            set => Set(ref account, value);
        }

        public string Username
        {
            get => username;
            set => Set(ref username, value);
        }

        public string Email
        {
            get => email;
            set => Set(ref email, value);
        }

        public bool Remember
        {
            get => remember;
            set => Set(ref remember, value);
        }

        public ObservableDictionary<string, string> Errors
        {
            get => errors;
            set => Set(ref errors, value);
        }

        public void OnUsernameValueChanged(string value)
        {
            Debug.LogFormat("Username ValueChanged:{0}", value);
        }

        public void OnEmailValueChanged(string value)
        {
            Debug.LogFormat("Email ValueChanged:{0}", value);
        }

        public void OnSubmit()
        {
            if (string.IsNullOrEmpty(Username) || !Regex.IsMatch(Username, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                errors["errorMessage"] = "Please enter a valid username.";
                return;
            }

            if (string.IsNullOrEmpty(Email) || !Regex.IsMatch(Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                errors["errorMessage"] = "Please enter a valid email.";
                return;
            }

            errors.Clear();
            Account.Username = Username;
            Account.Email    = Email;
        }
    }

    public class DatabindingExample : UIView
    {
        public Text description;
        public Text title;
        public Text username;
        public Text password;
        public Text email;
        public Text birthday;
        public Text address;
        public Text remember;

        public Text errorMessage;

        public InputField usernameEdit;
        public InputField emailEdit;
        public Toggle     rememberEdit;
        public Button     submit;

        Localization localization;

        protected override void Awake()
        {
            var context        = Context.GetApplicationContext();
            var bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            var cultureInfo = Locale.GetCultureInfo();
            localization             = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LocalizationTutorials", new XmlDocumentParser()));
        }

        protected override void Start()
        {
            var account = new Account()
                          {
                              ID       = 1,
                              Username = "test",
                              Password = "test",
                              Email    = "yangpc.china@gmail.com",
                              Birthday = new(2000, 3, 3),
                          };
            account.Address.Value = "beijing";

            var accountViewModel = new AccountViewModel()
                                   {
                                       Account = account,
                                   };

            var bindingContext = this.BindingContext();
            bindingContext.DataContext = accountViewModel;

            /* databinding */
            var bindingSet = this.CreateBindingSet<DatabindingExample, AccountViewModel>();
            //			bindingSet.Bind (this.username).For ("text").To ("Account.Username").OneWay ();
            //			bindingSet.Bind (this.password).For ("text").To ("Account.Password").OneWay ();
            bindingSet.Bind(username).For(v => v.text).To(vm => vm.Account.Username).OneWay();
            bindingSet.Bind(password).For(v => v.text).To(vm => vm.Account.Password).OneWay();
            bindingSet.Bind(email).For(v => v.text).To(vm => vm.Account.Email).OneWay();
            bindingSet.Bind(remember).For(v => v.text).To(vm => vm.Remember).OneWay();
            bindingSet.Bind(birthday).For(v => v.text).ToExpression(vm => string.Format("{0} ({1})", vm.Account.Birthday.ToString("yyyy-MM-dd"), DateTime.Now.Year - vm.Account.Birthday.Year)).OneWay();

            bindingSet.Bind(address).For(v => v.text).To(vm => vm.Account.Address).OneWay();
            bindingSet.Bind(description).For(v => v.text).ToExpression(vm => localization.GetFormattedText("databinding.tutorials.description", vm.Account.Username, vm.Username)).OneWay();

            bindingSet.Bind(errorMessage).For(v => v.text).To(vm => vm.Errors["errorMessage"]).OneWay();

            bindingSet.Bind(usernameEdit).For(v => v.text, v => v.onEndEdit).To(vm => vm.Username).TwoWay();
            bindingSet.Bind(usernameEdit).For(v => v.onValueChanged).To<string>(vm => vm.OnUsernameValueChanged);
            bindingSet.Bind(emailEdit).For(v => v.text, v => v.onEndEdit).To(vm => vm.Email).TwoWay();
            bindingSet.Bind(emailEdit).For(v => v.onValueChanged).To<string>(vm => vm.OnEmailValueChanged);
            bindingSet.Bind(rememberEdit).For(v => v.isOn, v => v.onValueChanged).To(vm => vm.Remember).TwoWay();
            bindingSet.Bind(submit).For(v => v.onClick).To(vm => vm.OnSubmit);
            bindingSet.Build();

            var staticBindingSet = this.CreateBindingSet<DatabindingExample>();
            staticBindingSet.Bind(title).For(v => v.text).To(() => Res.databinding_tutorials_title).OneTime();
            //staticBindingSet.Bind(this.title).For(v => v.text).To("Res.databinding_tutorials_title").OneTime();
            staticBindingSet.Build();
        }
    }
}