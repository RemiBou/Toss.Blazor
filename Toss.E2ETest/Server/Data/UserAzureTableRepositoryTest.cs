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
        private UserAzureTableRepository _sut;
        private IdentityUserV2 _identity;

        public UserAzureTableRepositoryTest()
        {
            _identity = new IdentityUserV2("user test");

             appDbContext.UserTable.CreateIfNotExistsAsync().Wait();
            var store = new UserStoreV2<IdentityUserV2, IdentityRole, ApplicationDbContext>(appDbContext);
            store.CreateAsync(
                _identity
                ).Wait();


            _sut = new UserAzureTableRepository(appDbContext);
        }
        public void Dispose()
        {
            appDbContext.UserTable.DeleteIfExistsAsync().Wait();
        }

        [Fact]
        public async Task GetUserNames_when_user_exists_return_its_non_hashed_name()
        {


            var res = await _sut.GetUserNames(new[] { _identity.UserName });

            Assert.True(res.ContainsKey(_identity.UserName));

            Assert.Equal("user test", res[_identity.UserName]);
            Assert.Single(res);


        }
        [Fact]
        public async Task GetUserNames_dot_not_throw_if_user_doesnot_exists()
        {
            var res = await _sut.GetUserNames(new[] { "blank" });

            Assert.Empty(res);
        }
    }
}
