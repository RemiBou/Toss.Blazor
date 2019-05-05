using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class ConnectedTagsQueryHandler : IRequestHandler<ConnectedTagsQuery, ConnectedTags>
    {  
        Task<ConnectedTags> IRequestHandler<ConnectedTagsQuery, ConnectedTags>.Handle(ConnectedTagsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
