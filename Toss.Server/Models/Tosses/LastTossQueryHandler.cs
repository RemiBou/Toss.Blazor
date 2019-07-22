using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared.Tosses;
using Toss.Server.Models.Tosses;
namespace Toss.Server.Controllers
{
    public class LastTossQueryHandler : IRequestHandler<TossLastQuery, IEnumerable<TossLastQueryItem>>
    {

        private readonly IAsyncDocumentSession _session;
        private readonly RavenDBIdUtil ravenDBIdUtil;

        public LastTossQueryHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDBIdUtil)
        {
            _session = session;
            this.ravenDBIdUtil = ravenDBIdUtil;
        }

        public async Task<IEnumerable<TossLastQueryItem>> Handle(TossLastQuery request, CancellationToken cancellationToken)
        {
            List<TossLastQueryItem> list = await _session.Query<TossEntity>()
                .Where(t => t.Tags.Contains(request.HashTag))
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new TossLastQueryItem()
                {
                    Content = t.Content.Substring(0, 100),
                    CreatedOn = t.CreatedOn,
                    Id = t.Id,
                    UserName = t.UserName,
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
