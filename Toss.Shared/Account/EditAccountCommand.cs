using System.ComponentModel.DataAnnotations;
using MediatR;
using Toss.Shared.Account;

namespace Toss.Shared
{
    public class EditAccountCommand : IRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}