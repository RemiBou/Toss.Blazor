using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class TossConversationQuery : IRequest<TossConversationQueryResult>
    {
        public TossConversationQuery()
        {
        }

        public TossConversationQuery(string tossId)
        {
            TossId = tossId;
        }

        [Required]
        public String TossId { get; set; }

    }
}