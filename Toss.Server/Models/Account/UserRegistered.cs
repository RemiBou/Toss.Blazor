using MediatR;

namespace Toss.Server.Models.Account
{
    public class UserRegistered : INotification
    {
        public UserRegistered(ApplicationUser user)
        {
            this.User = user;
        }

        public ApplicationUser User { get; private set; }
    }
}