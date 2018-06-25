using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using Toss.Shared.Account;

namespace Toss.Shared
{
    public class TossCreateCommand : IRequest
    {
        [Required]
        [MaxLength(32000)]
        [MinLength(20)]
        [RegularExpression(".*(?<=#)"+AddHashtagCommand.HashTagRegex+".*", ErrorMessage ="Your toss must contain at least one hashtag (#)")]
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}