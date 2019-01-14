using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class BestTagsQueryHandler : IRequestHandler<BestTagsQuery, List<BestTagsResult>>
    {
        private readonly ICosmosDBTemplate<TagByDayIndex> _indexCosmosDBTemplate;
        private readonly INow _now;

        public BestTagsQueryHandler(ICosmosDBTemplate<TagByDayIndex> indexCosmosDBTemplate, INow now)
        {
            _indexCosmosDBTemplate = indexCosmosDBTemplate;
            _now = now;
        }

        public async Task<List<BestTagsResult>> Handle(BestTagsQuery request, CancellationToken cancellationToken)
        {
            var query = await _indexCosmosDBTemplate.CreateDocumentQuery();
            var firstDay = _now.Get().AddDays(-30) ;
            return await query.Where(i => i.CreatedOn >= firstDay)
                .Select(i => i.Tag)
                .AsEnumerable()
                .GroupBy(t => t)
                .OrderByDescending(t => t.Count())
                .Take(50)
                .Select(t => new BestTagsResult()
                {
                    CountLastMonth = t.Count(),
                    Tag = t.Key
                })
                .ToAsyncEnumerable()
                .ToList();
        }
    }
}
