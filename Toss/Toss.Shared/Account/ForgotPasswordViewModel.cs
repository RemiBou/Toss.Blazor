using System.ComponentModel.DataAnnotations;

namespace Toss.Shared.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
