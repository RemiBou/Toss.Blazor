using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class RemoveHashTagCommandHandler : IRequestHandler<RemoveHashTagCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator mediator;

        public RemoveHashTagCommandHandler(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            this.mediator = mediator;
        }

        public async Task<CommandResult> Handle(RemoveHashTagCommand request, CancellationToken cancellationToken)
        {
            var user = await mediator.Send(new CurrentUserQuery());
            user.RemoveHashTag(request.HashTag);
            await _userManager.UpdateAsync(user);
            return CommandResult.Success();
        }
    }
}