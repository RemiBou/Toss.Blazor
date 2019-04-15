using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Extensions;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand,CommandResult> 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var httpUser = _httpContextAccessor.HttpContext.User;
            var user = await _userManager.GetUserAsync(httpUser);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(httpUser)}'.");
            }
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return new CommandResult("User", "User has no password set");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                // see https://github.com/aspnet/AspNetCore/blob/bfec2c14be1e65f7dd361a43950d4c848ad0cd35/src/Identity/Extensions.Core/src/IdentityErrorDescriber.cs
                // for diffrent error codes
                var keyMapping = new Dictionary<string, string>()
                {
                    {"PasswordMismatch","CurrentPassword" },
                    {"PasswordTooShort","NewPassword" },
                    {"PasswordRequiresUniqueChars","NewPassword" },
                    {"PasswordRequiresNonAlphanumeric","NewPassword" },
                    {"PasswordRequiresDigit","NewPassword" },
                    {"PasswordRequiresLower","NewPassword" },
                    {"PasswordRequiresUpper","NewPassword" },

                };
            var formatedErrors = changePasswordResult.Errors
                .Select(e =>
                {
                    var key = e.Code;
                    keyMapping.TryGetValue(key, out key);
                    return new { Key = key, e.Description };
                }
                ).ToLookup(e => e.Key, e => e.Description)
                .ToDictionary(l => l.Key, l => l.ToList());
                return new CommandResult(formatedErrors);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            return CommandResult.Success();
        }
    }
}
