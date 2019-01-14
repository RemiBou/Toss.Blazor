using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    public class TossCreated : INotification
    {
        public TossCreated(TossEntity toss)
        {
            this.Toss = toss;
        }

        public TossEntity Toss { get; set; }
    }
}
