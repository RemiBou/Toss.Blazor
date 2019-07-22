using System;

namespace Toss.Server.Models.Tosses
{
    public class TossConversationMessage
    {
        public TossConversationMessage(string content, string userId)
        {
            Content = content;
            CreatedOn = DateTimeOffset.Now;
            UserId = userId;
        }

        public String Content { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public String UserId { get; set; }
    }
}