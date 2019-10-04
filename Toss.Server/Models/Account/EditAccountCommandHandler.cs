using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Toss.Server.Extensions;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class EditAccountCommandHandler : IRequestHandler<EditAccountCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator mediator;

        public EditAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            IMediator mediator)
        {
            _userManager = userManager;
            this.mediator = mediator;
        }

        public async Task<CommandResult> Handle(EditAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await mediator.Send(new CurrentUserQuery());
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user");
            }

            var email = user.Email;
            user.Bio = request.Bio;
            if (request.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, request.Email);
                if (!setEmailResult.Succeeded)
                {
                    return new CommandResult(setEmailResult.ToValidationErrorDictonary());
                }
                await mediator.Publish(new AccountEmailUpdated(user));

            }
            if (request.Name != user.UserName)
            {
                var setUserName = await _userManager.SetUserNameAsync(user, request.Name);

                if (!setUserName.Succeeded)
                {

                    return new CommandResult(setUserName.ToValidationErrorDictonary());
                }
                user.UserName = request.Name;
                await mediator.Publish(new AccountUserNameUpdated(user));

            }
            return CommandResult.Success();
        }
    }
}
