using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toss.Shared.Tosses
{
    public class MessageInConversationQuery : IRequest<MessageInConversationQueryResult>
    {
        public MessageInConversationQuery(string conversationId)
        {
            ConversationId = conversationId ?? throw new ArgumentNullException(nameof(conversationId));
        }

        [Required]
        public string ConversationId { get; set; }
    }
}
