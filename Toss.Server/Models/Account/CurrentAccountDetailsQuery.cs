using System;
using MediatR;
using Toss.Shared;

namespace Toss.Server.Models.Account
{
    public class CurrentAccountDetailsQuery : IRequest<AccountViewModel>
    {
    }
}
