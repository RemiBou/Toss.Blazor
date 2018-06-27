using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Controllers
{
    public class TossCreateCommandHandler : IRequestHandler<TossCreateCommand>
    {
        private readonly ICosmosDBTemplate<TossEntity> _dbTemplate;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TossCreateCommandHandler(ICosmosDBTemplate<TossEntity> cosmosTemplate, IHttpContextAccessor httpContextAccessor)
        {
            _dbTemplate = cosmosTemplate;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(TossCreateCommand command, CancellationToken cancellationToken)
        {            
            var toss = new TossEntity(
                command.Content, 
                _httpContextAccessor.HttpContext.User.Identity.Name,
                DateTimeOffset.Now);

            await _dbTemplate.Insert(toss);
            return Unit.Value;
        }
    }
}
