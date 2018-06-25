using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toss.Server.Controllers;
using Toss.Server.Extensions;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper urlHelper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, ILogger<AccountController> logger, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            this.urlHelper = urlHelper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult> Handle(RegisterCommand model, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser { UserName = model.Name, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = urlHelper.EmailConfirmationLink(user.Id, code, httpContextAccessor.HttpContext.Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(model.Email, model.Name, callbackUrl);

                //await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return CommandResult.Success();
            }
            return new CommandResult("UserName", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
