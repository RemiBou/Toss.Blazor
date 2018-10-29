using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Models;
using Toss.Server.Models.Account;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class ChangePasswordCommandHandlerTest : BaseCosmosTest
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
