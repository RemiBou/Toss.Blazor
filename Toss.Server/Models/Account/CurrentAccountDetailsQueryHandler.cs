using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Toss.Shared;

namespace Toss.Server.Models.Account
{
    public class CurrentAccountDetailsQueryHandler : IRequestHandler<CurrentAccountDetailsQuery, AccountViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAccountDetailsQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<AccountViewModel> Handle(CurrentAccountDetailsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user == null)
            {
                return null;
            }

            return new AccountViewModel
            {
                HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                Hashtags = user.Hashtags?.ToList() ?? new List<string>(),
                IsAdmin = user.Roles.Contains("Admin")
            };
        }
    }
}