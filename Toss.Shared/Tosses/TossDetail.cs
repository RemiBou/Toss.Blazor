using System;

namespace Toss.Shared.Tosses
{
    public class TossDetail
    {
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }
    }
}