using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class TossConversationQuery : IRequest<TossConversationQueryResult>
    {
        [Required]
        public String TossId { get; set; }

    }
}