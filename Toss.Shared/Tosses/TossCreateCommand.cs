using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Toss.Shared.Account;

namespace Toss.Shared.Tosses
{
    public class TossCreateCommand : IRequest
    {
        public const string TagRegexString = @"(?<=[\s>]|^)#(\w*[A-Za-z_]+\w*)";

        public static Regex TagRegex = new Regex(TagRegexString,RegexOptions.Compiled);
        [Required]
        [MaxLength(32000)]
        [MinLength(20)]
        [RegularExpression("(?s)(.)*(?<=#)" + AddHashtagCommand.HashTagRegex + "(.)*", ErrorMessage = "Your toss must contain at least one hashtag (#)")]
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

        public TossCreateCommand()
        {
        }

        public TossCreateCommand(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}