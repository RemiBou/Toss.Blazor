using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class EditAccountCommandHandler : IRequestHandler<EditAccountCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator mediator;

        public EditAccountCommandHandler(
            UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(EditAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(_httpContextAccessor.HttpContext.User)}'.");
            }

            var email = user.Email;
            if (request.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, request.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }
            if(request.Name != user.UserName)
            {
                var setUserName = await _userManager.SetUserNameAsync(user, request.Name);
                if (!setUserName.Succeeded)
                {                    
                    throw new ApplicationException($"Unexpected error occurred setting name for user with ID '{user.Id}'.");
                }
                user.UserName = request.Name;
                await mediator.Publish(new AccountUserNameUpdated(user));
                
            }
            return Unit.Value;
        }
    }
}
