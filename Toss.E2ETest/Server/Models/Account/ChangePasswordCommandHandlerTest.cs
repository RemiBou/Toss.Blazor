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
    public class ChangePasswordCommandHandlerTest
    {
        private readonly ChangePasswordCommandHandler _sut;
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();
        public ChangePasswordCommandHandlerTest()
        {

            _sut = new ChangePasswordCommandHandler(
                _m.UserManager.Object,
                _m.SignInManager.Object,
                new Mock<ILogger<AccountController>>().Object,
                _m.HttpContextAccessor.Object);

            _m.UserManager.Setup(u => u.ChangePasswordAsync(_m.ApplicationUser, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));
        }

        [Fact]
        public async Task Handle_when_user_has_no_password_returns_error()
        {
            _m.ApplicationUser.PasswordHash = null;

            var res = await _sut.Handle(new ChangePasswordCommand()
            {
                ConfirmPassword = "test2",
                NewPassword = "test2",
                OldPassword = "test"
            },
            new System.Threading.CancellationToken());
            _m.UserManager.Verify(u => u.ChangePasswordAsync(_m.ApplicationUser, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _m.EmailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("User"));
        }
    }
}
