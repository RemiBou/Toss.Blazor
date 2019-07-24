using System;
using System.Collections.Generic;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TossConversation : RavenDBDocument
    {
        public TossConversation(string tossId, string userid)
        {
            TossId = tossId;
            CreatorUserId = userid;
            Messages = new List<TossConversationMessage>();
            Id = BuildId(tossId, userid);
        }

        public static String BuildId(string tossId, string userid)
        {
            return String.Format("tossConversations/" + tossId + "/" + userid);
        }

        public List<TossConversationMessage> Messages { get; private set; }

        public String TossId { get; private set; }

        public String CreatorUserId { get; private set; }

        internal void AddMessage(string currentUser, string message)
        {
            Messages.Add(new TossConversationMessage(message, currentUser));
        }
    }
}