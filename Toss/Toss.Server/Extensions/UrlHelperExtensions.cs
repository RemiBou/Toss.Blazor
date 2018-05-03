using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationSample.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = "account/confirmation",
                Query = $"?userId={userId}&code={code}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            var ub = new UriBuilder(scheme, host.Host)
            {
                Path = "account/resetPassword",
                Query = $"?userId={userId}&code={code}",
                Port = host.Port.GetValueOrDefault(80)
            };
            return ub.ToString();
        }
    }
}
