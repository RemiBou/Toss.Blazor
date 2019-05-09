using MediatR;
using Raven.Client.Documents.Session;
using Raven.Client.Documents;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Services;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class SponsoredTossQueryHandler : IRequestHandler<SponsoredTossQuery, TossLastQueryItem>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IMediator mediator;
        private readonly IRandom random;
        private readonly RavenDBIdUtil ravenDBIdUtil;

        public SponsoredTossQueryHandler(IAsyncDocumentSession session, IMediator mediator, IRandom random, RavenDBIdUtil ravenDBIdUtil)
        {
            this._session = session;
            this.mediator = mediator;
            this.random = random;
            this.ravenDBIdUtil = ravenDBIdUtil;
        }

        public async Task<TossLastQueryItem> Handle(SponsoredTossQuery request, CancellationToken cancellationToken)
        {
            var resCollection = (await _session.Query<SponsoredTossEntity>()
                .Where(s => s.Tags.Contains(request.Hashtag))
                .Where(s => s.DisplayedCount > 0)
                .Select(t => new TossLastQueryItem()
                {
                    Content = t.Content,
                    CreatedOn = t.CreatedOn,
                    Id = t.Id,
                    UserName = t.UserName
                })
                .ToListAsync())
                  .ToLookup(t => t.UserName)
                  .ToArray();
            if (!resCollection.Any())
                return null;
            var index = random.NewRandom(resCollection.Length - 1);
            var userToss = resCollection[index].ToArray();
            var res = userToss[random.NewRandom(userToss.Count() - 1)];
            await mediator.Publish(new SponsoredTossDisplayed(res.Id));
            res.Id = ravenDBIdUtil.GetUrlId(res.Id);
            return res;

        }
    }
}
