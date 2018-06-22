using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Toss.Server.Controllers;
using Toss.Server.Models;
using Toss.Server.Models.Account;
using Toss.Server.Services;
using Toss.Shared;
using Toss.Shared.Account;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Controllers
{
    public class AccountControllerTest
    {
        private readonly AccountController _sut;
        private readonly ClaimsPrincipal _user;
        private readonly ApplicationUser _applicationUser;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<ILogger<AccountController>> _logger;
        private readonly Mock<IMediator> _mediator;
        public AccountControllerTest()
        {
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();
            _signInManager = MockHelpers.MockSigninManager(userManager.Object);
            _logger = new Mock<ILogger<AccountController>>();
            _mediator = new Mock<IMediator>();
            _sut = new AccountController(_signInManager.Object,
                _logger.Object,
                _mediator.Object);
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                     {
                            new Claim(ClaimTypes.Name, "username")
                     }, "someAuthTypeName"));
            _applicationUser = new ApplicationUser() { UserName = "username", PasswordHash = "XXX" };
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = _user
                }
            };
        }
       
        [Fact]
        public async Task Details_when_user_doesnt_exists_return_404()
        {
            _mediator.Setup(u => u.Send(It.IsAny<CurrentAccountDetailsQuery>(),It.IsAny<CancellationToken>()))
                        .ReturnsAsync((AccountViewModel)null);

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
       
      
    }
}
