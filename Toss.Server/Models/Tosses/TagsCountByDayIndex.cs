using System;
using System.Collections.Generic;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TagByDayIndex
    {
        public TagByDayIndex()
        {
        }

        public string Tag { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Count { get; set; }


    }
}