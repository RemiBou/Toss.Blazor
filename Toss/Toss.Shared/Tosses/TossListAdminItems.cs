using System.Collections.Generic;
using Toss.Shared.Tosses;

namespace Toss.Shared.Tosses
{
    public class TossListAdminItems
    {
        public TossListAdminItems()
        {
        }

        public TossListAdminItems(List<TossListAdminItem> result, int count)
        {
            if(count < 0)
                throw new System.ArgumentException(nameof(count));
            Result = result ?? throw new System.ArgumentNullException(nameof(result));
            Count = count;
        }

        public List<TossListAdminItem> Result { get; set; }

        public int Count { get; set; }
    }
}
