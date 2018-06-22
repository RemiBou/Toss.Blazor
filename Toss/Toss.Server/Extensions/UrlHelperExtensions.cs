using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Toss.Server.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
           
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = $"account/confirmationEmail/{userId}/{WebUtility.UrlEncode(code)}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = $"account/resetPassword/{userId}/{WebUtility.UrlEncode(code)}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }
    }
}
