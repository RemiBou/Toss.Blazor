using System;

namespace Toss.Shared.Tosses
{
    public class TossListAdminItem
    {
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Content { get; set; }

        public string Id { get; set; }
    }
}