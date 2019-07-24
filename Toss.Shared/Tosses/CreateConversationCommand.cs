using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class CreateConversationCommand : IRequest
    {
        public CreateConversationCommand()
        {
        }

        public CreateConversationCommand(string tossId)
        {
            TossId = tossId;
        }

        [Required]
        public String TossId { get; set; }
    }
}