using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class DeleteTossCommandHandler : IRequestHandler<DeleteTossCommand>
    {
        private IAsyncDocumentSession _session;

        public DeleteTossCommandHandler(IAsyncDocumentSession session)
        {
            this._session = session;
        }

        public async Task<Unit> Handle(DeleteTossCommand request, CancellationToken cancellationToken)
        {
            _session.Delete(request.TossId);
            return Unit.Value;
        }
    }
}
