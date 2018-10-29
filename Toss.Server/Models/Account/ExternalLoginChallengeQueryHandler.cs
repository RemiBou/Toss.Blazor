using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toss.Server.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System.Threading;

namespace Toss.Server.Controllers
{
    public class ExternalLoginChallengeQueryHandler : IRequestHandler<ExternalLoginChallengeQuery, AuthenticationProperties>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUrlHelper _urlHelper;

        public ExternalLoginChallengeQueryHandler(SignInManager<ApplicationUser> signInManager, IUrlHelper urlHelper)
        {
            _signInManager = signInManager;
            _urlHelper = urlHelper;
        }

        public Task<AuthenticationProperties> Handle(ExternalLoginChallengeQuery request, CancellationToken cancellationToken)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = _urlHelper.Action("ExternalLoginCallback", "Account", new { request.ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.ProviderName, redirectUrl);
            return Task.FromResult(properties);
        }
    }
}
