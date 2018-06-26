using MediatR;
using System.Collections.Generic;

namespace Toss.Shared.Tosses
{
    public class TossListAdminQuery : IRequest<List<TossListAdminItem>>
    {
    }
}