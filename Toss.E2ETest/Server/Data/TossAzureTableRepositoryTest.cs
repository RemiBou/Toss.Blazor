using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Xunit;

namespace Toss.Tests.Server.Data
{
    public class TossAzureTableRepositoryTest : IDisposable
    {
        private TossAzureTableRepository _sut;
        private CloudTable _tableReference;
        private readonly CloudTableClient storageClient;
        public TossAzureTableRepositoryTest()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true;");
            storageClient = storageAccount.CreateCloudTableClient();
            _sut = new TossAzureTableRepository(storageClient, tablePrefix:"UnitTests");
            _tableReference = storageClient.GetTableReference("UnitTestsToss");
            _tableReference.CreateIfNotExistsAsync().Wait();
        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {
            var batchOperation = new TableBatchOperation();

            for (int i = 0; i < 60; i++)
            {
                batchOperation.Insert(new OneTossEntity("lorem ipsum", "blabla", new DateTime(2017, 12, 31).AddDays(-i)));
            }

            await _tableReference.ExecuteBatchAsync(batchOperation);

            var res = await _sut.Last(50);

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.CreatedOn < new DateTime(2017, 12, 31).AddDays(-50)));
        }

       
        [Fact]
        public async Task last_create_the_table_if_not_exists()
        {
            await _tableReference.DeleteAsync();

            var res =(await _sut.Last(50));

            Assert.True(await _tableReference.ExistsAsync());
        }
        [Fact]
        public async Task create_create_the_table_if_not_exists()
        {
            await _tableReference.DeleteAsync();

            await _sut.Create(new Shared.TossCreateCommand() { Content = "lorem ipsum", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" });

            Assert.True(await _tableReference.ExistsAsync());
        }
        [Fact]
        public async Task create_insert_item_in_azure_table()
        {
            var command = new Shared.TossCreateCommand() { Content = "lorem ipsum", CreatedOn = DateTimeOffset.Now, UserId = "usernametest" };
            await _sut.Create(command);

            TableContinuationToken tableContinuationToken = null;
            var toss = (await _tableReference
                .ExecuteQuerySegmentedAsync(new TableQuery<OneTossEntity>(), tableContinuationToken))
                .Results
                .First();

            Assert.Equal("lorem ipsum", toss.Content);
            Assert.Equal(command.CreatedOn, toss.CreatedOn);



        }

        [Fact]
        public async Task create_saves_corresponding_hashtag_index()
        {

        } 

        [Fact]
        public async Task last_returns_toss_matching_hashtag()
        {

        }

        public void Dispose()
        {
            _tableReference.DeleteAsync().Wait();
        }
    }
}
