using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Client.Services
{
    public class AccountService : IAccountService
    {
        private IHttpApiClientRequestBuilderFactory http;
        public event EventHandler OnLogoutDone;
        public event EventHandler OnLoginDone;
        private IUriHelper uriHelper;
        public AccountService(IHttpApiClientRequestBuilderFactory http, IUriHelper uriHelper)
        {
            this.http = http;
            this.uriHelper = uriHelper;
        }
        /// <summary>
        /// Get the current user account details from backend
        /// </summary>
        /// <returns></returns>
        public async Task<AccountViewModel> CurrentAccount()
        {
            AccountViewModel account = null;
            await http.Create("/api/account/details")
              .OnOK<AccountViewModel>((a) => account = a)
              .Get();
            return account;
        }

        public async Task Logout()
        {
            await http.Create("/api/account/logout")
                   .OnOK("You have successfuly logged off.", "/login")
                   .Post();
            OnLogoutDone?.Invoke(this, null);
        }

        public async Task<Dictionary<string, List<string>>> Login(LoginCommand loginCommand, ElementRef loginButton)
        {
            Dictionary<string, List<string>> _errors = null;
            await http.Create("/api/account/login", loginButton)
                  .OnBadRequest<Dictionary<string, List<string>>>(errors => _errors = errors)
                  .OnOK(() =>
                  {
                      OnLoginDone?.Invoke(this, null);
                      uriHelper.NavigateTo("/");
                  })
                  .Post(loginCommand);
            return _errors;
        }
    }
}
