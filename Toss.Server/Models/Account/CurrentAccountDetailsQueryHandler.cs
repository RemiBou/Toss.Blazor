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
        private readonly IMediator _mediator;

        public CurrentAccountDetailsQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AccountViewModel> Handle(CurrentAccountDetailsQuery request, CancellationToken cancellationToken)
        {
            new CurrentUserQuery();
            var user = await _mediator.Send(new CurrentUserQuery());

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
                IsAdmin = user.Roles.Contains(ApplicationUser.AdminRole),
                Name = user.UserName,
                Bio = user.Bio

            };
        }
    }
}