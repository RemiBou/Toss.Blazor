using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;

namespace Toss.Server.Models.Tosses
{
    /// <summary>
    /// This class will be responsible for managing the displaying of sponsored toss
    /// </summary>
    public class SponsoredTossService : INotificationHandler<TossCreated>
    {
        private ICosmosDBTemplate<TossEntity> cosmosDBTemplate;

        public SponsoredTossService(ICosmosDBTemplate<TossEntity> cosmosDBTemplate)
        {
            this.cosmosDBTemplate = cosmosDBTemplate;
        }

        public Task Handle(TossCreated notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new object());
        }


    }

    public class SponsoredTossTagIndex
    {
        public string Tag { get; set; }

    }
    public class SponsoredTossTagIndexUser
    {
        public string UserName { get; set; }
        public List<string> TossIds { get; set; }
    }
}
