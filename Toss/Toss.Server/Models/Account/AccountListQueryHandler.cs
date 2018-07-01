using MediatR;
using Microsoft.AspNetCore.Identity;
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
        
        private UserManager<ApplicationUser> userManager;

       

        public AccountListQueryHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<List<AdminAccountListItem>> Handle(AccountListQuery request, CancellationToken cancellationToken)
        {
            var users = userManager.Users.ToList();
            return users.Select(u => new AdminAccountListItem()
            {
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                Id = u.Id,
                UserName = u.UserName                
            }).ToList();
        }
    }
}
