using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Data.Indexes;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class BestTagsQueryHandler : IRequestHandler<BestTagsQuery, List<BestTagsResult>>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly INow _now;

        public BestTagsQueryHandler(IAsyncDocumentSession session, INow now)
        {
            _session = session;
            _now = now;
        }

        public async Task<List<BestTagsResult>> Handle(BestTagsQuery request, CancellationToken cancellationToken)
        {
            var firstDay = _now.Get().AddDays(-30);
           
            return (await _session
                .Query<TagByDayIndex, Toss_TagPerDay>()
                .Where(i => i.CreatedOn >= firstDay)
                .ToListAsync())
                //this needs has to be handled server side or at least cached
                .GroupBy(i => i.Tag)
                .OrderByDescending(g => g.Sum(i => i.Count))
                .Take(50)
                .Select(t => new BestTagsResult()
                {
                    CountLastMonth = t.Sum(i => i.Count),
                    Tag = t.Key
                })
                .ToList();
        }
    }
}
