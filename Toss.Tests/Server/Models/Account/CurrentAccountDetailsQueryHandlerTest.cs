using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Toss.Server.Models.Account;
using Toss.Shared;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class CurrentAccountDetailsQueryHandlerTest : BaseCosmosTest
    {
        [Fact]
        public async Task Details_return_account_view_model()
        {
            var res = await _mediator.Send(
                new CurrentAccountDetailsQuery());
            Assert.NotNull(res);
        }
        [Fact]
        public async Task Details_return_user_hashtags()
        { 
            await _mediator.Send(new AddHashtagCommand("toto"));
            await _mediator.Send(new AddHashtagCommand("titi"));

            var res = await _mediator.Send(
                new CurrentAccountDetailsQuery());
           
            Assert.Equal(new HashSet<string> { "toto", "titi" }, res.Hashtags);
        }

        [Fact]
        public async Task Details_when_user_has_password_return_HasPassword_to_true()
        {
            await EditCurrentUser(u => u.PasswordHash = "AAA");
            var res = await _mediator.Send(new CurrentAccountDetailsQuery());

            Assert.True(res.HasPassword);
        }
        [Fact]
        public async Task Details_when_user_has_no_password_return_HasPassword_to_false()
        {
            var user = await _userManager.GetUserAsync(TestFixture.ClaimPrincipal);
            user.PasswordHash = null;
            await _userManager.UpdateAsync(user);
            var res = await _mediator.Send(new CurrentAccountDetailsQuery());

            Assert.False(res.HasPassword);
        }

        [Fact]
        public async Task Handle_when_user_has_role_admin_return_IsAdmin_true()
        {
            var user = await _userManager.GetUserAsync(TestFixture.ClaimPrincipal);
            user.AddRole("Admin");
            await _userManager.UpdateAsync(user);

            var res = await _mediator.Send(new CurrentAccountDetailsQuery());

            Assert.True(res.IsAdmin);
        }
    }
}
