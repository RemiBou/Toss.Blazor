using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Toss.Server.Extensions;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand,CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CommandResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            request.Code = WebUtility.UrlDecode(request.Code);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return CommandResult.Success();
            }
            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);

            return result.Succeeded ? CommandResult.Success() : new CommandResult(result.ToValidationErrorDictonary());
        }
    }
}
