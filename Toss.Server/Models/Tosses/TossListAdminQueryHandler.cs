using MediatR;
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
        private ICosmosDBTemplate<TossEntity> _tossCosmosDB;

        public TossListAdminQueryHandler(ICosmosDBTemplate<TossEntity> tossCosmosDB)
        {
            _tossCosmosDB = tossCosmosDB;
        }

        public async Task<TossListAdminItems> Handle(TossListAdminQuery request, CancellationToken cancellationToken)
        {
            var count = (await _tossCosmosDB.CreateDocumentQuery<int>("SELECT VALUE COUNT(t) FROM TossEntity t"))
                .AsEnumerable()
                .First();
            var query = (await _tossCosmosDB.CreateDocumentQuery()).AsQueryable();
            if (request.MaxDate.HasValue)
                query = query.Where(t => t.CreatedOn < request.MaxDate);
            var items = query
                 .Take(request.ItemCount)
                 .Select(t => new TossListAdminItem()
                 {
                     Content = t.Content,
                     CreatedOn = t.CreatedOn,
                     Id = t.Id,
                     UserName = t.UserName
                 })
                 
                 .ToList();
            return new TossListAdminItems(items, count);
        }
    }
}
