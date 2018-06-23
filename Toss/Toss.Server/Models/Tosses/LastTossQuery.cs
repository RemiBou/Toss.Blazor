using MediatR;
using System.Collections.Generic;
using Toss.Shared;

namespace Toss.Server.Controllers
{
    public class LastTossQuery : IRequest<IEnumerable<TossLastQueryItem>>
    {
        public string HashTag { get; set; }
    }
}
