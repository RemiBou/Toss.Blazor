using MediatR;
using Raven.Client.Documents.Session;
using System.Threading;
using System.Threading.Tasks;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class DeleteTossCommandHandler : IRequestHandler<DeleteTossCommand>
    {
        private readonly IAsyncDocumentSession _session;

        public DeleteTossCommandHandler(IAsyncDocumentSession session)
        {
            this._session = session;
        }

        public Task<Unit> Handle(DeleteTossCommand request, CancellationToken cancellationToken)
        {
            _session.Delete(request.TossId);
            return Task.FromResult( Unit.Value);
        }
    }
}
