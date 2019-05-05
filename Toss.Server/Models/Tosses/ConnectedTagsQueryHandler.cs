using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data.Indexes;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class ConnectedTagsQueryHandler : IRequestHandler<ConnectedTagsQuery, ConnectedTags>
    {

        private readonly IAsyncDocumentSession _session;

        public ConnectedTagsQueryHandler(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        async Task<ConnectedTags> IRequestHandler<ConnectedTagsQuery, ConnectedTags>.Handle(ConnectedTagsQuery request, CancellationToken cancellationToken)
        {
            var res = await _session.Query<TossConnectedTagsIndex, Toss_ConnectedTags>()
                .Where(t => t.Tag == request.Hashtag)
                .FirstOrDefaultAsync();
            if(res == null)
            {
                return new ConnectedTags(); 
            }
            return new ConnectedTags(res.ConnectedTags);


        }
    }
}
