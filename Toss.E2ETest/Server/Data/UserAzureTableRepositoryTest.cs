using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Xunit;

namespace Toss.Tests.Server.Data
{
    public class UserAzureTableRepositoryTest : IDisposable
    {
        ApplicationDbContext appDbContext = new ApplicationDbContext(
                   new IdentityConfiguration()
                   {
                       StorageConnectionString = "UseDevelopmentStorage=true;",
                       TablePrefix = "UnitTests"
                   });

        public void Dispose()
        {
            appDbContext.UserTable.DeleteIfExistsAsync().Wait();
        }

        [Fact]
        public async Task GetUserNames_when_user_exists_return_its_non_hashed_name()
        {
            var identity = new IdentityUserV2("user test");

            await appDbContext.UserTable.CreateIfNotExistsAsync();
            var store = new UserStoreV2<IdentityUserV2, IdentityRole, ApplicationDbContext>(appDbContext);
            await store.CreateAsync(
                identity
                );


            var sut = new UserAzureTableRepository(appDbContext);

            var res = await sut.GetUserNames(new[] { identity.RowKey });

            Assert.True(res.ContainsKey(identity.RowKey));

            Assert.Equal("user test", res[identity.RowKey]);
            Assert.Single(res);


        }
    }
}
