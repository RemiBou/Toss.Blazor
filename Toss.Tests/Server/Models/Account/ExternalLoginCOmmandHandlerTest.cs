using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Models;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Account
{
    public class ExternalLoginCommandHandlerTest : BaseTest
    {
        [Fact]
        public async Task login_external_acccount()
        {
            //Here we test at the UserManager level instead of the command level because signinmanager is hardly mockable in the case of 
            // external login
            var _userManager = TestFixture.GetInstance<UserManager<ApplicationUser>>();
            var user = new ApplicationUser
            {
                UserName = "test",
                Email = "test@monmail.com",
                EmailConfirmed = true
            };
            var info = new UserLoginInfo("Google", "123456", "Google");
            var result = await _userManager.CreateAsync(user);

            result = await _userManager.AddLoginAsync(user, info);

            var externalLogin = await (TestFixture.GetInstance<IUserStore<ApplicationUser>>() as IUserLoginStore<ApplicationUser>).FindByLoginAsync("Google", "123456", new CancellationToken());

            Assert.NotNull(externalLogin);
        }


    }
}
