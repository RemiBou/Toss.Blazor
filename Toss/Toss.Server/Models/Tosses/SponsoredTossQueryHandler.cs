using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class SponsoredTossQueryHandler : IRequestHandler<SponsoredTossQuery, TossLastQueryItem>
    {
        private ICosmosDBTemplate<TossEntity> cosmosDBTemplate;
        private IMediator mediator;

        public SponsoredTossQueryHandler(ICosmosDBTemplate<TossEntity> cosmosDBTemplate)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
        }

        public SponsoredTossQueryHandler(ICosmosDBTemplate<TossEntity> cosmosDBTemplate, IMediator mediator) : this(cosmosDBTemplate)
        {
            this.mediator = mediator;
        }

        public async Task<TossLastQueryItem> Handle(SponsoredTossQuery request, CancellationToken cancellationToken)
        {
            var res = (await cosmosDBTemplate.CreateDocumentQuery<SponsoredTossEntity>())
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
                //first or default does not work in the current cosmodb sdk
                .Take(1)
                .AsEnumerable()
                .FirstOrDefault();
            if (res != null)
                await mediator.Publish(new SponsoredTossDisplayed(res.Id));
            return res;
            
        }
    }
}
