using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Documents.Linq;
using Toss.Server.Data;
using Toss.Shared.Tosses;

namespace Toss.Server.Models.Tosses
{
    public class TossDetailsQueryHandler : IRequestHandler<TossDetailQuery, TossDetail>
    {
        private readonly ICosmosDBTemplate<TossEntity> _dbTemplate;

        public TossDetailsQueryHandler(ICosmosDBTemplate<TossEntity> dbTemplate)
        {
            _dbTemplate = dbTemplate;
        }

        public async Task<TossDetail> Handle(TossDetailQuery request, CancellationToken cancellationToken)
        {
            var res =await (await _dbTemplate.CreateDocumentQuery<TossEntity>())
                .Where(t => t.Id == request.TossId)
                .Select(t => new TossDetail()
                {
                    Content = t.Content,
                    CreatedOn = t.CreatedOn,
                    UserName = t.UserName,
                    Id = t.Id
                })
                .AsDocumentQuery()
                .GetAllResultsAsync();
            return res.FirstOrDefault();
        }
    }
}
