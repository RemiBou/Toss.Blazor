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
using Toss.Server.Controllers;
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
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();

        public CurrentAccountDetailsQueryHandlerTest()
        {
            _sut = new CurrentAccountDetailsQueryHandler(
                _m.HttpContextAccessor
                    .Object,
                _m.UserManager.Object
              );
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
            _m.UserManager.Setup(u => u.GetUserAsync(_m.User))
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
            _m.ApplicationUser.PasswordHash = null;

            var res = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());

            Assert.False(res.HasPassword);
        }

        [Fact]
        public async Task Handle_when_user_has_role_admin_return_IsAdmin_true()
        {
            _m.ApplicationUser.AddRole("Admin");

             var res = await _sut.Handle(new CurrentAccountDetailsQuery(), new CancellationToken());

            Assert.True(res.IsAdmin);
        }
    }
}
