using MediatR;

using Raven.Client.Documents.Session;
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
        private readonly IAsyncDocumentSession _session;

        public LastTossQueryHandler(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<TossLastQueryItem>> Handle(TossLastQuery request, CancellationToken cancellationToken)
        {
            return await _session.Query<TossEntity>()
                .Where(t => t.Tags.Contains(request.HashTag))
                .OrderByDescending(t => t.CreatedOn)
                 .Select(t => new TossLastQueryItem()
                 {
                     Content = t.Content.Substring(0, 100),
                     CreatedOn = t.CreatedOn,
                     Id = t.Id,
                     UserName = t.UserName
                 })
                .Take(50)
                .ToAsyncEnumerable()
                .ToList();
        }
    }
}
