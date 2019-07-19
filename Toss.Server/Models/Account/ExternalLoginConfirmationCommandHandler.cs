using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Toss.Server.Controllers;
using Toss.Server.Extensions;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ExternalLoginConfirmationCommandHandler : IRequestHandler<ExternalLoginConfirmationCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public ExternalLoginConfirmationCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<CommandResult> Handle(ExternalLoginConfirmationCommand model, CancellationToken cancellationToken)
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                EmailConfirmed = true //we trust the external provider that this email belongs to the user
            };
            info.Principal = null; //needed because ravendb client will try so serialize it while it's not serializable 
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    return CommandResult.Success();
                }
            }
            return new CommandResult(result.ToValidationErrorDictonary());
        }
    }
}
