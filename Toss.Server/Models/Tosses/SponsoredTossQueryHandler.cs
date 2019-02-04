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
        private IAsyncDocumentSession _session;
        private IMediator mediator;
        private IRandom random;

        public SponsoredTossQueryHandler(IAsyncDocumentSession session, IMediator mediator, IRandom random)
        {
            this._session = session;
            this.mediator = mediator;
            this.random = random;
        }

        public async Task<TossLastQueryItem> Handle(SponsoredTossQuery request, CancellationToken cancellationToken)
        {
            var resCollection = (await _session.Query<TossEntity>()
                .OfType<SponsoredTossEntity>()
                .Where(s => s.Tags.Contains(request.Hashtag))
                .Where(s => s.DisplayedCount > 0)
                .Select(t => new TossLastQueryItem()
                {
                    Content = t.Content,
                    CreatedOn = t.CreatedOn,
                    Id = t.Id,
                    UserName = t.UserId
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
            return res;

        }
    }
}
