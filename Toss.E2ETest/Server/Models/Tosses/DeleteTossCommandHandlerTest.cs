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
    [Collection("CosmosDBFixture Collection")]
    public class DeleteTossCommandHandlerTest : BaseCosmosTest, IClassFixture<CosmosDBFixture>
    {
        private CosmosDBTemplate<TossEntity> _cosmosDBTemplateEntity;
        private DeleteTossCommandHandler _sut;

        public DeleteTossCommandHandlerTest(CosmosDBFixture cosmosDBFixture) : base(cosmosDBFixture)
        {
            _cosmosDBTemplateEntity = new CosmosDBTemplate<TossEntity>(cosmosDBFixture.Client, new CosmosDBTemplateOptions() { DataBaseName = CosmosDBFixture.DatabaseName });
            _sut = new DeleteTossCommandHandler(_cosmosDBTemplateEntity);
        }

        [Fact]
        public async Task Handle_removes_toss()
        {
            await _cosmosDBTemplateEntity.Insert(new TossEntity("test content", "user test", DateTimeOffset.Now));
            await _cosmosDBTemplateEntity.Insert(new TossEntity("test content2", "user test", DateTimeOffset.Now));
            var allInsertedToss = (await _cosmosDBTemplateEntity.CreateDocumentQuery()).ToList();

            await _sut.Handle(new DeleteTossCommand(allInsertedToss.First().Id), new System.Threading.CancellationToken());


            var allRemaining = (await _cosmosDBTemplateEntity.CreateDocumentQuery()).ToList();
            Assert.Single(allRemaining);
            Assert.Null(allRemaining.FirstOrDefault(t => t.Id == allInsertedToss.First().Id));
        }
    }
}
