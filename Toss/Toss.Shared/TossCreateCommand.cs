using System;
using System.ComponentModel.DataAnnotations;

namespace Toss.Shared
{
    public class TossCreateCommand
    {
        [Required]
        [StringLength(32000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 20)]
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}