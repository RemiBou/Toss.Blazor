using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Toss.Server.Data
{
    public abstract class CosmosDBEntity
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Type { get {
                if (string.IsNullOrEmpty(_type))
                {
                    _type = this.GetType().Name;
                }
                return _type; } set => _type = value; }

        private string _type;
    }
    public class TossEntity : CosmosDBEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string UserName { get; set; }
        public List<string> Tags { get; set; }
        public object UserIp { get; set; }

        public TossEntity(string content, string userId, DateTimeOffset dateOfPost)
        {
            Content = content;
            UserId = userId;
            CreatedOn = dateOfPost;
        }

        public TossEntity()
        {
        }
    }
}
