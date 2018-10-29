using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Tosses;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    
    public class DeleteTossCommandHandlerTest : BaseCosmosTest
    {
        private ICosmosDBTemplate<TossEntity> _cosmosDBTemplateEntity;

        public DeleteTossCommandHandlerTest()
        {
            _cosmosDBTemplateEntity = TestFixture.GetInstance<ICosmosDBTemplate<TossEntity>>();
            
        }

        [Fact]
        public async Task Handle_removes_toss()
        {
            await _cosmosDBTemplateEntity.Insert(new TossEntity("test content", "user test", DateTimeOffset.Now));
            await _cosmosDBTemplateEntity.Insert(new TossEntity("test content2", "user test", DateTimeOffset.Now));
            var allInsertedToss = (await _cosmosDBTemplateEntity.CreateDocumentQuery()).ToList();

            await _mediator.Send(new DeleteTossCommand(allInsertedToss.First().Id));


            var allRemaining = (await _cosmosDBTemplateEntity.CreateDocumentQuery()).ToList();
            Assert.Single(allRemaining);
            Assert.Null(allRemaining.FirstOrDefault(t => t.Id == allInsertedToss.First().Id));
        }
    }
}
