using System;

namespace Toss.Shared.Tosses
{
    public class TossConversationQueryResultMessage
    {
        public DateTimeOffset CreatedOn { get; set; }
        public String Content { get; set; }
        public String UserName { get; set; }
    }
}