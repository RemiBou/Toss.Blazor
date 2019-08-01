using System;
using System.Collections.Generic;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TossConversation : RavenDBDocument
    {
        public TossConversation(TossEntity toss, ApplicationUser user)
        {
            TossId = toss.Id;
            CreatorUserId = user.Id;
            Messages = new List<TossConversationMessage>();
        }

        public List<TossConversationMessage> Messages { get; private set; }

        public String TossId { get; private set; }

        public String CreatorUserId { get; private set; }

        internal void AddMessage(ApplicationUser currentUser, string message, DateTimeOffset creationDate)
        {
            Messages.Add(new TossConversationMessage(message, currentUser.Id, creationDate));
        }

        internal bool CanSendMessage(TossEntity toss, ApplicationUser currentUser)
        {
            return currentUser.Id != CreatorUserId && currentUser.Id != toss.UserId;
        }
    }
}