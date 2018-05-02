using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

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
