using MediatR;
using System;
using System.Collections.Generic;

namespace Toss.Shared.Tosses
{
    public class TossListAdminQuery : IRequest<TossListAdminItems>
    {
        public TossListAdminQuery()
        {
        }

        public TossListAdminQuery(int itemCount, DateTimeOffset? maxDate)
        {
            ItemCount = itemCount;
            MaxDate = maxDate;
        }

        public int ItemCount { get; set; }
        /// <summary>
        /// 1 based page number
        /// </summary>
        public DateTimeOffset? MaxDate { get; set; }
    }
}