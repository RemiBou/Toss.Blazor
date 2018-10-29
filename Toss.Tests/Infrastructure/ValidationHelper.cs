using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toss.Tests.Infrastructure
{
    public class ValidationHelper
    {
        public static bool IsValid(List<ValidationResult> res, object sut)
        {
            return Validator.TryValidateObject(sut, new ValidationContext(sut, null, null), res, true);
        }
    }
}
