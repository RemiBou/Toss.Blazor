using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class LoginProvidersQueryHandler : IRequestHandler<LoginProvidersQuery, IEnumerable<SigninProviderViewModel>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginProvidersQueryHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<SigninProviderViewModel>> Handle(LoginProvidersQuery request, CancellationToken cancellationToken)
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Select(s => new SigninProviderViewModel()
                {
                    Name = s.Name,
                    DisplayName = s.DisplayName
                });
        }
    }
}
