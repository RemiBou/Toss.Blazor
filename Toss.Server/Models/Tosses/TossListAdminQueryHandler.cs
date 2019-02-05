using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class TossListAdminQueryHandler : IRequestHandler<TossListAdminQuery, TossListAdminItems>
    {
        private IAsyncDocumentSession _session;

        public TossListAdminQueryHandler(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<TossListAdminItems> Handle(TossListAdminQuery request, CancellationToken cancellationToken)
        {
            var count = await _session.Query<TossEntity>().CountAsync();
            var query = _session.Query<TossEntity>().AsQueryable();
            if (request.MaxDate.HasValue)
                query = query.Where(t => t.CreatedOn < request.MaxDate);
            var items = await query
                 .Take(request.ItemCount)
                 .Select(t => new TossListAdminItem()
                 {
                     Content = t.Content,
                     CreatedOn = t.CreatedOn,
                     Id = t.Id,
                     UserName = t.UserName
                 })
                 .ToListAsync();
            return new TossListAdminItems(items, count);
        }
    }
}
