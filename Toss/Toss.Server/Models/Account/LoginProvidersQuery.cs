using System.Collections.Generic;
using MediatR;
using Toss.Shared;
using Toss.Shared.Account;

namespace Toss.Server.Models.Account
{
    public class LoginProvidersQuery : IRequest<IEnumerable<SigninProviderViewModel>>
    {

    }
}
