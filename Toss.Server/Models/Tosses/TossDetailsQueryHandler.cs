using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class TossDetailsQueryHandler : IRequestHandler<TossDetailQuery, TossDetail>
    {
        private readonly IAsyncDocumentSession _session;

        public TossDetailsQueryHandler(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<TossDetail> Handle(TossDetailQuery request, CancellationToken cancellationToken)
        {
            return await _session.Query<TossEntity>()
                .Where(t => t.Id == request.TossId)
                .Select(t => new TossDetail()
                {
                    Content = t.Content,
                    CreatedOn = t.CreatedOn,
                    UserName = t.UserName,
                    Id = t.Id
                })
                .FirstOrDefaultAsync();
        }
    }
}
