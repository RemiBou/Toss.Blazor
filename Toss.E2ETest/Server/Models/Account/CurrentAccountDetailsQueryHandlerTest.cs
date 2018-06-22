using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Toss.Server.Models;
using Toss.Server.Models.Account;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class CurrentAccountDetailsQueryHandlerTest
    {
        private readonly CurrentAccountDetailsQueryHandler _sut;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly ClaimsPrincipal _user;
        private ApplicationUser _applicationUser;

        public CurrentAccountDetailsQueryHandlerTest()
        {
            var httpCOntextAccessor = new Mock<IHttpContextAccessor>();
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _sut = new CurrentAccountDetailsQueryHandler(
                httpCOntextAccessor
                    .Object,
                _userManager.Object
              );
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "username")
            }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
            _userManager.Setup(u => u.GetUserAsync(_user))
                .ReturnsAsync(_applicationUser);
            _userManager.Setup(u => u.ChangePasswordAsync(_applicationUser, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));
            httpCOntextAccessor
                .SetupGet(h => h.HttpContext)
                .Returns(new DefaultHttpContext() { User = _user });
        }

        [Fact]
        public async Task Details_return_account_view_model()
        {

            var res = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());
            Assert.NotNull(res);
            Assert.IsType<AccountViewModel>(res);
        }
        [Fact]
        public async Task Details_return_user_hashtags()
        {
            _userManager.Setup(u => u.GetUserAsync(_user))
                .ReturnsAsync(new ApplicationUser() { UserName = "username", Hashtags = new HashSet<string> { "toto", "titi" } });

            var details = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());

            Assert.Equal(new HashSet<string> { "toto", "titi" }, details.Hashtags);
        }

        [Fact]
        public async Task Details_when_user_has_password_return_HasPassword_to_true()
        {
            var res = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());

            Assert.True(res.HasPassword);
        }
        [Fact]
        public async Task Details_when_user_has_no_password_return_HasPassword_to_false()
        {
            _applicationUser.PasswordHash = null;

            var res = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());

            Assert.False(res.HasPassword);
        }
    }
}
