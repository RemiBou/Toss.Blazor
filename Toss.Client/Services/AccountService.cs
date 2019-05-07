using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Client.Services {
    public class AccountService : IAccountService {
        private readonly IHttpApiClientRequestBuilderFactory http;
        public event EventHandler<AccountViewModel> OnCurrentAccountChanged;
        private readonly IUriHelper uriHelper;

        private AccountViewModel _currentAccount;
        public AccountService (IHttpApiClientRequestBuilderFactory http, IUriHelper uriHelper) {
            this.http = http;
            this.uriHelper = uriHelper;
        }
        /// <summary>
        /// Get the current user account details from backend
        /// </summary>
        /// <returns></returns>
        public async Task<AccountViewModel> CurrentAccount () {
            if (_currentAccount == null) {
                await http.Create ("/api/account/details")
                    .OnOK<AccountViewModel> ((a) => {
                        _currentAccount = a;
                        RaiseEvent ();
                    })
                    .Get ();
            }
            return _currentAccount;
        }

        private void RaiseEvent () {
            this.OnCurrentAccountChanged?.Invoke (this, _currentAccount);
        }

        public async Task Logout () {
            await http.Create ("/api/account/logout")
                .OnOK ("You have successfuly logged off.", "/login")
                .Post ();
            _currentAccount = null;
            RaiseEvent ();
        }

        public async Task<Dictionary<string, List<string>>> Login (LoginCommand loginCommand) {
            Dictionary<string, List<string>> _errors = null;
            await http.Create ("/api/account/login")
                .OnBadRequest<Dictionary<string, List<string>>> (errors => _errors = errors)
                .OnOK (() => {
                    uriHelper.NavigateTo ("/");
                })
                .Post (loginCommand);
            return _errors;
        }

        public void SubscribeOnCurrentAccountChanged (EventHandler<AccountViewModel> handler) {
            this.OnCurrentAccountChanged += handler;
            handler.Invoke (this, _currentAccount);
        }
    }
}