using System.Collections.Generic;

namespace Toss.Shared.Tosses
{
    public class TossConversationQueryResult
    {
        public string TossCreatorUserId { get; set; }
        public List<TossConversationQueryResultItem> Conversations { get; set; }
    }
}