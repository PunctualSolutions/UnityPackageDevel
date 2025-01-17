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

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Prefs;
using Loxodon.Framework.ViewModels;
using Loxodon.Log;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Loxodon.Framework.Examples
{
    public class LoginViewModel : ViewModelBase
    {
        static readonly ILog log = LogManager.GetLogger(typeof(ViewModelBase));

        const string LAST_USERNAME_KEY = "LAST_USERNAME";

        ObservableDictionary<string, string> errors = new();
        string                               username;
        string                               password;
        SimpleCommand                        loginCommand;
        SimpleCommand                        cancelCommand;

        Account account;

        Preferences     globalPreferences;
        IAccountService accountService;
        Localization    localization;

        InteractionRequest                    interactionFinished;
        InteractionRequest<ToastNotification> toastRequest;

        public LoginViewModel(IAccountService accountService, Localization localization, Preferences globalPreferences)
        {
            this.localization      = localization;
            this.accountService    = accountService;
            this.globalPreferences = globalPreferences;

            interactionFinished = new(this);
            toastRequest        = new(this);

            if (username == null) username = globalPreferences.GetString(LAST_USERNAME_KEY, "");

            loginCommand  = new(Login);
            cancelCommand = new(() => { interactionFinished.Raise(); /* Request to close the login window */ });
        }

        public IInteractionRequest InteractionFinished => interactionFinished;

        public IInteractionRequest ToastRequest => toastRequest;

        public ObservableDictionary<string, string> Errors => errors;

        public string Username
        {
            get => username;
            set
            {
                if (Set(ref username, value)) ValidateUsername();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (Set(ref password, value)) ValidatePassword();
            }
        }

        bool ValidateUsername()
        {
            if (string.IsNullOrEmpty(username) || !Regex.IsMatch(username, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                errors["username"] = localization.GetText("login.validation.username.error", "Please enter a valid username.");
                return false;
            }
            else
            {
                errors.Remove("username");
                return true;
            }
        }

        bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(password) || !Regex.IsMatch(password, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                errors["password"] = localization.GetText("login.validation.password.error", "Please enter a valid password.");
                return false;
            }
            else
            {
                errors.Remove("password");
                return true;
            }
        }

        public ICommand LoginCommand => loginCommand;

        public ICommand CancelCommand => cancelCommand;

        public Account Account => account;

        public async void Login()
        {
            try
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("login start. username:{0} password:{1}", username, password);

                this.account         = null;
                loginCommand.Enabled = false; /*by databinding, auto set button.interactable = false. */
                if (!(ValidateUsername() && ValidatePassword()))
                    return;

                var account = await accountService.Login(username, password);
                if (account != null)
                {
                    /* login success */
                    globalPreferences.SetString(LAST_USERNAME_KEY, username);
                    globalPreferences.Save();
                    this.account = account;
                    interactionFinished.Raise(); /* Interaction completed, request to close the login window */
                }
                else
                {
                    /* Login failure */
                    var tipContent = localization.GetText("login.failure.tip", "Login failure.");
                    toastRequest.Raise(new(tipContent, 2f)); /* show toast */
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("Exception:{0}", e);

                var tipContent = localization.GetText("login.exception.tip", "Login exception.");
                toastRequest.Raise(new(tipContent, 2f)); /* show toast */
            }
            finally
            {
                loginCommand.Enabled = true; /*by databinding, auto set button.interactable = true. */
            }
        }

        public Task<Account> GetAccount() => accountService.GetAccount(Username);
    }
}