using Newtonsoft.Json;
using System;

namespace Toss.Server.Data
{
    public class SponsoredTossEntity : TossEntity
    {
        public SponsoredTossEntity() { }
        public SponsoredTossEntity(string content, string userId, DateTimeOffset dateOfPost, int displayCount) : base(content, userId, dateOfPost)
        {
            DisplayedCount = displayCount;
            DisplayedCountBought = displayCount;
        }

        [JsonProperty]
        public int DisplayedCount { get; protected set; }

        [JsonProperty]
        public int DisplayedCountBought { get; protected set; }

        /// <summary>
        /// Decreases the display count by 1
        /// </summary>
        internal void DecreaseDisplayCount()
        {
            if (DisplayedCount <= 0)
            {
                DisplayedCount = 0;
                return;
            }
            DisplayedCount--;
        }
    }
}
