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
using Raven.Client.Documents.Queries;
using Toss.Server.Models.Account;

namespace Toss.Server.Models.Tosses
{
    class TossConversationQueryHandler : IRequestHandler<TossConversationQuery, TossConversationQueryResult>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil _ravenDbIdUtil;

        private readonly IMediator mediator;

        public TossConversationQueryHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDbIdUtil, IMediator mediator)
        {
            _session = session;
            _ravenDbIdUtil = ravenDbIdUtil;
            this.mediator = mediator;
        }

        public async Task<TossConversationQueryResult> Handle(TossConversationQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await mediator.Send(new CurrentUserQuery());
            var tossId = _ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId);
            var toss = await _session.LoadAsync<TossEntity>(tossId);
            IQueryable<TossConversation> queryable = _session.Query<TossConversation>()
                                    .Where(c => c.TossId == tossId);

            if (!toss.IsCreator(currentUser))
            {
                queryable = queryable.Where(c => c.CreatorUserId == currentUser.Id);
            }

            var items = await (from c in queryable
                               let u = RavenQuery.Load<ApplicationUser>(c.CreatorUserId)
                               select new TossConversationQueryResultItem()
                               {
                                   Id = c.Id,
                                   CreatorUserName = u.UserName,
                                   MessageCount = c.Messages.Count()
                               }).ToListAsync();
            items.ForEach(c => c.Id = _ravenDbIdUtil.GetUrlId(c.Id));
            return new TossConversationQueryResult()
            {
                Conversations = items
            };
        }
    }
}