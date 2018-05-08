using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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
            _sut = new TossAzureTableRepository(storageClient);
            _tableReference = storageClient.GetTableReference("Toss");
        }

        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {
            var batchOperation = new TableBatchOperation();
            
            await _tableReference.CreateIfNotExistsAsync();
            for (int i = 0; i < 60; i++)
            {


                batchOperation.Insert(new OneTossEntity("lorem ipsum", "blabla", new DateTime(2017, 12, 31).AddDays(-i)));
                
            }

            await _tableReference.ExecuteBatchAsync(batchOperation);

            var res = await _sut.Last(50);

            Assert.Equal(50, res.Count());
            Assert.Null(res.FirstOrDefault(r => r.DateOfPost < new DateTime(2017, 12, 31).AddDays(-50)));
        }

        [Fact]
        public async Task last_return_field_mapped_to_toss_last_item()
        {

        }
        [Fact]
        public async Task last_create_the_table_if_not_exists()
        {

        }
        [Fact]
        public async Task create_create_the_table_if_not_exists()
        {

        }
        [Fact]
        public async Task create_insert_item_in_azure_table()
        {

        }

        public void Dispose()
        {
            _tableReference.DeleteAsync().Wait();
        }
    }
}
