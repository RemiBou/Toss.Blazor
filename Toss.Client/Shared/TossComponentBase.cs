using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Toss.Client.Services;
using Toss.Shared;

namespace Toss.Client.Shared
{
    public class TossComponentBase : ComponentBase, IDisposable
    {
        [Inject]
        protected ApiAuthenticationStateProvider AuthenticationStateProvider { get; set; }
        protected AccountViewModel Account;

        public TossComponentBase()
        {
        }

        protected override void OnInitialized()
        {
            Account = AuthenticationStateProvider.Account;
            AuthenticationStateProvider.AuthenticationStateChanged += RefreshAccount;
        }

        private void RefreshAccount(Task<AuthenticationState> newAuthStateTask)
        {
            Account = AuthenticationStateProvider.Account;
            Console.WriteLine(this.GetType() + " RefreshAccount ");
            StateHasChanged();
        }

        void IDisposable.Dispose()
        {
            AuthenticationStateProvider.AuthenticationStateChanged -= RefreshAccount;
        }
    }

}