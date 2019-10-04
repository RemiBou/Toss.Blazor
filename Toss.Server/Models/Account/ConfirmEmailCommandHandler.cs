using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ConfirmEmailCommandHandler> logger;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, RavenDBIdUtil ravenDBIdUtil, ILogger<ConfirmEmailCommandHandler> logger)
        {
            _userManager = userManager;
            _ravenDBIdUtil = ravenDBIdUtil;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            string userId = this._ravenDBIdUtil.GetRavenDbIdFromUrlId<ApplicationUser>(request.UserId);
            var user = await _userManager.FindByIdAsync(userId);
            logger.LogInformation("Handle. UserId =  " + request.UserId + ", Code = " + request.Code);
            if (user == null)
            {
                return new CommandResult("User", "User does not exists");
            }
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);
            return result.Succeeded ? CommandResult.Success() : new CommandResult(result.ToValidationErrorDictonary());
        }
    }
}