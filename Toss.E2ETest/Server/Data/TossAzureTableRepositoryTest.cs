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
        private Mock<IUserRepository> mockUserRepository;
        public TossAzureTableRepositoryTest()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true;");
            storageClient = storageAccount.CreateCloudTableClient();
            mockUserRepository = new Mock<IUserRepository>();
            _sut = new TossAzureTableRepository(storageClient, mockUserRepository.Object,tablePrefix:"UnitTests");
            _tableReference = storageClient.GetTableReference("UnitTestsToss");
            _tableReference.CreateIfNotExistsAsync().Wait();
            mockUserRepository.Setup(r => r.GetUserNames(It.IsAny<IEnumerable<string>>()))
               .ReturnsAsync(new Dictionary<string, string>
               {
                    {"blabla","user test" }
               });
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
        public async Task last_toss_field_mapped_to_toss_last_item_query()
        {
            var dateOfCreation = new DateTimeOffset(new DateTime(2017, 12, 31).AddDays(-10));
            var toss = new OneTossEntity("lorem ipsum", "blabla", dateOfCreation);

            await _tableReference.ExecuteAsync(TableOperation.Insert(toss));


            var res = (await _sut.Last(50)).First();

            Assert.Equal("lorem ipsum", res.Content);
            Assert.Equal("user test", res.UserName);
            Assert.Equal(dateOfCreation, res.CreatedOn);
            mockUserRepository.Verify(r => r.GetUserNames(It.Is<IEnumerable<string>>(d => d.Count() == 1 && d.Contains("blabla"))));

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

        public void Dispose()
        {
            _tableReference.DeleteAsync().Wait();
        }
    }
}
