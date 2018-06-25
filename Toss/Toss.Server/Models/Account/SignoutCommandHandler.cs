using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Controllers;

namespace Toss.Server.Models.Account
{
    public class SignoutCommandHandler : IRequestHandler<SignoutCommand>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public SignoutCommandHandler(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Unit> Handle(SignoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Unit.Value;
        }
    }
}
