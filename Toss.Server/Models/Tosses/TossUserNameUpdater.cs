using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Server.Models.Account;

namespace Toss.Server.Models.Tosses
{
    public class TossUserNameUpdater : INotificationHandler<AccountUserNameUpdated>
    {
        private readonly IAsyncDocumentSession _session;

        public TossUserNameUpdater(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task Handle(AccountUserNameUpdated notification, CancellationToken cancellationToken)
        {
            var tosses = await _session.Query<TossEntity>()
               .Where(t => t.UserId == notification.User.Id)               
               .ToListAsync();
            foreach (var item in tosses)
            {
                item.UserName = notification.User.UserName;
            }
        }
    }
}
