using System.Collections.Generic;
using System.Linq;

namespace Toss.Shared.Account
{
    public class CommandResult
    {
        public ValidationErrorDictonary Errors { get; set; }
        public bool IsSucess
        {
            get
            {
                return Errors == null || !Errors.Any();
            }
        }
        public CommandResult()
        {

        }
        public CommandResult(string errorKey, string errorMessage)
        {
            Errors = new ValidationErrorDictonary()
            {
                {errorKey,new List<string>{errorMessage} }
            };
        }
        public CommandResult(ValidationErrorDictonary errors)
        {
            Errors = errors;
        }
        public static CommandResult Success()
        {
            return new CommandResult();
        }
    }
}