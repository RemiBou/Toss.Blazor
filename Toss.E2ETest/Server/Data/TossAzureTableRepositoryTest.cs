using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Data
{
    [Collection("AzureTablecollection")]
    public class TossCosmosDBRepository
    {
        private readonly TossCosmosRepository _sut;
        private readonly Uri _collectionLink;
        private readonly AzureTableFixture azureTableFixture;



        public TossCosmosDBRepository(AzureTableFixture azureCosmoFixture)
        {
            this.azureTableFixture = azureCosmoFixture;
            _sut = new TossCosmosRepository(azureCosmoFixture.Client, "UnitTests");
            _collectionLink = UriFactory.CreateDocumentCollectionUri("UnitTests", "Toss"));
        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {


            for (int i = 0; i < 60; i++)
            {
                await azureTableFixture.Client.CreateDocumentAsync(_collectionLink, new OneTossEntity("lorem ipsum", "blabla", new DateTime(2017, 12, 31).AddDays(-i)));
            }

            var res = await _sut.Last(50, null);

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-50)));
        }

        [Fact]
        public async Task last_returns_last_items_ids()
        {


            await azureTableFixture.Client.CreateDocumentAsync(_collectionLink, new OneTossEntity("lorem ipsum", "blabla", new DateTime(2017, 12, 31)));


            var res = await _sut.Last(50, null);

            Assert.Single(res);
            Assert.NotNull(res.First().Id);
        }

        [Fact]
        public async Task last_create_the_table_if_not_exists()
        {
            await azureTableFixture.TossTable.DeleteAsync();

            var res = (await _sut.Last(50, null));

            Assert.True(await azureTableFixture.TossTable.ExistsAsync());
        }
        [Fact]
        public async Task create_create_the_table_if_not_exists()
        {
            await azureTableFixture.TossTable.DeleteAsync();

            await _sut.Create(new TossCreateCommand() { Content = "lorem ipsum", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" });

            Assert.True(await azureTableFixture.TossTable.ExistsAsync());
        }
        [Fact]
        public async Task create_insert_item_in_azure_table()
        {
            var command = new TossCreateCommand() { Content = "lorem ipsum", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" };
            await _sut.Create(command);

            TableContinuationToken tableContinuationToken = null;
            var toss = (await azureTableFixture.TossTable
                .ExecuteQuerySegmentedAsync(new TableQuery<OneTossEntity>(), tableContinuationToken))
                .Results
                .First();

            Assert.Equal("lorem ipsum", toss.Content);
            Assert.Equal(command.CreatedOn, toss.CreatedOn);
        }



        [Fact]
        public async Task last_returns_toss_matching_hashtag()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(_sut.Create(new TossCreateCommand() { Content = "lorem #ipsum #toto num" + i, CreatedOn = DateTimeOffset.Now, UserId = "usernametest" }));

            }
            tasks.Add(_sut.Create(new TossCreateCommand() { Content = "blabla #ipsum #tutu", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" }));
            await Task.WhenAll(tasks);

            var tosses = await _sut.Last(2, "toto");
            Assert.Equal(2, tosses.Count());
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("#tutu")));
            Assert.Null(tosses.FirstOrDefault(t => t.Content.Contains("num0")));

        }
    }
}
