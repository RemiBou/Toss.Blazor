using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
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
        private readonly Mock<IMediator> _mediator;
        public AccountControllerTest()
        {
            _userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _signInManager = MockHelpers.MockSigninManager(_userManager.Object);
            _emailSender = new Mock<IEmailSender>();
            _logger = new Mock<ILogger<AccountController>>();
            _mediator = new Mock<IMediator>();
            _sut = new AccountController(_userManager.Object,
                _signInManager.Object,
                _emailSender.Object,
                _logger.Object,
                _mediator.Object);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
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
        public async Task AddHashTag_if_not_ok_return_bad_request()
        {
            _mediator.Setup(u => u.Send(It.IsAny<AddHashtagCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new CommandResult("test", "test 2")));

            var res = await _sut.AddHashTag("toto");

            _mediator.Verify(u => u.Send(It.IsAny<AddHashtagCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(res);
            var errors = Assert.IsType<ValidationErrorDictonary>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("test"));
        }
        [Fact]
        public async Task AddHashTag_if_ok_returns_ok()
        {
            _mediator.Setup(u => u.Send(It.IsAny<AddHashtagCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(CommandResult.Success()));

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
