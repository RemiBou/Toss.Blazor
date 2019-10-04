using MediatR;

namespace Toss.Server.Models.Tosses
{
    public class ConversationStarted : INotification
    {

        public ConversationStarted(TossConversation conversation)
        {
            Conversation = conversation;
        }

        public TossConversation Conversation { get; }
    }
}