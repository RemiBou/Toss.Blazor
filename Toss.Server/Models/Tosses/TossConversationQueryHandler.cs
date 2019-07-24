using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;
using Raven.Client.Documents;

namespace Toss.Server.Models.Tosses
{
    class TossConversationQueryHandler : IRequestHandler<TossConversationQuery, TossConversationQueryResult>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;
        public async Task<TossConversationQueryResult> Handle(TossConversationQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.UserId();
            var tossId = _ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId);
            var conversation = await _session.Query<TossConversation>()
                                    .Where(c => c.TossId == request.TossId)
                                    .ToListAsync();
            // we fail silently, there is no point in managing error when user hacked the desired behavior, this might change in the future
            if (conversation == null)
            {
                return null;
            }
            var toss = await _session.LoadAsync<TossEntity>(tossId);
            return null;
        }
    }
}