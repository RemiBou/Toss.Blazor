using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Toss.Shared;
using System.Linq;
using System.Collections.Generic;

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
                       .OnOK<AccountViewModel>((a) =>
                       {
                           account = a;
                       })
                       .Get();
            var identity = account != null ?
                new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Name, account.Name),
                    new Claim(SecurityExtension.ClaimBio, account.Bio ?? ""),
                    new Claim(SecurityExtension.ClaimHashtags, string.Join(",",  account.Hashtags)),
                    new Claim(ClaimTypes.AuthenticationMethod, account.HasPassword ? "internal" : "external")
                }, "apiauth")
                     :
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

    public static class SecurityExtension
    {
        public static string ClaimBio = "Bio";
        public static string ClaimHashtags = "Tags";
        public static IEnumerable<string> HashTags(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimHashtags)?.Value.Split(',') ?? Enumerable.Empty<string>();
        }

        public static string Email(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }
        public static string Name(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static string Bio(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimBio)?.Value;
        }
    }
}
