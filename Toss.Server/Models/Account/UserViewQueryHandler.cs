using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class UserViewQueryHandler : IRequestHandler<UserViewQuery, UserViewResult>

    {
        private RavenDBIdUtil ravenDBIdUtil;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger _logger;

        public UserViewQueryHandler(RavenDBIdUtil ravenDBIdUtil, IAsyncDocumentSession session, ILogger<AccountController> logger)
        {
            this.ravenDBIdUtil = ravenDBIdUtil;
            _session = session;
            _logger = logger;
        }

        public async Task<UserViewResult> Handle(UserViewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetch user for view : {request.UserName}");
            ApplicationUser user = await _session.Query<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                return null;
            }
            return new UserViewResult(
                user.UserName,
                user.Bio
            );
        }
    }
}