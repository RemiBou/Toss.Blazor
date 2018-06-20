using Toss.Shared;
using MediatR;

namespace Toss.Server.Controllers
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
