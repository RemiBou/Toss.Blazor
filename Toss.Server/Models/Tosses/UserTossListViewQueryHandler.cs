using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    class UserTossListViewQueryHandler : IRequestHandler<UserTossListViewQuery, IEnumerable<UserTossListViewResult>>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil ravenDBIdUtil;

        public UserTossListViewQueryHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDBIdUtil)
        {
            _session = session;
            this.ravenDBIdUtil = ravenDBIdUtil;
        }

        public async Task<IEnumerable<UserTossListViewResult>> Handle(UserTossListViewQuery request, CancellationToken cancellationToken)
        {
            List<UserTossListViewResult> list = await _session.Query<TossEntity>()
                .Where(t => t.UserName == request.UserName)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new UserTossListViewResult()
                {
                    Content = t.Content.Substring(0, 100),
                    CreatedOn = t.CreatedOn,
                    Id = t.Id,
                    Tags = t.Tags
                })
                .Paginate(request.Page, TossLastQuery.TossPerPage)
                .ToListAsync();
            foreach (var item in list)
            {
                item.Id = ravenDBIdUtil.GetUrlId(item.Id);
            }
            return list;
        }
    }
}