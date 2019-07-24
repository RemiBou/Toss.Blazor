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

namespace Toss.Server.Models.Tosses
{
    class TossConversationQueryHandler : IRequestHandler<TossConversationQuery, TossConversationQueryResult>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RavenDBIdUtil _ravenDbIdUtil;



        public TossConversationQueryHandler(IAsyncDocumentSession session, IHttpContextAccessor httpContextAccessor, RavenDBIdUtil ravenDbIdUtil)
        {
            _session = session;
            _httpContextAccessor = httpContextAccessor;
            _ravenDbIdUtil = ravenDbIdUtil;
        }

        public async Task<TossConversationQueryResult> Handle(TossConversationQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.UserId();
            var tossId = _ravenDbIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId);
            var toss = await _session.LoadAsync<TossEntity>(tossId);
            IQueryable<TossConversation> queryable = _session.Query<TossConversation>()
                                    .Where(c => c.TossId == tossId);
            //if user is toss
            if (!toss.IsCreator(currentUser))
            {
                queryable = queryable.Where(c => c.CreatorUserId == currentUser);
            }

            var items = await (from c in queryable
                               let u = RavenQuery.Load<ApplicationUser>(c.CreatorUserId)
                               select new TossConversationQueryResultItem()
                               {
                                   Id = c.Id,
                                   CreatorUserName = u.UserName
                               }).ToListAsync();
            items.ForEach(c => c.Id = _ravenDbIdUtil.GetUrlId(c.Id));
            return new TossConversationQueryResult()
            {
                Conversations = items
            };
        }
    }
}