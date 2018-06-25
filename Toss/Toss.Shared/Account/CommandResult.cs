using System.Collections.Generic;
using System.Linq;

namespace Toss.Shared.Account
{
    public class CommandResult
    {
        public  Dictionary<string,List<string>> Errors { get; set; }
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
            Errors = new  Dictionary<string,List<string>>()
            {
                {errorKey,new List<string>{errorMessage} }
            };
        }
        public CommandResult(Dictionary<string,List<string>> errors)
        {
            Errors = errors;
        }
        public static CommandResult Success()
        {
            return new CommandResult();
        }
    }
}