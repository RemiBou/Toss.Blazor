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
        private readonly ITossRepository tossRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TossCreateCommandHandler(ITossRepository tossRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.tossRepository = tossRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(TossCreateCommand createTossCommand, CancellationToken cancellationToken)
        {
            createTossCommand.UserId = _httpContextAccessor.HttpContext.User.Identity.Name;
            createTossCommand.CreatedOn = DateTimeOffset.Now;
            await tossRepository.Create(createTossCommand);
            return Unit.Value;
        }
    }
}
