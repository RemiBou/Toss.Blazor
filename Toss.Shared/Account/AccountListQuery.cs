using MediatR;
using System.Collections.Generic;

namespace Toss.Shared.Account
{
    public class AccountListQuery : IRequest<List<AdminAccountListItem>>
    {
    }
}