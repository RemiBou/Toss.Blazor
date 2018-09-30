using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    internal class SponsoredTossDisplayed : INotification
    {
        public SponsoredTossDisplayed(string id)
        {
            this.TossId = id;
        }

        public string TossId { get; set; }
    }

    internal class SponsoredTossDisplayCountReducer : INotificationHandler<SponsoredTossDisplayed>
    {
        private ICosmosDBTemplate<SponsoredTossEntity> cosmosDBTemplate;

        public SponsoredTossDisplayCountReducer(ICosmosDBTemplate<SponsoredTossEntity> cosmosDBTemplate)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
        }

        public async Task Handle(SponsoredTossDisplayed notification, CancellationToken cancellationToken)
        {
            var toss = (await cosmosDBTemplate.CreateDocumentQuery())
                .Where(t => t.Id == notification.TossId)
                .Take(1)
                .AsEnumerable()
                .FirstOrDefault();
            //we displayed a now removed
            if (toss == null)
                return;
            toss.DecreaseDisplayCount();
           await  cosmosDBTemplate.Update(toss);

        }
    }
}