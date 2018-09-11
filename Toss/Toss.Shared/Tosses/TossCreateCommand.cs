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
        [RegularExpression("(?s)(.)*(?<=#)" + AddHashtagCommand.HashTagRegex+"(.)*", ErrorMessage ="Your toss must contain at least one hashtag (#)")]
        public string Content { get; set; }

        [Range(50,1000)]
        public int? SponsoredDisplayedCount { get; set; }

        public string SponsoredDisplayedCountStr
        {
            get
            {
                return SponsoredDisplayedCount.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    SponsoredDisplayedCount = null;
                else
                {
                    int parseResult;
                    if (int.TryParse(value, out parseResult))
                    {
                        SponsoredDisplayedCount = parseResult;
                    }
                    else
                    {
                        SponsoredDisplayedCount = null;
                    }
                }
            }
        }

        public string StripeChargeToken { get; set; }

        public const int CtsCostPerDisplay = 1;
    }
}