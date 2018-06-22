using MediatR;
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
using Toss.Server.Data;
using Toss.Server.Models;
using Toss.Shared;
using Toss.Shared.Services;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Controllers
{
    public class AccountControllerTest
    {
        private AccountController _sut;
        private ClaimsPrincipal _user;
        private ApplicationUser _applicationUser;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<ILogger<AccountController>> _logger;
        public AccountControllerTest()
        {
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _signInManager = MockHelpers.MockSigninManager(_userManager.Object);
            _emailSender = new Mock<IEmailSender>();
            _logger = new Mock<ILogger<AccountController>>();
            _sut = new AccountController(_userManager.Object,
                _signInManager.Object,
                _emailSender.Object,
                _logger.Object,
                new Mock<IMediator>().Object);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash="XXX" };
            _userManager.Setup(u => u.GetUserAsync(_user))
              .ReturnsAsync(_applicationUser);
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = _user
                }
            };
        }
        [Fact]
        public async Task Details_when_user_has_password_return_HasPassword_to_true()
        {
            var res = Assert.IsType<AccountViewModel>(Assert.IsType<OkObjectResult>(await _sut.Details()).Value);

            Assert.True(res.HasPassword);
        }
        [Fact]
        public async Task Details_when_user_has_no_password_return_HasPassword_to_false()
        {
            _applicationUser.PasswordHash = null;

            var res = Assert.IsType<AccountViewModel>(Assert.IsType<OkObjectResult>(await _sut.Details()).Value);

            Assert.False(res.HasPassword);
        }
        [Fact]
        public async Task Details_when_user_doesnt_exists_return_404()
        {
            _userManager.Setup(u => u.GetUserAsync(_user))
              .ReturnsAsync((ApplicationUser)null);
            var res = await _sut.Details();

            var notOk = Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public async Task AddHashTag_calls_user_store_with_new_hashtag()
        {
          

            var res = await _sut.AddHashTag("toto");

            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Contains("toto"))));
        }
        [Fact]
        public async Task AddHashTag_cannot_add_null_hashtag()
        {


            var res = await _sut.AddHashTag(null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(res);
            var errors = Assert.IsType<Dictionary<string,string>>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("newTag"));
            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 1)),Times.Never)    ;
        }
        [Fact]
        public async Task AddHashTag_cannot_add_twice_same_hashtag()
        {


            var res = await _sut.AddHashTag("toto");
            res = await _sut.AddHashTag("toto");
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(res);
            var errors = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("newTag"));
            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 1)), Times.Once);
            _userManager.Verify(u => u.UpdateAsync(It.Is<ApplicationUser>(a => a.Hashtags.Count == 2)), Times.Never);

        }
        [Fact]
        public async Task AddHashTag_returns_ok()
        {
            var res = await _sut.AddHashTag("toto");

            Assert.IsType<OkResult>(res);
        }
        [Fact]
        public async Task Details_return_account_view_model()
        {

            var res = (await _sut.Details());

            var okResult = Assert.IsType<OkObjectResult>(res);

            var viewModel = Assert.IsType<AccountViewModel>(okResult.Value);
        }
        [Fact]
        public async Task Details_return_user_hashtags()
        {
            _userManager.Setup(u => u.GetUserAsync(_user))
               .ReturnsAsync(new ApplicationUser() { UserName = "username", Hashtags = new HashSet<string> { "toto", "titi" } });

            var details = ((await _sut.Details()) as OkObjectResult).Value as AccountViewModel;

            Assert.Equal(new HashSet<string> { "toto", "titi" }, details.Hashtags);


        }
    }
}
