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
    public class UserStoreV2Tests : IDisposable
    {
        ApplicationDbContext applicationDbContext = AzureTableHelper.GetApplicationDbContext();
        public void Dispose()
        {
            applicationDbContext.IndexTable.DeleteAsync().Wait();
            applicationDbContext.RoleTable.DeleteAsync().Wait();
            applicationDbContext.UserTable.DeleteAsync().Wait();
        }

        [Fact]
        public async Task UpdateAsync_saves_hashtags()
        {
            var sut = new UserStoreV2<ApplicationUser, IdentityRole, ApplicationDbContext>(applicationDbContext);

            await sut.CreateTablesIfNotExists();
            var user = new ApplicationUser() { UserName = "toto" };
            await sut.CreateAsync(user);
            user.Hashtags = new HashSet<string> { "test", "test2" };
            await sut.UpdateAsync(user);

            var user2 = await sut.FindByNameAsync("toto");

            Assert.Equal(new HashSet<string> { "test", "test2" }, user2.Hashtags);


        }
    }
}
