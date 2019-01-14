using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Services;

namespace Toss.Server.Models.Tosses
{
    public class TagsCountByDayIndexService : INotificationHandler<TossCreated>
    {
        private ICosmosDBTemplate<TagByDayIndex> _indexCosmosDBTemplate;
        private INow _now;

        public TagsCountByDayIndexService(ICosmosDBTemplate<TagByDayIndex> indexCosmosDBTemplate, INow now)
        {
            _indexCosmosDBTemplate = indexCosmosDBTemplate;
            _now = now;
        }

        public async Task Handle(TossCreated notification, CancellationToken cancellationToken)
        {
            var query = await _indexCosmosDBTemplate.CreateDocumentQuery();
            var today = _now.Get().Date;
            foreach (var tag in notification.Toss.Tags)
            {
                var index = new TagByDayIndex(tag, _now.Get());
                await _indexCosmosDBTemplate.Insert(index);
            }
        }
    }
}
