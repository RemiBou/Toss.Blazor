using System.Linq;
using System.Threading.Tasks;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class AccountListQueryHandlerTest :BaseCosmosTest
    {   
        [Fact]
        public async Task AccountListQuery_returns_all_users()
        {
            var res = await _mediator.Send(new Toss.Shared.Account.AccountListQuery());

            Assert.Single(res);
        }

        [Fact]
        public async Task AccountListQuery_maps_result()
        {
            var res = await _mediator.Send(new Toss.Shared.Account.AccountListQuery()
                );
            
            var first = res.FirstOrDefault();
            Assert.Equal("username", first.UserName);
            Assert.NotEmpty(first.Id);
            Assert.Equal("test@yopmail.com", first.Email);
            Assert.True(first.EmailConfirmed);

        }
    }
}
