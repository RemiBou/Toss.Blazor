using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using Toss.Shared.Account;

namespace Toss.Shared.Tosses
{
    public class TossCreateCommand : IRequest
    {
        [Required]
        [MaxLength(32000)]
        [MinLength(20)]
        [RegularExpression(".*(?<=#)"+AddHashtagCommand.HashTagRegex+".*", ErrorMessage ="Your toss must contain at least one hashtag (#)")]
        public string Content { get; set; }
    }
}