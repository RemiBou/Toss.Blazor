using System.ComponentModel.DataAnnotations;
using MediatR;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class ConfirmEmailCommand : IRequest<CommandResult>
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
