using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Account
{
    public class ExternalLoginConfirmationCommand : IRequest<CommandResult>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        public string Provider { get; set; }
    }
}
