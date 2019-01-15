using MediatR;

namespace Toss.Server.Models.Account
{
    public class AccountEmailUpdated : INotification
    {

        public AccountEmailUpdated(ApplicationUser user)
        {
            this.User = user;
        }

        public ApplicationUser User { get; set; }
    }
}