using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Toss.Server.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static ModelStateDictionary ToModelStateDictionary(this IdentityResult identityResult)
        {
            var res = new ModelStateDictionary();
            foreach (var errors in identityResult.Errors)
            {
                res.AddModelError(errors.Code,errors.Description);
            }
            return res;
        }
    }
}
