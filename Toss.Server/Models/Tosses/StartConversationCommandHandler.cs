using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;

        public StartConversationCommandHandler(IAsyncDocumentSession session, IHttpContextAccessor httpContextAccessor, RavenDBIdUtil ravenDbIdUtil)
        {
            _session = session;
            _httpContextAccessor = httpContextAccessor;
            _ravenDbIdUtil = ravenDbIdUtil;
        }

        public async Task<Unit> Handle(StartConversationCommand request, CancellationToken cancellationToken)
        {
            var toss = await _session.LoadAsync<TossEntity>(_ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId));
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (toss == null)
            {
                throw new InvalidOperationException($"Toss does not exists : {request.TossId}");
            }
            var currentUser = _httpContextAccessor.HttpContext.User.UserId();
            if (currentUser == toss.UserId)
            {
                throw new InvalidOperationException($"Cannot create conversation when toss creator : {request.TossId}");
            }

            if (await _session.Query<TossConversation>().AnyAsync(c => c.CreatorUserId == currentUser && c.TossId == toss.Id))
            {
                throw new InvalidOperationException($"Conversation already exists. User : {currentUser}, Toss: {toss.Id}");
            }

            await _session.StoreAsync(new TossConversation(toss.Id, currentUser));
            return Unit.Value;
        }
    }
}