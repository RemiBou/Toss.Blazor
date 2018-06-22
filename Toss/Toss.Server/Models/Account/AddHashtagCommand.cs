using MediatR;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class AddHashtagCommand : IRequest<CommandResult>
    {
        public AddHashtagCommand(string newHashTag)
        {           

            NewHashTag = newHashTag;
        }

        public string NewHashTag { get; set; }
    }

}
