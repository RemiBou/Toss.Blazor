using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Account
{
    public class ForgotPasswordCommand: IRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
