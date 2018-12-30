using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Documents.Linq;
using Toss.Server.Data;
using Toss.Server.Models.Account;

namespace Toss.Server.Models.Tosses
{
    public class TossUserNameUpdater : INotificationHandler<AccountUserNameUpdated>
    {
        private ICosmosDBTemplate<TossEntity> cosmosDBTemplate;

        public TossUserNameUpdater(ICosmosDBTemplate<TossEntity> cosmosDBTemplate)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
        }

        public async Task Handle(AccountUserNameUpdated notification, CancellationToken cancellationToken)
        {
             var tosses = (await (await cosmosDBTemplate.CreateDocumentQuery())
                .Where(t => t.UserId == notification.User.Id)
                .AsDocumentQuery()
                .GetAllResultsAsync())
                .ToList();
            foreach (var item in tosses)
            {
                item.UserName = notification.User.UserName;
                await cosmosDBTemplate.Update(item);
            }
        }
    }
}
