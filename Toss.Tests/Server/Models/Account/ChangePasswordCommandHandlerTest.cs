using System.Threading.Tasks;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class ChangePasswordCommandHandlerTest : BaseTest
    {


        [Fact]
        public async Task Handle_when_user_has_no_password_returns_error()
        {
            await EditCurrentUser(u => u.PasswordHash = null);

            var res = await _mediator.Send(new ChangePasswordCommand()
            {
                ConfirmPassword = "test2",
                NewPassword = "test2",
                OldPassword = "test"
            });
            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("User"));
        }
    }
}
