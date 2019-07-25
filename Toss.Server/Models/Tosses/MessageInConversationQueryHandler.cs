using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    internal class MessageInConversationQueryHandler : IRequestHandler<MessageInConversationQuery, MessageInConversationQueryResult>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;

        public MessageInConversationQueryHandler(IAsyncDocumentSession session, IHttpContextAccessor httpContextAccessor, RavenDBIdUtil ravenDbIdUtil)
        {
            _session = session;
            _httpContextAccessor = httpContextAccessor;
            _ravenDbIdUtil = ravenDbIdUtil;
        }

        public async Task<MessageInConversationQueryResult> Handle(MessageInConversationQuery request, CancellationToken cancellationToken)
        {
            string id = _ravenDbIdUtil.GetRavenDbIdFromUrlId<TossConversation>(request.ConversationId);
            var conversation = await _session.Query<TossConversation>()
                .Where(c => c.Id == id)
                .Select(
                    c => new MessageInConversationQueryResult()
                    {

                    }
                )
                .FirstAsync();
            return conversation;
        }
    }
}