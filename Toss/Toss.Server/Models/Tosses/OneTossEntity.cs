using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Toss.Server.Data
{
    public class TossEntity
    {
        [Newtonsoft.Json.JsonProperty(PropertyName="id")]
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public TossEntity(string content, string userId, DateTimeOffset dateOfPost)
        {
            Content = content;
            UserName = userId;
            CreatedOn = dateOfPost;
        }

        public TossEntity()
        {
        }
    }

    public class SponsoredTossEntity : TossEntity
    {
        public SponsoredTossEntity() { }
        public SponsoredTossEntity(string content, string userId, DateTimeOffset dateOfPost, int displayCount) : base(content, userId, dateOfPost)
        {
            DisplayedCount = displayCount;
            DisplayedCountBought = displayCount;
        }

        public int DisplayedCount { get; set; }
        public int DisplayedCountBought { get; set; }
    }
}
