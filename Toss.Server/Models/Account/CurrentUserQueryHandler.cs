using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Toss.Server.Models.Account
{
    internal class CurrentUserQueryHandler : IRequestHandler<CurrentUserQuery, ApplicationUser>
    {
        private UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _httpContextAccessor;

        public CurrentUserQueryHandler()
        {
        }

        public CurrentUserQueryHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationUser> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }
    }
}