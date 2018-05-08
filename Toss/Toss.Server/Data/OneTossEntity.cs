using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Data
{
    public class OneTossEntity : TableEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public OneTossEntity(string content, string userId, DateTimeOffset dateOfPost)
        {
            Content = content;
            UserId = userId;
            CreatedOn = dateOfPost;
            PartitionKey = "AllToss";
            //cf https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-design-guide#log-tail-pattern
            RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - dateOfPost.Ticks);

        }

        public OneTossEntity()
        {
        }
    }
}
