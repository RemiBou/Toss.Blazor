using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toss.Shared;
using Toss.Server.Extensions;
using Toss.Server.Models;
using System.Net;
using System.Security.Claims;
using MediatR;
using System.Collections.Generic;
using Toss.Server.Models.Account;
using Toss.Shared.Account;

namespace Toss.Server.Controllers
{
    [Authorize, ApiController, Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        private readonly IMediator _mediator;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IMediator mediator)
        {
            _signInManager = signInManager;
            _logger = logger;
            _mediator = mediator;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginProviders()
        {
            return Ok(await _mediator.Send(new LoginProvidersQuery()));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                if (result.IsLockout)
                    return Redirect("/lockout");
                ModelState.AddModelError("UserName", "Invalid login attempt.");
                return BadRequest(ModelState);

            }
            //if (result.Need2FA)
            //{
            //    return RedirectToAction("/loginWith2fa");
            //}
            return Ok();
        }
        /// <summary>
        /// Adds a hashtag to a user
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddHashTag(string newTag)
        {
            return await _mediator.ExecuteCommandReturnActionResult(new AddHashtagCommand(newTag));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

     

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Redirect("/login");

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return Redirect("/login");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect("/login");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return Redirect("/account/lockout");
            }

            if (result.IsNotAllowed)
            {
                return Redirect("/login");

            }
            // If the user does not have an account, then ask the user to create an account.

            return Redirect("/account/externalLogin");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginDetails()
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }
            return Ok(new ExternalLoginConfirmationCommand()
            {
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                Provider = info.LoginProvider
            });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationCommand command)
        {
            
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var user = await _mediator.Send(new CurrentAccountDetailsQuery());

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAccountCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            return await _mediator.ExecuteCommandReturnActionResult(command);
        }
    }
}
