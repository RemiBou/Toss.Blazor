using Microsoft.AspNetCore.Identity;
using MediatR;

namespace Toss.Server.Controllers
{
    public class ExternalLoginDetailQuery : IRequest<ExternalLoginInfo>
    {

    }
}
