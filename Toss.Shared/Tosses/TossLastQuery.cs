using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toss.Shared;

namespace Toss.Shared.Tosses
{
    public class TossLastQuery : IRequest<IEnumerable<TossLastQueryItem>>
    {
        public const int TossPerPage = 10;
        public TossLastQuery()
        {
        }

        public TossLastQuery(string hashTag, int page = 0)
        {
            HashTag = hashTag;
            Page = page;
        }

        [Required]
        public string HashTag { get; set; }
        public int Page { get; set; }
    }
}
