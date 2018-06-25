using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared;

namespace Toss.Server.Controllers
{
    public class LastTossQueryHandler : IRequestHandler<LastTossQuery, IEnumerable<TossLastQueryItem>>
    {
        private readonly ITossRepository tossRepository;

        public LastTossQueryHandler(ITossRepository tossRepository)
        {
            this.tossRepository = tossRepository;
        }

        public async Task<IEnumerable<TossLastQueryItem>> Handle(LastTossQuery request, CancellationToken cancellationToken)
        {
            return (await tossRepository.Last(50, request.HashTag)).ToList();
        }
    }
}
