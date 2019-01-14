using System;
using System.Collections.Generic;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TagByDayIndex : CosmosDBEntity
    {
        public TagByDayIndex(string tag, DateTimeOffset createdOn)
        {
            Tag = tag;
            CreatedOn = createdOn;
        }

        public string Tag { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}