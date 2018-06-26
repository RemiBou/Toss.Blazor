using System;

namespace Toss.Server.Data
{
    public class OneTossEntity
    {
        
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public OneTossEntity(string content, string userId, DateTimeOffset dateOfPost)
        {
            Content = content;
            UserName = userId;
            CreatedOn = dateOfPost;
        }

        public OneTossEntity()
        {
        }
    }
}
