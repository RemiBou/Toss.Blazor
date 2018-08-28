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
        private readonly IHttpContextAccessor httpContextAccessor;

        public RemoveHashTagCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult> Handle(RemoveHashTagCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            user.RemoveHashTag(request.HashTag);
            await _userManager.UpdateAsync(user);
            return CommandResult.Success();
        }
    }
}