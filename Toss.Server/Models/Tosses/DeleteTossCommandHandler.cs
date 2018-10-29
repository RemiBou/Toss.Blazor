using MediatR;
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
        private ICosmosDBTemplate<TossEntity> cosmosDBTemplate;

        public DeleteTossCommandHandler(ICosmosDBTemplate<TossEntity> cosmosDBTemplate)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
        }

        public async Task<Unit> Handle(DeleteTossCommand request, CancellationToken cancellationToken)
        {
            await cosmosDBTemplate.Delete(request.TossId);
            return Unit.Value;
        }
    }
}
