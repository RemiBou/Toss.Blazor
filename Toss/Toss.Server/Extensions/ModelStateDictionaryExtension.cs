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
        public static ValidationErrorDictonary ToValidationErrorDictonary(this IdentityResult identityResult)
        {
            return identityResult.ToModelStateDictionary().ToValidationErrorDictonary();
        }
        public static ValidationErrorDictonary ToValidationErrorDictonary(this ModelStateDictionary modelState)
        {
            var res = new ValidationErrorDictonary();
            foreach (var validationError in modelState)
            {
                foreach (var msg in validationError.Value.Errors)
                {
                    res.AddModelError(validationError.Key, msg.ErrorMessage);

                }
            }
            return res;
        }
    }
}
