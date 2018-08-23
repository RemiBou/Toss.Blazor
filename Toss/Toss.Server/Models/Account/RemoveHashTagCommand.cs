using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Models;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class RemoveHashTagCommand : IRequest<CommandResult>
    {
        public RemoveHashTagCommand()
        {
        }

        public RemoveHashTagCommand(string hashTag)
        {
            HashTag = hashTag;
        }

        [Required]
        public string HashTag { get; set; }
    }

    public class RemoveHashTagCommandHandler : IRequestHandler<RemoveHashTagCommand, CommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RemoveHashTagCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult> Handle(RemoveHashTagCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            user.RemoveHashTag(request.HashTag);
            return CommandResult.Success();
        }
    }
}