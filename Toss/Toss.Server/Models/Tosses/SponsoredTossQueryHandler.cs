using MediatR;
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
        private ICosmosDBTemplate<TossEntity> cosmosDBTemplate;
        private IMediator mediator;
        private IRandom random;

        public SponsoredTossQueryHandler(ICosmosDBTemplate<TossEntity> cosmosDBTemplate, IMediator mediator, IRandom random)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
            this.mediator = mediator;
            this.random = random;
        }

        public async Task<TossLastQueryItem> Handle(SponsoredTossQuery request, CancellationToken cancellationToken)
        {
            var resCollection = (await cosmosDBTemplate.CreateDocumentQuery<SponsoredTossEntity>())
                .Where(s => s.Type == nameof(SponsoredTossEntity))
                .Where(s => s.Content.Contains("#" + request.Hashtag))
                .Where(s => s.DisplayedCount > 0)
                .Select(t => new TossLastQueryItem()
                {
                    Content = t.Content,
                    CreatedOn = t.CreatedOn,
                    Id = t.Id,
                    UserName = t.UserName
                })
                
                .AsEnumerable()
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
