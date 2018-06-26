using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class TossListAdminQueryHandler : IRequestHandler<TossListAdminQuery, List<TossListAdminItem>>
    {
        public Task<List<TossListAdminItem>> Handle(TossListAdminQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
