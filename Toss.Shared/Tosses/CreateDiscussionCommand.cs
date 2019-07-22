using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class CreateConversationCommand : IRequest
    {
        [Required]
        public String TossId { get; set; }
    }
}