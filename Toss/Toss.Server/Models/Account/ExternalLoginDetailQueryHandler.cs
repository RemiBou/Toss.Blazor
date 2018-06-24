using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Toss.Server.Models;
using MediatR;
using System.Threading;

namespace Toss.Server.Controllers
{
    public class ExternalLoginDetailQueryHandler : IRequestHandler<ExternalLoginDetailQuery, ExternalLoginInfo>
    {

        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalLoginDetailQueryHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ExternalLoginInfo> Handle(ExternalLoginDetailQuery request, CancellationToken cancellationToken)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }
            return info;
        }
    }
}
