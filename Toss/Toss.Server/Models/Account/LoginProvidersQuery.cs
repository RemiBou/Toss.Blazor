using Toss.Shared;
using MediatR;
using System.Collections.Generic;

namespace Toss.Server.Controllers
{
    public class LoginProvidersQuery : IRequest<IEnumerable<SigninProviderViewModel>>
    {

    }
}
