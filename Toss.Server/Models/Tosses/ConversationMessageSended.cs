using MediatR;

namespace Toss.Server.Models.Tosses
{
    public class ConversationMessageSended : INotification
    {

        public ConversationMessageSended(TossConversation conversation, TossConversationMessage message)
        {
            Conversation = conversation;
            Message = message;
        }

        public TossConversation Conversation { get; }
        public TossConversationMessage Message { get; }
    }
}