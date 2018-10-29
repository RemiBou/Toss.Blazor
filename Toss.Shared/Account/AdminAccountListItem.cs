using System.Collections.Generic;

namespace Toss.Shared.Account
{
    public class AdminAccountListItem
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}