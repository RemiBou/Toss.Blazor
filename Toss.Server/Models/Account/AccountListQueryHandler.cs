using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class AccountListQueryHandler : IRequestHandler<AccountListQuery, List<AdminAccountListItem>>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccountListQueryHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<List<AdminAccountListItem>> Handle(AccountListQuery request, CancellationToken cancellationToken)
        {
            return await userManager.Users
                    .Select(u => new AdminAccountListItem()
                    {
                        Email = u.Email,
                        EmailConfirmed = u.EmailConfirmed,
                        Id = u.Id,
                        UserName = u.UserName
                    }).ToListAsync();
        }
    }
}
