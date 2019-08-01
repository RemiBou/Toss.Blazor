using System;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Toss.Server.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            ActionContext actionContext = urlHelper.ActionContext;
            Microsoft.AspNetCore.Http.HttpContext httpContext = actionContext.HttpContext;
            Microsoft.AspNetCore.Http.HttpRequest request = httpContext.Request;
            var host = request.Host;

            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = $"account/confirmationEmail/{WebUtility.UrlEncode(userId)}/{WebUtility.UrlEncode(code)}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = $"account/resetPassword/{WebUtility.UrlEncode(userId)}/{WebUtility.UrlEncode(code)}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }

        public static string TossLink(this IUrlHelper urlHelper, string tossId, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = $"toss/"+tossId,
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }
    }
}
