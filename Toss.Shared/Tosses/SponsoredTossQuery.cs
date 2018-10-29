using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toss.Shared.Tosses
{
    public class SponsoredTossQuery : IRequest<TossLastQueryItem>
    {
        public SponsoredTossQuery()
        {
        }

        public SponsoredTossQuery(string hashtag)
        {
            Hashtag = hashtag;
        }

        [Required]
        public string Hashtag { get; set; }
    }
}
