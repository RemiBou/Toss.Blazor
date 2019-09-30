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
        private readonly NavigationManager uriHelper;

        private AccountViewModel _currentAccount;
        public AccountService (IHttpApiClientRequestBuilderFactory http, NavigationManager uriHelper) {
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
                    })
                    .Get ();
            }
            return _currentAccount;
        }


        public async Task Logout () {
            await http.Create ("/api/account/logout")
                .OnOK ("You have successfuly logged off.", "/login")
                .Post ();
            _currentAccount = null;
        }

        public void SubscribeOnCurrentAccountChanged (EventHandler<AccountViewModel> handler) {
            this.OnCurrentAccountChanged += handler;
            handler.Invoke (this, _currentAccount); 
        }

        public async Task AddHashTag(AddHashtagCommand command, Action successCallback, Action<Dictionary<string,List<string>>> badRequestCallBack)
        {
            await http.Create("/api/account/addHashTag")
            .OnOK(() =>
            {
                _currentAccount.Hashtags.Add(command.NewHashTag);
                successCallback();
            })
            .OnBadRequest<Dictionary<string, List<string>>>(badRequestCallBack)
            .Post(command);
        }

        public async Task RemoveHashTag(RemoveHashTagCommand command, Action successCallback)
        {
            await http.Create("/api/account/removeHashTag")
                .OnOK(() =>
                {
                    _currentAccount.Hashtags.Remove(command.HashTag);
                    successCallback();
                })
                .Post(command);
        }
    }
}