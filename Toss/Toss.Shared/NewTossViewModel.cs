using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toss.Shared
{
    public class NewTossViewModel
    {
        [Required]
        [StringLength(32000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 20)]
        public string Content { get; set; }
    }
}
