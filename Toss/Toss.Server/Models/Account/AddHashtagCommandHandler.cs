using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Toss.Shared;
using Toss.Server.Models;
using MediatR;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Toss.Server.Controllers
{
    public class AddHashtagCommandHandler : IRequestHandler<AddHashtagCommand, CommandResult>
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor httpContextAccessor;

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
