using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Toss.Shared.Account
{
    /// <summary>
    /// This query will return the user description on the user page
    /// </summary>
    public class UserViewQuery : IRequest<UserViewResult>
    {
        [Required]
        public string UserName { get; set; }
    }
}