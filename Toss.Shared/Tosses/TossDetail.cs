using System;
using System.Collections.Generic;

namespace Toss.Shared.Tosses
{
    public class TossDetail
    {
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }

        public List<string> Hashtags { get; set; }
    }
}