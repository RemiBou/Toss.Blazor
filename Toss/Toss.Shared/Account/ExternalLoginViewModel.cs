using System.ComponentModel.DataAnnotations;

namespace Toss.Shared
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
