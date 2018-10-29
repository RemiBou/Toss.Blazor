using MediatR;
using System.ComponentModel.DataAnnotations;
using Toss.Shared.Account;

namespace Toss.Shared.Account
{
    public class RemoveHashTagCommand : IRequest<CommandResult>
    {
        public RemoveHashTagCommand()
        {
        }

        public RemoveHashTagCommand(string hashTag)
        {
            HashTag = hashTag;
        }

        [Required]
        public string HashTag { get; set; }
    }
}