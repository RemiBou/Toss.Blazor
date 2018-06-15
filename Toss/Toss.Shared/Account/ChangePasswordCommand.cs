using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Shared
{
    public class ChangePasswordCommand : MediatR.IRequest<CommandResult>
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }

    public class CommandResult
    {
        public Dictionary<string, string> Errors { get; set; }
        public bool IsSucess
        {
            get
            {
                return Errors == null || !Errors.Any();
            }
        }
        public CommandResult()
        {

        }
        public CommandResult(string errorKey,string errorMessage)
        {
            Errors = new Dictionary<string, string>()
            {
                {errorKey,errorMessage}
            };
        }
         public CommandResult(Dictionary<string,string> errors)
        {
            Errors = errors;
        }
        public static CommandResult Success()
        {
            return new CommandResult();
        }
    }
}
