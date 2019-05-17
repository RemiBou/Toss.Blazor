﻿using System;
using System.Collections.Generic;

namespace Toss.Shared.Tosses
{
    public class TossLastQueryItem
    {
        public string UserName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Content { get; set; }
        public TimeSpan PostedAgo
        {
            get
            {
                return DateTimeOffset.Now - CreatedOn;
            }
        }

        public string Id { get; set; }

        public List<string> Tags { get; set; }
    }
}