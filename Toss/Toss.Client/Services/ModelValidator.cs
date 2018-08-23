using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Toss.Client.Services
{
    /// <summary>
    /// Validate a given instance
    /// </summary>
    public class ModelValidator : IModelValidator
    {
        /// <summary>
        /// Raised is a validation error occurs
        /// </summary>
        public event EventHandler<ValidationErrorEventArgs> OnValidationDone;

        /// <summary>
        /// Validate the instance, if an error occurs returns false and raise the event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Validate(object instance)
        {
            List<ValidationResult> res = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instance, new ValidationContext(instance, null, null), res, true);

            OnValidationDone?.Invoke(this, new ValidationErrorEventArgs() { Errors = res, Instance = instance });
            Console.Write("Validation result : " + isValid);
            return isValid;

        }

      
    }
}
