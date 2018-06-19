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
using Toss.Shared.Services;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class ChangePasswordCommandHandlerTest
    {
        private ChangePasswordCommandHandler _sut;
        private ClaimsPrincipal _user;
        private ApplicationUser _applicationUser;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<ILogger<AccountController>> _logger;
        private readonly Mock<IHttpContextAccessor> _httpCOntextAccessor;
        public ChangePasswordCommandHandlerTest()
        {
            _httpCOntextAccessor = new Mock<IHttpContextAccessor>();
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _signInManager = MockHelpers.MockSigninManager(_userManager.Object);
            _emailSender = new Mock<IEmailSender>();
            _logger = new Mock<ILogger<AccountController>>();
            _sut = new ChangePasswordCommandHandler(
                _userManager.Object,
                _signInManager.Object,
                _emailSender.Object,
                _logger.Object,
                _httpCOntextAccessor
                .Object);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
            _userManager.Setup(u => u.GetUserAsync(_user))
              .ReturnsAsync(_applicationUser);
            _userManager.Setup(u => u.ChangePasswordAsync(_applicationUser, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));
            _httpCOntextAccessor
                .SetupGet(h => h.HttpContext)
                .Returns(new DefaultHttpContext() { User = _user });
        }

        [Fact]
        public async Task Handle_when_user_has_no_password_returns_error()
        {
            _applicationUser.PasswordHash = null;

            var res = await _sut.Handle(new Shared.ChangePasswordCommand()
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
