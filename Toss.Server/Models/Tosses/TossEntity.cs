using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TossEntity : RavenDBDocument
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
