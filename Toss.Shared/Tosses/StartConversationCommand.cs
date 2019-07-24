using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class StartConversationCommand : IRequest
    {
        public StartConversationCommand()
        {
        }

        public StartConversationCommand(string tossId)
        {
            TossId = tossId;
        }

        [Required]
        public String TossId { get; set; }
    }
}