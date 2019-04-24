using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Extensions;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RavenDBIdUtil _ravenDBIdUtil;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, RavenDBIdUtil ravenDBIdUtil)
        {
            _userManager = userManager;
            _ravenDBIdUtil = ravenDBIdUtil;
        }

        public async Task<CommandResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(this._ravenDBIdUtil.GetRavenDbIdFromUrlId<ApplicationUser>( request.UserId));
            if (user == null)
            {
                return new CommandResult("User", "User does not exists");
            }
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);
            return result.Succeeded ? CommandResult.Success() : new CommandResult(result.ToValidationErrorDictonary());
        }
    }
}