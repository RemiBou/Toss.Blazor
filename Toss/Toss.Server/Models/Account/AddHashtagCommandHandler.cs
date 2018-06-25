using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class AddHashtagCommandHandler : IRequestHandler<AddHashtagCommand, CommandResult>
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AddHashtagCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult> Handle(AddHashtagCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.NewHashTag))
            {
                return new CommandResult("newTag", "You must send a new tag");
            }
            var user = await _userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            if (user.AlreadyHasHashTag(request.NewHashTag))
            {
                return new CommandResult("newTag", "You already have this hashtag");
            }
            user.AddHashTag(request.NewHashTag);
            await _userManager.UpdateAsync(user);
            return CommandResult.Success();
        }
    }

}
