using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Data
{
    [Collection("AzureTablecollection")]
    public class TossCosmosDBRepository : IDisposable
    {
        private readonly TossCosmosRepository _sut;
        private readonly Uri _collectionLink;
        private readonly AzureTableFixture azureTableFixture;



        public TossCosmosDBRepository(AzureTableFixture azureCosmoFixture)
        {
            this.azureTableFixture = azureCosmoFixture;
            _sut = new TossCosmosRepository(azureCosmoFixture.Client, AzureTableFixture.UnitTestsDatabaseId);
            azureCosmoFixture.Init().Wait();
            _collectionLink = UriFactory.CreateDocumentCollectionUri(azureCosmoFixture.Database.Id, "Toss");
        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {


            for (int i = 0; i < 60; i++)
            {
                var command = new TossCreateCommand() { Content = "lorem #ipsum", CreatedOn = new DateTime(2017, 12, 31).AddDays(-i), UserId = "usernametest" };
                await _sut.Create(command);
            }

            var res = await _sut.Last(50, "ipsum");

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-50)));
        }


        [Fact]
        public async Task create_insert_item_in_azure_table()
        {
            var command = new TossCreateCommand() { Content = "lorem ipsum", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" };
            await _sut.Create(command);


            var toss = azureTableFixture.Client.CreateDocumentQuery<OneTossEntity>(_collectionLink)
                .AsEnumerable()
                .FirstOrDefault();
            Assert.NotNull(toss);
            Assert.Equal("lorem ipsum", toss.Content);
            Assert.Equal(command.CreatedOn, toss.CreatedOn);
        }



        [Fact]
        public async Task last_returns_toss_matching_hashtag()
        {
            
            for (int i = 0; i < 3; i++)
            {
                await _sut.Create(new TossCreateCommand() { Content = "lorem #ipsum #toto num" + i, CreatedOn = DateTimeOffset.Now, UserId = "usernametest" });

            }
            await _sut.Create(new TossCreateCommand() { Content = "blabla #ipsum #tutu", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" });
            

            var tosses = await _sut.Last(2, "toto");
            Assert.Equal(2, tosses.Count());
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("#tutu")));
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("num0")));

        }

        public void Dispose()
        {
            azureTableFixture.Clean().Wait();
        }
    }
}
