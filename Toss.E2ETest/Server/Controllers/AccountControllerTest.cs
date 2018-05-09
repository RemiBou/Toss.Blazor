using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Models;
using Toss.Shared.Services;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Controllers
{
    public class AccountControllerTest
    {
        private AccountController _sut;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<ILogger<AccountController>> _logger;
        public AccountControllerTest()
        {
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _signInManager = MockHelpers.MockSigninManager<ApplicationUser>(_userManager.Object);
            _emailSender = new Mock<IEmailSender>();
            _logger = new Mock<ILogger<AccountController>>();
            _sut = new AccountController(_userManager.Object, _signInManager.Object, _emailSender.Object, _logger.Object);
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, "username")
                    }, "someAuthTypeName"))
                }
            };
        }

        [Fact]
        public async Task Details_when_user_doesnt_exists_return_404()
        {
            var res = await _sut.Details();

            var notOk = Assert.IsType<UnauthorizedResult>(res);
        }
    }
}
