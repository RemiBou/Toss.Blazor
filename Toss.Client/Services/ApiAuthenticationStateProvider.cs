﻿using Microsoft.AspNetCore.Components;
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
        private readonly IHttpApiClientRequestBuilderFactory httpFactory;

        public ApiAuthenticationStateProvider(IHttpApiClientRequestBuilderFactory httpFactory)
        {
            this.httpFactory = httpFactory;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AccountViewModel account = null;
            await httpFactory.Create("/api/account/details")
                       .OnOK<AccountViewModel>((a) => {
                           account = a;
                       })
                       .Get();
            var identity = account  != null ?
                new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Name, account.Name),}, "apiauth") :
                new ClaimsIdentity();

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void MarkUserAsAuthenticated()
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
