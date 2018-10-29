using System.Threading.Tasks;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class RemoveHashtagCommandHandlerTest : BaseCosmosTest
    {
       
        [Fact]
        public async Task RemoveHashTag_calls_user_store_with_hashtag_removed()
        {
            await _mediator.Send(new AddHashtagCommand("test"));

            var res = await _mediator.Send(new RemoveHashTagCommand("test"));           

            Assert.True(res.IsSucess);

            var user = await _userManager.GetUserAsync(TestFixture.ClaimPrincipal);

            Assert.Empty(user.Hashtags);
        }
        

    }
}
