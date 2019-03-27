using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents.Session;
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
        private readonly IAsyncDocumentSession _session;

        public SponsoredTossDisplayCountReducer(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task Handle(SponsoredTossDisplayed notification, CancellationToken cancellationToken)
        {
            var toss = await _session.LoadAsync<TossEntity>(notification.TossId) as SponsoredTossEntity;
            //we displayed a now removed
            if (toss == null)
                return;
            toss.DecreaseDisplayCount();
        }
    }
}