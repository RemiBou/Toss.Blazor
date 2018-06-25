using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Toss.Shared;

namespace Toss.Server.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static ModelStateDictionary ToModelStateDictionary(this IdentityResult identityResult)
        {
            var res = new ModelStateDictionary();
            foreach (var errors in identityResult.Errors)
            {
                res.AddModelError(errors.Code, errors.Description);
            }
            return res;
        }
        public static  Dictionary<string,List<string>> ToValidationErrorDictonary(this IdentityResult identityResult)
        {
            return identityResult.ToModelStateDictionary().ToValidationErrorDictonary();
        }
        public static  Dictionary<string,List<string>> ToValidationErrorDictonary(this ModelStateDictionary modelState)
        {
            var res = new  Dictionary<string,List<string>>();
            foreach (var validationError in modelState)
            {
                foreach (var msg in validationError.Value.Errors)
                {
                    List<string> errors;
                    if(!res.TryGetValue(validationError.Key, out errors))
                    {
                        errors = new List<string>();
                        res.Add(validationError.Key,errors);
                    }
                    errors.Add(msg.ErrorMessage);

                }
            }
            return res;
        }
    }
}
