using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    internal class MessageInConversationQueryHandler : IRequestHandler<MessageInConversationQuery, MessageInConversationQueryResult>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil _ravenDbIdUtil;

        public MessageInConversationQueryHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDbIdUtil)
        {
            _session = session;
            _ravenDbIdUtil = ravenDbIdUtil;
        }

        public async Task<MessageInConversationQueryResult> Handle(MessageInConversationQuery request, CancellationToken cancellationToken)
        {
            string id = _ravenDbIdUtil.GetRavenDbIdFromUrlId<TossConversation>(request.ConversationId);
            var res = await (from c in _session.Query<TossConversation>()
                             where c.Id == id
                             //let u = 
                             select new MessageInConversationQueryResult()
                             {
                                 Messages = c.Messages
                                    .Select(
                                        m => new MessageInConversationQueryResultItem()
                                        {
                                            Content = m.Content,
                                            CreatedOn = m.CreatedOn,
                                            UserName = RavenQuery.Load<ApplicationUser>(m.UserId).UserName
                                        })
                                    .ToList()
                             }
                ).FirstAsync();
            return res;
        }
    }
}