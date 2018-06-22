using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Data
{
    [Collection("AzureTablecollection")]
    public class UserStoreV2Tests 
    {
        private readonly AzureTableFixture azureTableFixture;

        public UserStoreV2Tests(AzureTableFixture azureTableFixture)
        {
            this.azureTableFixture = azureTableFixture;
        }

        [Fact]
        public async Task UpdateAsync_saves_hashtags()
        {
           
            await azureTableFixture.UserStoreV2.CreateTablesIfNotExists();
            var user = new ApplicationUser() { UserName = "toto" };
            await azureTableFixture.UserStoreV2.CreateAsync(user);
            user.Hashtags = new HashSet<string> { "test", "test2" };
            await azureTableFixture.UserStoreV2.UpdateAsync(user);

            var user2 = await azureTableFixture.UserStoreV2.FindByNameAsync("toto");

            Assert.Equal(new HashSet<string> { "test", "test2" }, user2.Hashtags);
            await azureTableFixture.UserStoreV2.DeleteAsync(user);

        }
    }
}
