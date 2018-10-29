using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toss.Shared;

namespace Toss.Shared.Tosses
{
    public class TossLastQuery : IRequest<IEnumerable<TossLastQueryItem>>
    {
        public TossLastQuery()
        {
        }

        public TossLastQuery(string hashTag)
        {
            HashTag = hashTag;
        }

        [Required]
        public string HashTag { get; set; }
    }
}
