using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Toss.Shared;

namespace Toss.Client.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {

        public AccountViewModel Account { get; private set; }
        private readonly IHttpApiClientRequestBuilderFactory httpFactory;

        public ApiAuthenticationStateProvider(IHttpApiClientRequestBuilderFactory httpFactory)
        {
            this.httpFactory = httpFactory;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            await httpFactory.Create("/api/account/details")
                       .OnOK<AccountViewModel>((a) =>
                       {
                           Account = a;
                       })
                       .Get();
            var identity = Account != null ?
                new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, Account.Email),
                    new Claim(ClaimTypes.Name, Account.Name),}, "apiauth") :
                new ClaimsIdentity();
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void RefreshCurrentUser()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
