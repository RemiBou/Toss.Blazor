using MediatR;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Controllers
{
    public class LastTossQueryHandler : IRequestHandler<TossLastQuery, IEnumerable<TossLastQueryItem>>
    {
        private readonly ICosmosDBTemplate<TossEntity> _dbTemplate;

        public LastTossQueryHandler(ICosmosDBTemplate<TossEntity> dbTemplate)
        {
            _dbTemplate = dbTemplate;
        }

        public async Task<IEnumerable<TossLastQueryItem>> Handle(TossLastQuery request, CancellationToken cancellationToken)
        {
            var query = await _dbTemplate.CreateDocumentQuery();

            var results = await query
                .Where(t => t.Content.Contains("#" + request.HashTag))
                .OrderByDescending(t => t.CreatedOn)
                .Take(50)
                .AsDocumentQuery()
                .GetAllResultsAsync();

            return results
                .Select(t => new TossLastQueryItem()
                    {
                        Content = t.Content,
                        CreatedOn = t.CreatedOn,
                        Id = t.Id,
                        UserName = t.UserName
                    });
        }
    }
}
