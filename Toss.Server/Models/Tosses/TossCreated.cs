using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Server.Models.Tosses
{
    public class TossCreated : INotification
    {
        public TossCreated(string tossId)
        {
            TossId = tossId ?? throw new ArgumentNullException(nameof(tossId));
        }

        public string TossId { get; set; }

    }
}
