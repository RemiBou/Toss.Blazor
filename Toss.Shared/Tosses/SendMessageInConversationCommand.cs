using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class SendMessageInConversationCommand : IRequest
    {
        public SendMessageInConversationCommand(string conversationId, string message)
        {
            ConversationId = conversationId ?? throw new ArgumentNullException(nameof(conversationId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        [Required]
        public String ConversationId { get; set; }

        [Required]
        public String Message { get; set; }
    }
}