using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toss.Server.Extensions;
using Toss.Server.Services;

namespace Toss.Server.Models.Account
{
    public class ConfirmEmailSender : INotificationHandler<UserRegistered>, INotificationHandler<AccountEmailUpdated>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfirmEmailSender(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(UserRegistered notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            await SendEmail(user);
        }

        private async Task SendEmail(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.EmailConfirmationLink(user.Id, code, _httpContextAccessor.HttpContext.Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, user.UserName, callbackUrl);
        }

        public async Task Handle(AccountEmailUpdated notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
            await SendEmail(user);
        }
    }
}
