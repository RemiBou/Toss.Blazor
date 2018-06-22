using System.ComponentModel.DataAnnotations;

namespace Toss.Shared.Account
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
