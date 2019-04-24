using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Services;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger,
            IMediator mediator)
        {
            _userManager = userManager;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<CommandResult> Handle(RegisterCommand model, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser { UserName = model.Name, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _mediator.Publish(new UserRegistered(user));
                _logger.LogInformation("User created a new account with password.");

                return CommandResult.Success();
            }
            // see https://github.com/aspnet/AspNetCore/blob/bfec2c14be1e65f7dd361a43950d4c848ad0cd35/src/Identity/Extensions.Core/src/IdentityErrorDescriber.cs
            // for diffrent error codes
            var keyMapping = new Dictionary<string, string>()
            {
                {"PasswordMismatch","Password" },
                {"InvalidUserName","Name" },
                {"InvalidEmail","Email" },
                {"DuplicateUserName","Name" },
                {"DuplicateEmail","Email" },
                {"PasswordTooShort","Password" },
                {"PasswordRequiresUniqueChars","Password" },
                {"PasswordRequiresNonAlphanumeric","Password" },
                {"PasswordRequiresDigit","Password" },
                {"PasswordRequiresLower","Password" },
                {"PasswordRequiresUpper","Password" },

            };
            var formatedErrors = result.Errors
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
    }
}
