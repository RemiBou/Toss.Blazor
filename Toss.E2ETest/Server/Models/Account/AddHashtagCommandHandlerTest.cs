using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Toss.Server.Models;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class AddHashtagCommandHandlerTest : BaseCosmosTest
    {
        [Fact]
        public async Task AddHashTag_if_null_hashtag_returns_error()
        {
            var res= await _mediator.Send(
                new AddHashtagCommand(null));

            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("newTag"));
        }
        [Fact]
        public async Task AddHashTag_update_user_with_new_hashtag()
        {
            var res = await _mediator.Send(
               new AddHashtagCommand("toto"));


            var user = await TestFixture.GetInstance<UserManager<ApplicationUser>>().GetUserAsync(TestFixture.ClaimPrincipal);
            Assert.Contains("toto", user.Hashtags);
        }

        [Fact]
        public async Task AddHashTag_cannot_add_twice_same_hashtag()
        {

            var res = await _mediator.Send(
                new AddHashtagCommand("toto"));
            res = await _mediator.Send(
                new AddHashtagCommand("toto"));

            var user = await TestFixture.GetInstance<UserManager<ApplicationUser>>().GetUserAsync(TestFixture.ClaimPrincipal);
            Assert.Contains("toto", user.Hashtags);

            Assert.False(res.IsSucess);
            Assert.Contains("newTag",res.Errors.Keys);

        }

    }
}
