using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Toss.Server.Extensions;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CommandResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new CommandResult("User","User does not exists");
            }
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);
            return result.Succeeded ? CommandResult.Success() : new CommandResult(result.ToValidationErrorDictonary());
        }
    }
}