using System;

namespace Toss.Shared.Tosses
{
    public class MessageInConversationQueryResultItem
    {
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}