using MediatR;

namespace Toss.Server.Models.Account
{
    public class CurrentUserQuery : IRequest<ApplicationUser>
    {
        public CurrentUserQuery()
        {
        }
    }
}