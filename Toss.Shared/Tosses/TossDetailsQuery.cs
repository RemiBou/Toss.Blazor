using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class TossDetailsQuery : IRequest<TossDetail>
    {
        public TossDetailsQuery()
        {
        }

        public TossDetailsQuery(string tossId)
        {
            TossId = tossId;
        }

        [Required]
        public string TossId { get; private set; }
    }
}
