using MediatR;

namespace Toss.Server.Controllers
{
    public class ExternalLoginCommand : IRequest<Microsoft.AspNetCore.Identity.SignInResult>
    {

    }
}
