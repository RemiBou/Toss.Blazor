using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class TossDetailQuery : IRequest<TossDetail>
    {
        public TossDetailQuery()
        {
        }

        public TossDetailQuery(string tossId)
        {
            TossId = tossId;
        }

        [Required]
        public string TossId { get; set; }
    }
}
