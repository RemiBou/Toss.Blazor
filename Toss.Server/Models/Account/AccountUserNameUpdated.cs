using MediatR;

namespace Toss.Server.Models.Account
{
    public class AccountUserNameUpdated : INotification
    {
        public AccountUserNameUpdated(ApplicationUser user)
        {
            this.User = user;
        }

        public ApplicationUser User { get; set; }
    }
}