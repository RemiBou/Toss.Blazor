using Microsoft.AspNetCore.Identity.DocumentDB;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Models;
using Toss.Server.Models.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class AccountListQueryHandlerTest
    {
        private AccountListQueryHandler _sut;
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();
        public AccountListQueryHandlerTest()
        {
            _sut = new AccountListQueryHandler(_m.UserManager.Object);
        }

        [Fact]
        public async Task Handle_calls_get_user_in_role_empty()
        {
            _m.UserManager.Setup(u => u.GetUsersInRoleAsync(""))
               .ReturnsAsync(new List<ApplicationUser>());

            var res = await _sut.Handle(new Toss.Shared.Account.AccountListQuery(), new System.Threading.CancellationToken());

            _m.UserManager.Verify(u => u.GetUsersInRoleAsync(""), Times.Once());

        }

        [Fact]
        public async Task Handle_maps_result()
        {
            _m.UserManager.Setup(u => u.GetUsersInRoleAsync(""))
                .ReturnsAsync(new List<ApplicationUser>(){
                    new ApplicationUser() {
                        UserName = "test un",
                        Id = "test id",
                        Email = "toto@gmail.com",
                        EmailConfirmed = true
                    }
                });
            var res = await _sut.Handle(new Toss.Shared.Account.AccountListQuery(), new System.Threading.CancellationToken());

            Assert.Single(res);
            var first = res.FirstOrDefault();
            Assert.Equal("test un", first.UserName);
            Assert.Equal("test id", first.Id);
            Assert.Equal("toto@gmail.com", first.Email);
            Assert.True(first.EmailConfirmed);

        }
    }
}
