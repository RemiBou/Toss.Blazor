using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Tosses
{
    public class UserTossListViewQuery : IRequest<IEnumerable<UserTossListViewResult>>
    {
        [Required]
        public string UserName { get; set; }
        public int Page { get; set; }
    }
}