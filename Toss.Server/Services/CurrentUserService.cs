using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toss.Server.Services
{
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Returns the user id, it's always setup by aspnet identity in a claim
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string UserId(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;
            if (!user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                return null;
            return user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
        
    }
}
