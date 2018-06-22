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
        private readonly ApplicationUser _applicationUser;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IEmailSender> _emailSender;

        public ChangePasswordCommandHandlerTest()
        {
            var httpCOntextAccessor = new Mock<IHttpContextAccessor>();
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            var signInManager = MockHelpers.MockSigninManager(_userManager.Object);
            _emailSender = new Mock<IEmailSender>();
            _sut = new ChangePasswordCommandHandler(
                _userManager.Object,
                signInManager.Object,
                new Mock<ILogger<ChangePasswordCommandHandler>>().Object,
                httpCOntextAccessor
                .Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "username")
            }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
            _userManager.Setup(u => u.GetUserAsync(user))
              .ReturnsAsync(_applicationUser);
            _userManager.Setup(u => u.ChangePasswordAsync(_applicationUser, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));
            httpCOntextAccessor
                .SetupGet(h => h.HttpContext)
                .Returns(new DefaultHttpContext() { User = user });
        }

        [Fact]
        public async Task Handle_when_user_has_no_password_returns_error()
        {
            _applicationUser.PasswordHash = null;

            var res = await _sut.Handle(new ChangePasswordCommand()
            {
                ConfirmPassword = "test2",
                NewPassword = "test2",
                OldPassword = "test"
            },
            new System.Threading.CancellationToken());
            _userManager.Verify(u => u.ChangePasswordAsync(_applicationUser, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.False(res.IsSucess);
            Assert.True(res.Errors.ContainsKey("User"));
        }
    }
}
