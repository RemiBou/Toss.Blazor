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
        private CommonMocks<AccountController> _m = new CommonMocks<AccountController>();
        private readonly Mock<IMediator> _mediator;
        public AccountControllerTest()
        {
           
            _mediator = new Mock<IMediator>();
            _sut = new AccountController(_m.SignInManager.Object,
                _m.Logger.Object,
                _mediator.Object);
            _m.SetControllerContext(_sut);
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

            var res = await _sut.AddHashTag(new AddHashtagCommand("toto"));

            _mediator.Verify(u => u.Send(It.IsAny<AddHashtagCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(res);
            var errors = Assert.IsType<Dictionary<string,List<string>>>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("test"));
        }
        [Fact]
        public async Task AddHashTag_if_ok_returns_ok()
        {
            _mediator.Setup(u => u.Send(It.IsAny<AddHashtagCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(CommandResult.Success()));

            var res = await _sut.AddHashTag(new AddHashtagCommand("toto"));

            Assert.IsType<OkResult>(res);
        }
       
      
    }
}
