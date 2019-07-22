using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;
        public async Task<Unit> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            var toss = await _session.LoadAsync<TossEntity>(_ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId));
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (toss == null)
            {
                return Unit.Value;
            }
            var currentUser = _httpContextAccessor.HttpContext.User.UserId();
            var newConversation = new TossConversation(toss.Id, currentUser);
            if (await _session.Advanced.ExistsAsync(newConversation.Id, cancellationToken))
            {
                return Unit.Value;
            }
            await _session.StoreAsync(newConversation);
            return Unit.Value;
        }
    }
}