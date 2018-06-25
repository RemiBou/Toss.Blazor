using MediatR;
using System.ComponentModel.DataAnnotations;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Shared.Account
{
    public class AddHashtagCommand : IRequest<CommandResult>
    {

        public const string HashTagRegex = @"\w+";
        public AddHashtagCommand()
        { }
        public AddHashtagCommand(string newHashTag)
        {

            NewHashTag = newHashTag;
        }
        [RegularExpression(HashTagRegex, ErrorMessage = "The hashtag must be only character and numbers")]
        public string NewHashTag { get; set; }
    }

}
