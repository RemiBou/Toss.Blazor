using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static Dictionary<string,string> ToFlatDictionary(this ModelStateDictionary modelState)
        {
            return modelState.ToDictionary(s => s.Key, s => string.Join(" ", s.Value.Errors.Select(e => e.ErrorMessage)));
        }
        public static Dictionary<string, string> ToFlatDictionary(this IdentityResult modelState)
        {
            return new Dictionary<string, string>()
            {
                {"Other",string.Join(" ", modelState.Errors.Select(e => e.Description)) }
            };
        }
    }
}
