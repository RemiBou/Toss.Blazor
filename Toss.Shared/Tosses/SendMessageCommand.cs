using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class SendMessageCommand : IRequest
    {
        [Required]
        public String ConversationId { get; set; }

        [Required]
        public String Message { get; set; }
    }
}