using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toss.Shared;

namespace Toss.Server.Controllers
{
    public class LastTossQuery : IRequest<IEnumerable<TossLastQueryItem>>
    {
        public LastTossQuery()
        {
        }

        public LastTossQuery(string hashTag)
        {
            HashTag = hashTag;
        }

        [Required]
        public string HashTag { get; set; }
    }
}
