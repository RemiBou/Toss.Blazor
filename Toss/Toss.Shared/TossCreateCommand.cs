using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Toss.Shared
{
    public class TossCreateCommand : IRequest
    {
        [Required]
        [MaxLength(32000)]
        [MinLength(20)]
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}