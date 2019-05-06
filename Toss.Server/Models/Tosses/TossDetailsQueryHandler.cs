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
        private readonly RavenDBIdUtil ravenDBIdUtil;

        public TossDetailsQueryHandler(IAsyncDocumentSession session, RavenDBIdUtil ravenDBIdUtil)
        {
            _session = session;
            this.ravenDBIdUtil = ravenDBIdUtil;
        }

        public async Task<TossDetail> Handle(TossDetailQuery request, CancellationToken cancellationToken)
        {
            var t = (await _session.LoadAsync<TossEntity>(ravenDBIdUtil.GetRavenDbIdFromUrlId<TossEntity>(request.TossId)));
            if (t == null)
                return null;
            return new TossDetail()
            {
                Content = t.Content,
                CreatedOn = t.CreatedOn,
                UserName = t.UserName,
                Id = t.Id,
                Hashtags = t.Tags
            };
        }
    }
}
